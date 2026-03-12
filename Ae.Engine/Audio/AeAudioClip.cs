using Ae.Engine.Metadata;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Engine.Audio
{
    /// <summary>
    /// A single pre-loaded audio-clip.
    /// </summary>
    [AssetClass("Sound", "", AeBaseAssetType.Sound, true)]
    public class AeAudioClip
    {
        private readonly XAudio2 _audio = new();
        private readonly WaveFormat _waveFormat;
        private readonly AudioBuffer _buffer;
        private readonly SoundStream _soundStream;
        private SourceVoice? _singleSourceVoice;
        private bool _loopForever;
        private bool _isPlaying = false; //Only applicable when _loopForever == false;
        private bool _isFading;
        internal float InitialVolume { get; private set; }

        internal void SetVolume(float volume)
        {
            _singleSourceVoice?.SetVolume(volume);
        }

        internal void SetInitialVolume(float volume)
        {
            InitialVolume = volume;
        }

        internal void SetLoopForever(bool loopForever)
        {
            _loopForever = loopForever;
        }

        internal AeAudioClip(Stream stream, float initialVolume = 1, bool loopForever = false)
        {
            _loopForever = loopForever;
            InitialVolume = initialVolume;

            _ = new MasteringVoice(_audio); //Yes, this is required.

            _soundStream = new SoundStream(stream);

            _waveFormat = _soundStream.Format;
            _buffer = new AudioBuffer
            {
                Stream = _soundStream.ToDataStream(),
                AudioBytes = (int)_soundStream.Length,
                Flags = BufferFlags.EndOfStream,
            };

            if (loopForever)
            {
                _buffer.LoopCount = 100;
            }
        }

        /// <summary>
        /// Starts playback of the audio stream. If looping is enabled, playback will continue indefinitely until
        /// stopped.
        /// </summary>
        /// <remarks>If playback is already in progress and fading is active, calling this method will
        /// cancel the fade and restore the initial volume. This method is thread-safe and can be called multiple times
        /// without causing overlapping playback.</remarks>
        public void Play()
        {
            lock (this)
            {
                if (_loopForever == true)
                {
                    if (_isPlaying)
                    {
                        if (_isFading)
                        {
                            _isFading = false;
                            _singleSourceVoice?.SetVolume(InitialVolume);
                        }

                        return;
                    }

                    _singleSourceVoice = new SourceVoice(_audio, _waveFormat, true);
                    _singleSourceVoice.SubmitSourceBuffer(_buffer, _soundStream.DecodedPacketsInfo);
                    _singleSourceVoice.SetVolume(InitialVolume);
                    _singleSourceVoice.Start();
                    _isPlaying = true;
                    return;
                }
            }

            var sourceVoice = new SourceVoice(_audio, _waveFormat, true);
            sourceVoice.SubmitSourceBuffer(_buffer, _soundStream.DecodedPacketsInfo);
            sourceVoice.SetVolume(InitialVolume);
            sourceVoice.Start();
        }

        /// <summary>
        /// Initiates a fade-out operation if playback is active and no fade is currently in progress.
        /// </summary>
        /// <remarks>This method starts the fade asynchronously. If a fade is already in progress or
        /// playback is not active, calling this method has no effect.</remarks>
        public void Fade()
        {
            if (_isPlaying && _isFading == false)
            {
                _isFading = true;
                Task.Run(FadeThread);
            }
        }

        /// <summary>
        /// Gradually reduces the volume of the single audio source to zero in a background thread.
        /// </summary>
        /// <remarks>This method is intended to be run on a separate thread and will decrement the volume
        /// in steps until it reaches zero. Once fading is complete, the audio source is stopped. The method assumes
        /// that the audio source and fading state are properly managed by the caller. This method is not thread-safe
        /// and should not be called concurrently.</remarks>
        private void FadeThread()
        {
            float volume;

            if (_singleSourceVoice != null)
            {
                _singleSourceVoice.GetVolume(out volume);

                while (_isFading && volume > 0)
                {
                    volume -= 0.25f;
                    volume = volume < 0 ? 0 : volume;
                    _singleSourceVoice.SetVolume(volume);
                    Thread.Sleep(100);
                }
                Stop();
            }
        }

        /// <summary>
        /// Stops audio playback if looping is enabled.
        /// </summary>
        /// <remarks>This method is intended for use when audio playback is set to loop continuously. It
        /// does not support stopping overlapped audio scenarios.</remarks>
        /// <exception cref="Exception">Thrown if the audio is configured for overlapped playback and cannot be stopped using this method.</exception>
        public void Stop()
        {
            if (_loopForever == true)
            {
                if (_singleSourceVoice != null && _isPlaying)
                {
                    _singleSourceVoice.Stop();
                }
                _isPlaying = false;
                _isFading = false;
            }
            else
            {
                throw new Exception("Cannot stop overlapped audio.");
            }
        }
    }
}
