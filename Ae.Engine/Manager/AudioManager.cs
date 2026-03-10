using Ae.Engine.Audio;
using Ae.Engine.Helpers;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class AudioManager
    {
        private readonly AeEngine _engine;

        public AeAudioClip? BackgroundMusicSound { get; private set; }
        public AeAudioClip? RadarBlipsSound { get; private set; }
        public AeAudioClip? DoorIsAjarSound { get; private set; }
        public AeAudioClip? LockedOnBlip { get; private set; }
        public AeAudioClip? Click { get; private set; }

        public AudioManager(AeEngine engine)
        {
            _engine = engine;

            engine.OnInitializationComplete += (AeEngine engine) =>
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
            _engine.Assets.GetAudio(AeRandom.OneOf(assetKeys)).Play();
        }
    }
}
