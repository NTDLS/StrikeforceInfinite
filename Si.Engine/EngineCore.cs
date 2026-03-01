using NTDLS.Persistence;
using NTDLS.Semaphore;
using Si.Engine.AI._Superclass;
using Si.Engine.EngineLibrary;
using Si.Engine.Interrogation._Superclass;
using Si.Engine.Manager;
using Si.Engine.Menu;
using Si.Engine.MultiPlay;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.TickController.PlayerSpriteTickController;
using Si.Engine.TickController.UnvectoredTickController;
using Si.Library;
using Si.Library.Mathematics;
using Si.MpClientToServerComms;
using Si.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static Si.Library.SiConstants;

namespace Si.Engine
{
    /// <summary>
    /// The core game engine. Contained the controllers and managers.
    /// </summary>
    public class EngineCore
    {
        #region Backend variables.

        private readonly EngineWorldClock? _worldClock;
        private readonly PessimisticCriticalResource<List<RenderLoopInvocation>> _renderLoopInvocations = new();
        private int _renderLoopInvocationCount = 0;

        #endregion

        #region Public properties.

        internal MpCommsManager? CommsManager { get; set; }

        public SiEngineExecutionMode ExecutionMode { get; private set; }
        public bool IsRunning { get; private set; } = false;
        public bool IsInitializing { get; private set; } = false;

        public ManagedLobby? MultiplayLobby { get; set; }

        #endregion

        #region Managers. 

        public InputManager Input { get; private set; }
        public DisplayManager Display { get; private set; }
        public SpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public AudioManager Audio { get; private set; }
        public AssetManager Assets { get; private set; }
        public DevelopmentManager? Development { get; private set; }
        public CollisionManager Collisions { get; private set; }

        #endregion

        #region Tick Controllers.

        public SituationTickController Situations { get; private set; }
        public EventTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }
        public MenuTickController Menus { get; private set; }
        public SiRendering Rendering { get; private set; }
        public SiEngineSettings Settings { get; private set; }

        #endregion

        #region Events.

        public delegate void InitializationEvent(EngineCore engine);
        public event InitializationEvent? OnInitializationComplete;

        public delegate void ShutdownEvent(EngineCore engine);
        public event ShutdownEvent? OnShutdown;

        #endregion

        #region Render-Loop Invocation.

        /// <summary>
        /// Executes code within the engine render loop. Safe for adding sprites, etc.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RenderLoopInvocation Invoke(Action action)
        {
            var invocation = new RenderLoopInvocation(this, action);
            _renderLoopInvocations.Use(o =>
            {
                o.Add(invocation);
                Interlocked.Increment(ref _renderLoopInvocationCount);
            });
            return invocation;
        }

        public void RemoveRenderLoopInvocation(RenderLoopInvocation invocation)
        {
            _renderLoopInvocations.Use(o =>
            {
                int count = o.RemoveAll(o => o.Id == invocation.Id);
                for (int i = 0; i < count; i++)
                {
                    Interlocked.Decrement(ref _renderLoopInvocationCount);
                }
            });
        }

        #endregion

        public void InitializeForMultiplayer()
        {
            CommsManager = new MpCommsManager(Settings.ServerAddress, Settings.ServerPort);
            CommsManager.AddHandler(new DatagramMessageHandler(this));
            CommsManager.AddHandler(new ReliableMessageHandler(this));
        }

        public void InitializeForSinglePlayer()
        {
            CommsManager?.Dispose();
            CommsManager = null;
        }

