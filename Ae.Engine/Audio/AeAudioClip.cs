using Ae.Engine.Metadata;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Engine.Audio
{
    /// <summary>
    /// Represents an audio clip asset that can be played, looped, faded, or stopped using the underlying audio engine.
    /// </summary>
    /// <remarks>An instance of AeAudioClip manages the playback of a specific audio asset loaded from the
    /// engine's asset system. It supports both one-shot and looping playback modes, as determined by the asset's
    /// metadata. The class is not thread-safe for concurrent playback operations. Dispose of the instance to release
    /// any resources associated with looping playback.</remarks>
    [AssetClass("Sound", "", AeBaseAssetType.Sound, true)]
    public class AeAudioClip
        : IDisposable
    {
        /// <summary>
        /// Gets the instance of the underlying AeEngine used by the class.
        /// </summary>
        public AeEngine Engine { get; private set; }
        private readonly Lock _syncRoot = new();

        private readonly AssetMetadata _metadata;
        private readonly WaveFormat _waveFormat;
        private readonly byte[] _audioBytes;

        private SourceVoice? _singleSourceVoice;
        private MemoryStream? _loopStream;
        private SoundStream? _loopSoundStream;
        private DataStream? _loopDataStream;

        private bool _isPlaying = false;
        private bool _isFading = false;

        internal float Volume { get; private set; }

        internal void SetVolume(float volume)
        {
            Volume = volume;
            _singleSourceVoice?.SetVolume(volume);
        }

        /// <summary>
        /// Initializes a new instance of the AeAudioClip class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The audio engine instance used to access assets and audio resources. Cannot be null.</param>
        /// <param name="assetKey">The key identifying the audio asset to load. Cannot be null or empty.</param>
        public AeAudioClip(AeEngine engine, string assetKey)
        {
            Engine = engine;

            var asset = Engine.Assets.GetAsset(assetKey);
            _metadata = asset.Metadata;
            Volume = _metadata.SoundVolume ?? 1.0f;
            _audioBytes = (byte[])asset.Object;

            using var stream = new MemoryStream(_audioBytes, writable: false);
            using var soundStream = new SoundStream(stream);

            _waveFormat = soundStream.Format;
        }

        /// <summary>
        /// Begins playback of the audio. If the sound is configured to loop, playback will repeat continuously until
        /// stopped; otherwise, the audio is played once.
        /// </summary>
        /// <remarks>If the audio is already playing in loop mode and is currently fading, calling this
        /// method will cancel the fade and restore the original volume. This method is thread-safe.</remarks>
        public void Play()
        {
            if (_metadata.LoopSound == true)
            {
                lock (_syncRoot)
                {
                    if (_isPlaying)
                    {
                        if (_isFading)
                        {
                            _isFading = false;
                            _singleSourceVoice?.SetVolume(Volume);
                        }

                        return;
                    }

                    _loopStream = new MemoryStream(_audioBytes, writable: false);
                    _loopSoundStream = new SoundStream(_loopStream);
                    _loopDataStream = _loopSoundStream.ToDataStream();

                    var buffer = new AudioBuffer
                    {
                        Stream = _loopDataStream,
                        AudioBytes = (int)_loopSoundStream.Length,
                        Flags = BufferFlags.EndOfStream,
                        LoopCount = AudioBuffer.LoopInfinite
                    };

                    _singleSourceVoice = new SourceVoice(Engine.Audio.AudioEngine, _waveFormat, true);
                    _singleSourceVoice.SubmitSourceBuffer(buffer, _loopSoundStream.DecodedPacketsInfo);
                    _singleSourceVoice.SetVolume(Volume);
                    _singleSourceVoice.Start();

                    _isPlaying = true;
                    return;
                }
            }

            PlayOneShot();
        }

        private void PlayOneShot()
        {
            _isPlaying = true;

            var oneShotStream = new MemoryStream(_audioBytes, writable: false);
            var oneShotSoundStream = new SoundStream(oneShotStream);
            var oneShotDataStream = oneShotSoundStream.ToDataStream();

            var oneShotBuffer = new AudioBuffer
            {
                Stream = oneShotDataStream,
                AudioBytes = (int)oneShotSoundStream.Length,
                Flags = BufferFlags.EndOfStream
            };

            var sourceVoice = new SourceVoice(Engine.Audio.AudioEngine, _waveFormat, true);
            sourceVoice.SubmitSourceBuffer(oneShotBuffer, oneShotSoundStream.DecodedPacketsInfo);
            sourceVoice.SetVolume(Volume);
            sourceVoice.Start();

            _ = Task.Run(() =>
            {
                try
                {
                    while (sourceVoice.State.BuffersQueued > 0)
                    {
                        Thread.Sleep(10);
                    }
                    _isPlaying = false;
                }
                finally
                {
                    sourceVoice.DestroyVoice();
                    sourceVoice.Dispose();
                    oneShotDataStream.Dispose();
                    oneShotSoundStream.Dispose();
                    oneShotStream.Dispose();
                }
            });
        }

        /// <summary>
        /// Initiates a fade-out operation if playback is active and no fade is currently in progress.
        /// </summary>
        /// <remarks>This method has no effect if playback is not active or a fade operation is already
        /// running. The fade operation is performed asynchronously.</remarks>
        public void Fade(float fadeDecrements = 0.25f, int fadeDelayMilliseconds = 100)
        {
            if (_isPlaying && _isFading == false)
            {
                _isFading = true;
                Task.Run(() =>
                {
                    var voice = _singleSourceVoice;
                    if (voice == null)
                    {
                        return;
                    }

                    voice.GetVolume(out float volume);

                    while (_isFading && volume > 0)
                    {
                        volume -= fadeDecrements;
                        if (volume < 0)
                        {
                            volume = 0;
                        }

                        voice.SetVolume(volume);
                        Thread.Sleep(fadeDelayMilliseconds);
                    }

                    Stop();
                });
            }
        }

        /// <summary>
        /// Stops playback of the looping sound and releases associated resources.
        /// </summary>
        /// <remarks>This method is intended for use with looping sounds only. If called while a
        /// non-looping (overlapped) audio is playing, an exception is thrown. After calling this method, the sound
        /// cannot be resumed and all related resources are disposed.</remarks>
        /// <exception cref="Exception">Thrown if the current audio is not a looping sound and cannot be stopped using this method.</exception>
        public void Stop()
        {
            if (_metadata.LoopSound != true)
            {
                throw new Exception("Cannot stop overlapped audio.");
            }

            lock (_syncRoot)
            {
                if (_singleSourceVoice != null && _isPlaying)
                {
                    _singleSourceVoice.Stop();
                    _singleSourceVoice.FlushSourceBuffers();
                    _singleSourceVoice.DestroyVoice();
                    _singleSourceVoice.Dispose();
                    _singleSourceVoice = null;
                }

                _loopDataStream?.Dispose();
                _loopDataStream = null;

                _loopSoundStream?.Dispose();
                _loopSoundStream = null;

                _loopStream?.Dispose();
                _loopStream = null;

                _isPlaying = false;
                _isFading = false;
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        /// <remarks>If looping sound playback is active, this method stops the playback before releasing
        /// resources. After calling this method, the instance should not be used.</remarks>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_metadata.LoopSound == true)
            {
                Stop();
            }
        }
    }
}