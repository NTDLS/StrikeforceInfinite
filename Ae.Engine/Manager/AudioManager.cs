using Ae.Audio;
using Ae.Library;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class AudioManager
    {
        private readonly SiEngine _engine;

        public SiAudioClip? BackgroundMusicSound { get; private set; }
        public SiAudioClip? RadarBlipsSound { get; private set; }
        public SiAudioClip? DoorIsAjarSound { get; private set; }
        public SiAudioClip? LockedOnBlip { get; private set; }
        public SiAudioClip? Click { get; private set; }

        public AudioManager(SiEngine engine)
        {
            _engine = engine;

            engine.OnInitializationComplete += (SiEngine engine) =>
            {
                Click = _engine.Assets.GetAudio("Sounds/Other/Click");
                DoorIsAjarSound = _engine.Assets.GetAudio("Sounds/Ship/Door Is Ajar");
                RadarBlipsSound = _engine.Assets.GetAudio("Sounds/Ship/Radar Blips");
                LockedOnBlip = _engine.Assets.GetAudio("Sounds/Ship/Locked On");
                BackgroundMusicSound = _engine.Assets.GetAudio("Sounds/Music/Background");
            };
        }

        public void PlayRandomShieldHit()
        {
            _engine.Assets.GetAudio("Sounds/Ship/Shield Hit").Play();
        }

        public void PlayRandomHullHit()
        {
            _engine.Assets.GetAudio("Sounds/Ship/Object Hit").Play();
        }

        public void PlayRandomExplosion()
        {
            var assetKeys = _engine.Assets.GetAssetKeysInPath("Sounds/Explode");
            _engine.Assets.GetAudio(SiRandom.OneOf(assetKeys)).Play();
        }
    }
}