        /// <summary>
        /// Initializes a new instance of the game engine for shared engine content mode, which shares rendering and asset
        /// management with another instance of the engine (the "shared engine") that is running in shared engine content mode.
        /// 
        /// You see, the server can host multiple game instances for different lobbies, but we don't want to have multiple copies
        /// of the rendering and asset management code running on the server - that would be a waste of resources.
        /// So instead, we have one instance of the engine running in shared engine content mode that handles all of
        /// the rendering and asset management, and then each game instance runs in server host mode and shares the
        /// rendering and asset management of the shared engine.
        /// </summary>
        public EngineCore(SiEngineExecutionMode executionMode = SiEngineExecutionMode.SharedEngineContent)
        {
            ExecutionMode = executionMode;

            if (ExecutionMode != SiEngineExecutionMode.SharedEngineContent)
            {
                throw new Exception("This constructor is only meant for shared engine content mode.");
            }

            Settings = LoadSettings();

            var drawingSurface = new Control()
            {
                Height = 1080,
                Width = 1920
            };

            Display = new DisplayManager(this, drawingSurface);
            Rendering = new SiRendering(Settings, drawingSurface, Display.TotalCanvasSize);
            Assets = new AssetManager(this);
            Events = new EventTickController(this);
            Sprites = new SpriteManager(this);
            Input = new InputManager(this);
            Collisions = new CollisionManager(this);

            Situations = new SituationTickController(this);
            Audio = new AudioManager(this);
            Menus = new MenuTickController(this);
            Player = new PlayerSpriteTickController(this);

            //No clock for shared engine content mode.
            //_worldClock = new EngineWorldClock(this);
        }

        /// <summary>
        /// Initializes a new instance of the game engine for server host mode, which shares rendering and asset
        /// management with another instance of the engine (the "shared engine") that is running in shared engine content mode.
        /// </summary>
        public EngineCore(ManagedLobby lobby, EngineCore sharedEngine, SiEngineExecutionMode executionMode)
        {
            MultiplayLobby = lobby;
            ExecutionMode = executionMode;

            if (ExecutionMode != SiEngineExecutionMode.ServerHost)
            {
                throw new Exception("This constructor is only meant for server host mode.");
            }

            Settings = LoadSettings();

            var drawingSurface = new Control()
            {
                Height = 1080,
                Width = 1920
            };

            Display = sharedEngine.Display;
            Rendering = sharedEngine.Rendering;
            Assets = sharedEngine.Assets;

            Events = new EventTickController(this);
            Sprites = new SpriteManager(this);
            Input = new InputManager(this);
            Collisions = new CollisionManager(this);

            Situations = new SituationTickController(this);
            Audio = new AudioManager(this);
            Menus = new MenuTickController(this);
            Player = new PlayerSpriteTickController(this);

            _worldClock = new EngineWorldClock(this);
        }

        /// <summary>
        /// Initializes a new instance of the game engine.
        /// </summary>
        /// <param name="drawingSurface">The window that the game will be rendered to.</param>
        public EngineCore(Control drawingSurface, SiEngineExecutionMode executionMode, Size? sizeOverride = null)
        {
            ExecutionMode = executionMode;

            if (ExecutionMode != SiEngineExecutionMode.Play
                && ExecutionMode != SiEngineExecutionMode.Edit)
            {
                throw new Exception("This constructor is only meant for play and edit modes.");
            }

            Settings = LoadSettings();

            Display = new DisplayManager(this, drawingSurface, sizeOverride);
            Rendering = new SiRendering(Settings, drawingSurface, Display.TotalCanvasSize);
            Assets = new AssetManager(this);
            Events = new EventTickController(this);
            Sprites = new SpriteManager(this);
            Input = new InputManager(this);
            Collisions = new CollisionManager(this);

            Situations = new SituationTickController(this);
            Audio = new AudioManager(this);
            Menus = new MenuTickController(this);
            Player = new PlayerSpriteTickController(this);

            _worldClock = new EngineWorldClock(this);
        }

        public void EnableDevelopment(IInterrogationForm interrogationForm)
        {
            Development = new DevelopmentManager(this, interrogationForm);
        }

        public static SiEngineSettings LoadSettings()
        {
            var settings = LocalUserApplicationData.LoadFromDisk<SiEngineSettings>(SiConstants.FriendlyName);

            if (settings == null)
            {
                settings = new SiEngineSettings();

                int x = 1024;
                int y = 768;

                if (Screen.PrimaryScreen != null)
                {
                    x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
                    y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                    if (x % 2 != 0) x++;
                    if (y % 2 != 0) y++;
                }

                settings.Resolution = new Size(x, y);

                LocalUserApplicationData.SaveToDisk(SiConstants.FriendlyName, settings);
            }

            return settings;
        }

