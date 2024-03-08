using Newtonsoft.Json;
using Si.Engine.Interrogation._Superclass;
using Si.Engine.Manager;
using Si.Engine.Menu;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController.PlayerSpriteTickController;
using Si.Engine.TickController.UnvectoredTickController;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using Si.Rendering;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.Engine
{
    /// <summary>
    /// The core game engine. Containd the controllers and managers.
    /// </summary>
    public class EngineCore
    {
        #region Backend variables.

        private readonly EngineWorldClock _worldClock;

        #endregion

        #region Public properties.

        public bool IsRunning { get; private set; } = false;

        #endregion

        #region Managers. 

        public EngineInputManager Input { get; private set; }
        public EngineDisplayManager Display { get; private set; }
        public EngineSpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public EngineAudioManager Audio { get; private set; }
        public EngineAssetManager Assets { get; private set; }
        public EngineInterrogationManager Debug { get; private set; }

        #endregion

        #region Tick Controllers.

        public SituationsTickController Situations { get; private set; }
        public EventsTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }
        public MenusTickController Menus { get; private set; }
        public SiRendering Rendering { get; private set; }
        public SiEngineSettings Settings { get; private set; }

        #endregion

        #region Events.

        public delegate void StartEngineEvent(EngineCore sender);
        public event StartEngineEvent OnStartEngine;

        public delegate void StopEngineEvent(EngineCore sender);
        public event StopEngineEvent OnStopEngine;

        #endregion

        /// <summary>
        /// Initializes a new instace of the game engine.
        /// </summary>
        /// <param name="drawingSurface">The window that the game will be rendered to.</param>
        public EngineCore(Control drawingSurface)
        {
            Settings = LoadSettings();

            Display = new EngineDisplayManager(this, drawingSurface);
            Assets = new EngineAssetManager(this);
            Sprites = new EngineSpriteManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationsTickController(this);
            Events = new EventsTickController(this);
            Audio = new EngineAudioManager(this);
            Menus = new MenusTickController(this);
            Player = new PlayerSpriteTickController(this);
            Rendering = new SiRendering(Settings, drawingSurface, Display.TotalCanvasSize);

            _worldClock = new EngineWorldClock(this);
        }

        public void EnableDebugging(IInterrogationForm debugForm)
        {
            Debug = new EngineInterrogationManager(this, debugForm);
        }

        public static SiEngineSettings LoadSettings()
        {
            var engineSettingsText = EngineAssetManager.GetUserText("Engine.Settings.json");

            if (string.IsNullOrEmpty(engineSettingsText))
            {
                var defaultSettings = new SiEngineSettings();

                int x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
                int y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                if (x % 2 != 0) x++;
                if (y % 2 != 0) y++;
                defaultSettings.Resolution = new Size(x, y);

                engineSettingsText = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
                EngineAssetManager.PutUserText("Engine.Settings.json", engineSettingsText);
            }

            return JsonConvert.DeserializeObject<SiEngineSettings>(engineSettingsText);
        }

        public static void SaveSettings(SiEngineSettings settings)
        {
            EngineAssetManager.PutUserText("Engine.Settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void ResetGame()
        {
            Sprites.PlayerStatsText.Visable = false;
            Situations.End();
            Sprites.DeleteActionSprites();
        }

        public void StartGame()
        {
            Sprites.DeleteActionSprites();
            Situations.AdvanceLevel();
        }

        public void RenderEverything()
        {
            try
            {
                Rendering.RenderTargets.Use(o =>
                {
                    o.ScreenRenderTarget.BeginDraw();
                    o.IntermediateRenderTarget.BeginDraw();

                    o.ScreenRenderTarget.Clear(Rendering.Materials.Colors.Black);
                    o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Black);

                    Sprites.RenderPreScaling(o.IntermediateRenderTarget);
                    o.IntermediateRenderTarget.EndDraw();

                    if (Settings.EnableSpeedScaleFactoring)
                    {
                        Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, (float)Display.SpeedOrientedFrameScalingFactor());
                    }
                    else
                    {
                        Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, (float)Display.BaseDrawScale);
                    }
                    Sprites.RenderPostScaling(o.ScreenRenderTarget);

                    o.ScreenRenderTarget.EndDraw();
                });
            }
            catch
            {
            }
        }

        public void StartEngine()
        {
            if (IsRunning)
            {
                throw new Exception("The game engine is already running.");
            }

            IsRunning = true;
            Sprites.Start();
            //Sprites.ResetPlayer();
            _worldClock.Start();

            var textBlock = Sprites.TextBlocks.Create(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Red, new SiPoint(100, 100), true);

            textBlock.SetTextAndCenterXY("Building cache...");

            var percentTextBlock = Sprites.TextBlocks.Create(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Red, new SiPoint(textBlock.X, textBlock.Y + 50), true);

            textBlock.SetTextAndCenterXY("Building reflection cache...");
            SiReflection.BuildReflectionCacheOfType<SpriteBase>();

            if (Settings.PreCacheAllAssets)
            {
                textBlock.SetTextAndCenterXY("Building asset cache...");
                Assets.PreCacheAllAssets(percentTextBlock);
            }

            textBlock.QueueForDelete();
            percentTextBlock.QueueForDelete();

            OnStartEngine?.Invoke(this);

            if (Settings.PlayMusic)
            {
                Audio.BackgroundMusicSound.Play();
            }

            Events.Add(1, () => Menus.Show(new MenuStartNewGame(this)));
        }

        public void ShutdownEngine()
        {
            if (IsRunning)
            {
                IsRunning = false;

                OnStopEngine?.Invoke(this);

                _worldClock.Dispose();
                Sprites.Dispose();
                Rendering.Dispose();
                Assets.Dispose();
            }
        }

        public bool IsPaused() => _worldClock.IsPaused();
        public void TogglePause() => _worldClock.TogglePause();
        public void Pause() => _worldClock.Pause();
        public void Resume() => _worldClock.Resume();
    }
}