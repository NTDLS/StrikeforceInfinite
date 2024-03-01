﻿using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace Si.Audio
{
    /// <summary>
    /// A single pre-loaded audio-clip.
    /// </summary>
    public class SiAudioClip
    {
        private bool _isRunningHeadless = false;
        private readonly XAudio2 _xaudio = new();
        private readonly WaveFormat _waveFormat;
        private readonly AudioBuffer _buffer;
        private readonly SoundStream _soundstream;
        private SourceVoice? _singleSourceVoice;
        private bool _loopForever;
        private bool _isPlaying = false; //Only applicable when _loopForever == false;
        private bool _isFading;
        public float InitialVolumne { get; private set; }

        public void SetVolume(float volumne)
        {
            _singleSourceVoice?.SetVolume(volumne);
        }

        public void SetInitialVolumne(float volumne)
        {
            InitialVolumne = volumne;
        }

        public void SetLoopForever(bool loopForever)
        {
            _loopForever = loopForever;
        }

        public SiAudioClip(bool isRunningHeadless, Stream stream, float initialVolumne = 1, bool loopForever = false)
        {
            _isRunningHeadless = isRunningHeadless;
            _loopForever = loopForever;
            InitialVolumne = initialVolumne;

            _ = new MasteringVoice(_xaudio); //Yes, this is required.

            _soundstream = new SoundStream(stream);

            _waveFormat = _soundstream.Format;
            _buffer = new AudioBuffer
            {
                Stream = _soundstream.ToDataStream(),
                AudioBytes = (int)_soundstream.Length,
                Flags = BufferFlags.EndOfStream,
            };

            if (loopForever)
            {
                _buffer.LoopCount = 100;
            }
        }

        public void Play()
        {
            if (_isRunningHeadless)
            {
                return;
            }

            lock (this)
            {
                if (_loopForever == true)
                {
                    if (_isPlaying)
                    {
                        if (_isFading)
                        {
                            _isFading = false;
                            _singleSourceVoice?.SetVolume(InitialVolumne);
                        }

                        return;
                    }

                    _singleSourceVoice = new SourceVoice(_xaudio, _waveFormat, true);
                    _singleSourceVoice.SubmitSourceBuffer(_buffer, _soundstream.DecodedPacketsInfo);
                    _singleSourceVoice.SetVolume(InitialVolumne);
                    _singleSourceVoice.Start();
                    _isPlaying = true;
                    return;
                }
            }

            var sourceVoice = new SourceVoice(_xaudio, _waveFormat, true);
            sourceVoice.SubmitSourceBuffer(_buffer, _soundstream.DecodedPacketsInfo);
            sourceVoice.SetVolume(InitialVolumne);
            sourceVoice.Start();
        }

        public void Fade()
        {
            if (_isRunningHeadless)
            {
                return;
            }
            if (_isPlaying && _isFading == false)
            {
                _isFading = true;
                new Thread(FadeThread).Start();
            }
        }

        private void FadeThread()
        {
            float volumne;

            if (_singleSourceVoice != null)
            {
                _singleSourceVoice.GetVolume(out volumne);

                while (_isFading && volumne > 0)
                {
                    volumne -= 0.25f;
                    volumne = volumne < 0 ? 0 : volumne;
                    _singleSourceVoice.SetVolume(volumne);
                    Thread.Sleep(100);
                }
                Stop();
            }
        }

        public void Stop()
        {
            if (_isRunningHeadless)
            {
                return;
            }
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