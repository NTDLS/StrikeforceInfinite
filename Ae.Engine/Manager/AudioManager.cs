using Ae.Engine.Helpers;
using SharpDX.XAudio2;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class AudioManager
    {
        private readonly AeEngine _engine;

        /// <summary>
        /// Gets the XAudio2 audio engine instance used for audio processing and playback.
        /// </summary>
        public XAudio2 AudioEngine { get; private set; } = new();

        /// <summary>
        /// Gets the mastering voice used for audio output.
        /// </summary>
        public MasteringVoice MasteringVoice { get; private set; }

        /// <summary>
        /// Gets the audio clip used as background music for the engine.
        /// </summary>
        public void BackgroundMusicSound() => _engine.Assets.GetAudio("Sounds/Music/Background")?.Play();
        /// <summary>
        /// Gets the audio clip that is played for radar blips.
        /// </summary>
        public void RadarBlipsSound() => _engine.Assets.GetAudio("Sounds/Ship/Radar Blips")?.Play();
        /// <summary>
        /// Gets the audio clip that plays when the game has ended. (This IS a Shoutout to Hellfighter by Dracova!!)
        /// </summary>
        public void DoorIsAjarSound() => _engine.Assets.GetAudio("Sounds/Ship/Door Is Ajar")?.Play();
        /// <summary>
        /// Gets the audio clip that is played when a target is locked on.
        /// </summary>
        public void LockedOnBlip() => _engine.Assets.GetAudio("Sounds/Ship/Locked On")?.Play();
        /// <summary>
        /// General menu click sound. Used for buttons and other UI elements.
        /// </summary>
        public void Click() => _engine.Assets.GetAudio("Sounds/Other/Click")?.Play();

        /// <summary>
        /// Initializes a new instance of the AudioManager class and sets up audio assets when the engine initialization
        /// is complete.
        /// </summary>
        /// <remarks>Audio assets are loaded after the engine signals that initialization is complete.
        /// Ensure that the engine is fully initialized before accessing audio properties.</remarks>
        /// <param name="engine">The AeEngine instance used to access audio assets and trigger initialization events. Cannot be null.</param>
        public AudioManager(AeEngine engine)
        {
            _engine = engine;

            //Create and configure a mastering voice.
            MasteringVoice = new MasteringVoice(AudioEngine);
        }

        /// <summary>
        /// Plays a shield hit sound effect using a randomly selected audio asset.
        /// </summary>
        /// <remarks>This method is typically used to provide audio feedback when a ship's shield is
        /// struck. The specific sound played may vary depending on the available assets.</remarks>
        public void PlayRandomShieldHit()
        {
            _engine.Assets.GetAudio("Sounds/Ship/Shield Hit").Play();
        }

        /// <summary>
        /// Plays a random hull hit sound effect for the ship.
        /// </summary>
        /// <remarks>This method triggers an audio cue indicating that the ship has been struck. Use this
        /// method to provide feedback for collision or impact events in gameplay.</remarks>
        public void PlayRandomHullHit()
        {
            _engine.Assets.GetAudio("Sounds/Ship/Object Hit").Play();
        }

        /// <summary>
        /// Plays a randomly selected explosion sound effect from the available assets.
        /// </summary>
        /// <remarks>This method selects an explosion sound from the 'Sounds/Explode' asset path and plays
        /// it. Use this method to add audio feedback for explosion events in the game. The selection is random each
        /// time the method is called.</remarks>
        public void PlayRandomExplosion()
        {
            var assetKeys = _engine.Assets.GetAssetKeysInPath("Sounds/Explode");
            _engine.Assets.GetAudio(AeRandom.OneOf(assetKeys)).Play();
        }
    }
}