        public static void SaveSettings(SiEngineSettings settings)
        {
            LocalUserApplicationData.SaveToDisk(SiConstants.FriendlyName, settings);
        }

        public void ResetGame()
        {
            Sprites.TextBlocks.PlayerStatsText.IsVisible = false;
            Situations.End();
            Sprites.QueueDeletionOfActionSprites();
        }

        public void StartGame()
        {
            Sprites.QueueDeletionOfActionSprites();
            Situations.AdvanceLevel();
        }

        public void RenderEverything(float epoch)
        {
            try
            {
                Rendering.RenderTargets.Use((o =>
                {
                    if (o.ScreenRenderTarget != null && o.IntermediateRenderTarget != null)
                    {
                        o.IntermediateRenderTarget.BeginDraw();

                        o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Red);

                        if (ExecutionMode == SiEngineExecutionMode.Play)
                        {
                            o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Black);
                        }
                        else
                        {
                            o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.EditorBackground);
                        }

                        Sprites.RenderPreScaling(o.IntermediateRenderTarget, epoch);

                        //Render-Loop invocations are not meant to be performant. They are meant for one-off tasks that need to
                        //  be done in the render loop - which is why we attempt to optimize them out with _renderLoopInvocationCount.
                        if (_renderLoopInvocationCount > 0)
                        {
                            var invocationsToExecute = new List<RenderLoopInvocation>();
                            _renderLoopInvocations.Use(o => invocationsToExecute.AddRange(o));
                            foreach (var invocation in invocationsToExecute)
                            {
                                invocation.Execute();
                            }
                        }

                        #region Render Collisions.

                        if (Settings.HighlightCollisions)
                        {
                            foreach (var collision in Collisions.Detected)
                            {
                                Rendering.DrawRectangle(o.IntermediateRenderTarget,
                                    -Display.CameraPosition.X, -Display.CameraPosition.Y,
                                    collision.Value.OverlapRectangle.ToRawRectangleF(),
                                    Rendering.Materials.Colors.Orange, 1, 2, 0);

                                Rendering.DrawPolygon(o.IntermediateRenderTarget, -Display.CameraPosition.X, -Display.CameraPosition.Y,
                                    collision.Value.OverlapPolygon,
                                    Rendering.Materials.Colors.Cyan, 3);

                                Rendering.DrawRectangle(o.IntermediateRenderTarget,
                                    collision.Value.Body1.RawRenderBounds,
                                    Rendering.Materials.Colors.Red, 1, 1, collision.Value.Body1.PredictedDirection.RadiansSigned);

                                Rendering.DrawRectangle(o.IntermediateRenderTarget,
                                    collision.Value.Body2.RawRenderBounds,
                                    Rendering.Materials.Colors.LawnGreen, 1, 1, collision.Value.Body2.PredictedDirection.RadiansSigned);
                            }
                        }
                        #endregion

                        o.IntermediateRenderTarget.EndDraw();

                        o.ScreenRenderTarget.BeginDraw();

                        if (Display.ZoomOverride != null)
                        {
                            Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, Display.ZoomOverride.Value);
                        }
                        else if (Settings.EnableSpeedScaleFactoring)
                        {
                            Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, (float)Display.SpeedOrientedFrameScalingFactor());
                        }
                        else
                        {
                            Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, (float)Display.BaseDrawScale);
                        }

                        Sprites.RenderPostScaling(o.ScreenRenderTarget, epoch);


                        o.ScreenRenderTarget.EndDraw();
                    }
                }));
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
            //Sprites.ResetPlayer();

            if (ExecutionMode == SiEngineExecutionMode.Play
                || ExecutionMode == SiEngineExecutionMode.Edit
                || ExecutionMode == SiEngineExecutionMode.ServerHost)
            {
                _worldClock?.Start();
            }

            if (ExecutionMode == SiEngineExecutionMode.Play)
            {
                var loadingHeader = Sprites.TextBlocks.Add(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Red, new SiVector(100, 100), true);

                var loadingDetail = Sprites.TextBlocks.Add(Rendering.TextFormats.Loading,
                    Rendering.Materials.Brushes.OrangeRed, new SiVector(loadingHeader.X, loadingHeader.Y + 50), true);

                IsInitializing = true;

                HydrateCache(loadingHeader, loadingDetail);

                loadingHeader.QueueForDelete();
                loadingDetail.QueueForDelete();
            }
            else if (ExecutionMode == SiEngineExecutionMode.SharedEngineContent)
            {
                HydrateCache();
            }
            else if (ExecutionMode == SiEngineExecutionMode.Edit)
            {
                HydrateCache();
            }

            OnInitializationComplete?.Invoke(this);

            IsInitializing = false;

            if (ExecutionMode == SiEngineExecutionMode.Play)
            {
                //Add initial stars.
                for (int i = 0; i < Settings.InitialFrameStarCount; i++)
                {
                    Sprites.Stars.AddRandomStarAt(Display.RandomOnScreenLocation());
                }

                if (Settings.PlayMusic)
                {
                    Audio.BackgroundMusicSound?.Play();
                }

                //TODO: Get the random skybox sprite.
                //Sprites.SkyBoxes.AddAtCenterUniverse();

                //Events.Add(1, () => AddDemoSprites());
                Events.Once(() => Menus.Show(new MenuStartNewGame(this)));
            }
        }

        void AddDemoSprites()
        {
            /*
            for (int i = 0; i < 5; i++)
                ApplySpriteStates(Sprites.Enemies.AddTypeOf<SpriteEnemyMerc>());

            for (int i = 0; i < 5; i++)
                ApplySpriteStates(Sprites.Enemies.AddTypeOf<SpriteEnemyMinnow>());

            for (int i = 0; i < 5; i++)
                ApplySpriteStates(Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>());

            for (int i = 0; i < 5; i++)
                ApplySpriteStates(Sprites.Enemies.AddTypeOf<SpriteEnemyScav>());

            for (int i = 0; i < 5; i++)
                ApplySpriteStates(Sprites.Enemies.AddTypeOf<SpriteEnemySerf>());
            */

            //for (int i = 0; i < 3; i++)
            //ApplySpriteStates(Sprites.Enemies.AddTypeOf<SpriteEnemyBossDevastator>());

            //void ApplySpriteStates(SpriteEnemyBase sprite)
            //{
            //    sprite.ClearAIControllers();
            //    sprite.Location = Display.RandomOnScreenLocation();
            //    sprite.Orientation = SiVector.FromUnsignedDegrees(sprite.Location.AngleToInUnsignedDegrees(Player.Sprite.Location) + SiRandom.Variance(360, 0.15f));
            //    sprite.AddAIController(new AILogisticsDemo(this, sprite));
            //    sprite.SetCurrentAIController<AILogisticsDemo>();
            //}
        }

        private void HydrateCache(SpriteTextBlock? loadingHeader = null, SpriteTextBlock? loadingDetail = null)
        {
            loadingHeader?.SetTextAndCenterX("Loading assets...");
            loadingHeader?.SetTextAndCenterX("Loading reflection cache...");

            SiReflection.BuildReflectionCacheOfType<SpriteBase>();
            SiReflection.BuildReflectionCacheOfType<AIStateMachine>();

            Assets.LoadAllAssets(loadingHeader, loadingDetail);
        }

        public void ShutdownEngine()
        {
            if (IsRunning)
            {
                IsRunning = false;

                CommsManager?.Dispose();
                CommsManager = null;

                OnShutdown?.Invoke(this);

                _worldClock?.Dispose();
                Sprites.Dispose();
                Rendering.Dispose();
            }
        }

        public bool IsPaused() => _worldClock?.IsPaused() == true;
        public void TogglePause() => _worldClock?.TogglePause();
        public void Pause() => _worldClock?.Pause();
        public void Resume() => _worldClock?.Resume();
    }
}
