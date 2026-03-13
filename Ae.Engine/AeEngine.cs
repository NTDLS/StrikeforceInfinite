using Ae.Engine.AI;
using Ae.Engine.DataModels;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Interrogation._Superclass;
using Ae.Engine.Manager;
using Ae.Engine.Menu;
using Ae.Engine.MultiPlay;
using Ae.Engine.Rendering;
using Ae.Engine.Sprite.Base;
using Ae.Engine.TickController.PlayerSpriteTickController;
using Ae.Engine.TickController.UnvectoredTickController;
using Ae.Engine.Types;
using Ae.MpClientToServerComms;
using NTDLS.Persistence;
using NTDLS.Semaphore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Ae.Engine
{
    /// <summary>
    /// The core game engine. Contained the controllers and managers.
    /// </summary>
    public class AeEngine
    {
        #region Backend variables.

        private readonly EngineWorldClock? _worldClock;
        private readonly PessimisticCriticalResource<List<AeRenderLoopInvocation>> _renderLoopInvocations = new();
        private int _renderLoopInvocationCount = 0;

        #endregion

        #region Public properties.

        /// <summary>
        /// Specifies the relative path to the asset package database used during debugging builds.
        /// </summary>
        /// <remarks>This constant is only available in debug configurations. Use this path to locate the
        /// asset package file when running or testing the application in a development environment.</remarks>
#if DEBUG
        public string AssetPackagePath { get; set; } = "../../../../@Installer/Ae.Assets.db";
#else
        public string AssetPackagePath { get; set; } = "./Ae.Assets.db";
#endif

        internal MpCommsManager? CommsManager { get; set; }

        internal AeEngineExecutionMode ExecutionMode { get; private set; }
        internal bool IsRunning { get; private set; } = false;
        internal bool IsInitializing { get; private set; } = false;

        #endregion

        #region Managers. 

        /// <summary>
        /// Gets or sets the managed lobby instance used for multiplayer sessions.
        /// </summary>
        public ManagedLobby? MultiplayLobby { get; set; }

        /// <summary>
        /// Input manager responsible for keyboard, mouse and controller interactions.
        /// </summary>
        public InputManager Input { get; private set; }
        /// <summary>
        /// Display manager responsible for camera position, zoom level, and other display-related properties.
        /// </summary>
        public DisplayManager Display { get; private set; }
        /// <summary>
        /// Sprite manager responsible for managing all sprites in the game, including their creation, deletion, and updates.
        /// </summary>
        /// <remarks>The returned SpriteManager provides access to all sprites and their associated tick
        /// controllers. Use this property to add, remove, or update sprites as needed.</remarks>
        public SpriteManager Sprites { get; private set; }
        /// <summary>
        /// Audio manager responsible for audio playback and settings.
        /// </summary>
        public AudioManager Audio { get; private set; }
        /// <summary>
        /// Asset manager responsible for loading and providing access to game assets such as images, sounds, and other resources.
        /// </summary>
        public AssetManager Assets { get; private set; }
        /// <summary>
        /// Development manager responsible for development tools and features, such as the interrogation form and other debugging utilities.
        /// This manager is only initialized if development mode is enabled. If development mode is not enabled, this property will be null.
        /// </summary>
        public DevelopmentManager? Development { get; private set; }
        /// <summary>
        /// Collision manager is responsible for detecting and handling collisions.
        /// </summary>
        public CollisionManager Collisions { get; private set; }

        #endregion

        #region Tick Controllers.

        /// <summary>
        /// Situation tick controller responsible for managing the current situation and advancing the game state based on the current situation.
        /// </summary>
        public SituationTickController Situations { get; private set; }
        /// <summary>
        /// Events tick controller responsible for managing timed events in the game, such as spawning enemies, triggering cutscenes, and other time-based actions.
        /// </summary>
        public EventTickController Events { get; private set; }
        /// <summary>
        /// Player tick controller responsible for managing the player sprite and its associated properties, such as health, speed, and other player-related attributes. This tick controller is separate from the main sprite manager to allow for more specialized handling of the player sprite and its unique properties.
        /// </summary>
        public PlayerSpriteTickController Player { get; private set; }
        /// <summary>
        /// Menu tick controller responsible for managing in-game menus, such as the main menu, pause menu, and other UI elements that require regular updates. This controller handles the logic for displaying, hiding, and updating menu items based on user interactions and game state changes.
        /// </summary>
        public MenuTickController Menus { get; private set; }
        /// <summary>
        /// Rendering manager responsible for managing the rendering of sprites and other visual elements in the game. This controller handles the logic for drawing sprites to the screen, applying visual effects, and managing the rendering order of different elements. It works closely with the DisplayManager to ensure that sprites are rendered correctly based on the current camera position, zoom level, and other display-related properties.
        /// </summary>
        public AeRendering Rendering { get; private set; }
        /// <summary>
        /// Settings manager responsible for managing game settings, such as resolution, audio levels, and other configurable options. This manager provides access to the current settings and allows for updating settings as needed. It also handles the loading and saving of settings to disk, ensuring that user preferences are preserved across game sessions.
        /// </summary>
        public AeEngineSettings Settings { get; private set; }

        #endregion

        #region Events.

        /// <summary>
        /// Represents a delegate for handling initialization events of the engine.
        /// </summary>
        /// <param name="engine">The engine instance that is being initialized. Provides access to the engine's state and configuration
        /// during initialization.</param>
        public delegate void InitializationEvent(AeEngine engine);
        /// <summary>
        /// Occurs when the initialization process has completed.
        /// </summary>
        /// <remarks>Subscribers can use this event to perform actions after initialization is finished.
        /// The event is raised once initialization is complete, and may provide relevant initialization details through
        /// its event arguments.</remarks>
        public event InitializationEvent? OnInitializationComplete;

        /// <summary>
        /// Represents a method that handles shutdown events for the specified engine.
        /// </summary>
        /// <remarks>Use this delegate to subscribe to shutdown notifications and perform cleanup or
        /// finalization tasks when the engine is shutting down.</remarks>
        /// <param name="engine">The engine instance for which the shutdown event is triggered. Cannot be null.</param>
        public delegate void ShutdownEvent(AeEngine engine);
        /// <summary>
        /// Occurs when a shutdown operation is initiated, allowing subscribers to respond to the shutdown event.
        /// </summary>
        /// <remarks>Subscribers can use this event to perform cleanup or save state before the
        /// application shuts down. The event may not be raised if shutdown is not triggered through the expected
        /// mechanism.</remarks>
        public event ShutdownEvent? OnShutdown;

        #endregion

        #region Render-Loop Invocation.

        /// <summary>
        /// Executes code within the engine render loop. Safe for adding sprites, etc.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public AeRenderLoopInvocation Invoke(Action action)
        {
            var invocation = new AeRenderLoopInvocation(this, action);
            _renderLoopInvocations.Use(o =>
            {
                o.Add(invocation);
                Interlocked.Increment(ref _renderLoopInvocationCount);
            });
            return invocation;
        }

        internal void RemoveRenderLoopInvocation(AeRenderLoopInvocation invocation)
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

        internal void InitializeForMultiplayer()
        {
            CommsManager = new MpCommsManager(Settings.ServerAddress, Settings.ServerPort);
            CommsManager.AddHandler(new DatagramMessageHandler(this));
            CommsManager.AddHandler(new ReliableMessageHandler(this));
        }

        internal void InitializeForSinglePlayer()
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
        public AeEngine(AeEngineExecutionMode executionMode = AeEngineExecutionMode.SharedEngineContent, string? assetPackagePath = null)
        {
            ExecutionMode = executionMode;

            if (ExecutionMode != AeEngineExecutionMode.SharedEngineContent)
            {
                throw new Exception("This constructor is only meant for shared engine content mode.");
            }

            if (string.IsNullOrEmpty(assetPackagePath) == false)
            {
                AssetPackagePath = assetPackagePath;
            }

            Settings = LoadSettings();

            var drawingSurface = new Control()
            {
                Height = 1080,
                Width = 1920
            };

            Display = new DisplayManager(this, drawingSurface);
            Rendering = new AeRendering(Settings, drawingSurface, Display.TotalCanvasSize);
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
        public AeEngine(ManagedLobby lobby, AeEngine sharedEngine, AeEngineExecutionMode executionMode, string? assetPackagePath = null)
        {
            MultiplayLobby = lobby;
            ExecutionMode = executionMode;

            if (ExecutionMode != AeEngineExecutionMode.ServerHost)
            {
                throw new Exception("This constructor is only meant for server host mode.");
            }

            if (string.IsNullOrEmpty(assetPackagePath) == false)
            {
                AssetPackagePath = assetPackagePath;
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
        /// Initializes a new instance of the AeEngine class for play or edit mode, configuring core subsystems and
        /// rendering on the specified drawing surface.
        /// </summary>
        /// <remarks>This constructor sets up essential engine managers and subsystems required for
        /// interactive or editable sessions. Use this overload when initializing the engine for gameplay or editing
        /// scenarios.</remarks>
        /// <param name="drawingSurface">The control used as the drawing surface for rendering engine output. Must not be null.</param>
        /// <param name="executionMode">The execution mode for the engine. Must be either Play or Edit.</param>
        /// <param name="sizeOverride">An optional size override for the drawing surface. If specified, determines the canvas size used for rendering.</param>
        /// <param name="assetPackagePath">An optional override for the assets path.</param>
        /// <exception cref="Exception">Thrown if executionMode is not Play or Edit.</exception>
        public AeEngine(Control drawingSurface, AeEngineExecutionMode executionMode, Size? sizeOverride = null, string? assetPackagePath = null)
        {
            ExecutionMode = executionMode;

            if (ExecutionMode != AeEngineExecutionMode.Play
                && ExecutionMode != AeEngineExecutionMode.Edit
                && ExecutionMode != AeEngineExecutionMode.AttachedDebugging)
            {
                throw new Exception("This constructor is only meant for play and edit modes.");
            }

            if (string.IsNullOrEmpty(assetPackagePath) == false)
            {
                AssetPackagePath = assetPackagePath;
            }

            Settings = LoadSettings();

            Display = new DisplayManager(this, drawingSurface, sizeOverride);
            Rendering = new AeRendering(Settings, drawingSurface, Display.TotalCanvasSize);
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

        /// <summary>
        /// Enables development mode by initializing the development manager with the specified interrogation form.
        /// </summary>
        /// <remarks>Call this method to activate development features for the current instance.
        /// Subsequent operations may behave differently when development mode is enabled.</remarks>
        /// <param name="interrogationForm">The interrogation form used to configure development mode. Cannot be null.</param>
        public void EnableDevelopment(IInterrogationForm interrogationForm)
        {
            Development = new DevelopmentManager(this, interrogationForm);
        }

        /// <summary>
        /// Loads the engine settings from local user application data, creating and saving default settings if none
        /// exist.
        /// </summary>
        /// <remarks>If no settings are found on disk, default settings are created with a resolution
        /// based on the primary screen size and saved for future use.</remarks>
        /// <returns>An instance of AeEngineSettings containing the loaded or newly created settings.</returns>
        public static AeEngineSettings LoadSettings()
        {
            var settings = LocalUserApplicationData.LoadFromDisk<AeEngineSettings>(AeConstants.FriendlyName);

            if (settings == null)
            {
                settings = new AeEngineSettings();

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

                LocalUserApplicationData.SaveToDisk(AeConstants.FriendlyName, settings);
            }

            return settings;
        }

        /// <summary>
        /// Saves the specified engine settings to the local user application data store.
        /// </summary>
        /// <param name="settings">The engine settings to be persisted. Cannot be null.</param>
        public static void SaveSettings(AeEngineSettings settings)
        {
            LocalUserApplicationData.SaveToDisk(AeConstants.FriendlyName, settings);
        }

        /// <summary>
        /// Resets the game state by clearing player statistics, ending the current situation, and removing action
        /// sprites.
        /// </summary>
        /// <remarks>Call this method to return the game to its initial state after a session or when
        /// starting a new game. This method affects visible UI elements and game logic, and should be used when a full
        /// reset is required.</remarks>
        public void ResetGame()
        {
            Sprites.TextBlocks.PlayerStatsText.IsVisible = false;
            Situations.End();
            Sprites.QueueDeletionOfActionSprites();
        }

        /// <summary>
        /// Initializes a new game session by resetting relevant game state and advancing to the next level.
        /// </summary>
        /// <remarks>Call this method to begin gameplay or restart the game. This method updates the game
        /// state and prepares the environment for player interaction. Subsequent calls will reinitialize the session
        /// and may affect ongoing progress.</remarks>
        public void StartGame()
        {
            Sprites.QueueDeletionOfActionSprites();
            Situations.AdvanceLevel();
        }

        internal void RenderEverything(float epoch)
        {
            try
            {
                Rendering.RenderTargets.Use((o =>
                {
                    if (o.ScreenRenderTarget != null && o.IntermediateRenderTarget != null)
                    {
                        o.IntermediateRenderTarget.BeginDraw();

                        o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Red);

                        if (ExecutionMode == AeEngineExecutionMode.Play || ExecutionMode == AeEngineExecutionMode.AttachedDebugging)
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
                            var invocationsToExecute = new List<AeRenderLoopInvocation>();
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

        /// <summary>
        /// Starts the game engine and initializes the runtime environment based on the current execution mode.
        /// </summary>
        /// <remarks>Initialization behavior varies depending on the execution mode. In 'Play' mode,
        /// additional setup such as adding initial stars and starting background music is performed. The method is not
        /// thread-safe; concurrent calls may result in unexpected behavior.</remarks>
        /// <param name="progressCallback">An optional callback that receives progress updates during initialization. The first parameter is a message
        /// describing the current step, and the second parameter is a value between 0 and 1 indicating progress.</param>
        /// <param name="writeLog">An optional delegate used to log messages during initialization. If provided, log entries will be sent to
        /// this delegate.</param>
        /// <exception cref="Exception">Thrown if the game engine is already running.</exception>
        public void StartEngine(Action<string, float>? progressCallback = null, WriteLogDelegate? writeLog = null)
        {
            if (IsRunning)
            {
                throw new Exception("The game engine is already running.");
            }

            IsRunning = true;
            //Sprites.ResetPlayer();

            if (ExecutionMode == AeEngineExecutionMode.Play
                || ExecutionMode == AeEngineExecutionMode.Edit
                || ExecutionMode == AeEngineExecutionMode.ServerHost
                || ExecutionMode == AeEngineExecutionMode.AttachedDebugging)
            {
                _worldClock?.Start();
            }

            IsInitializing = true;

            HydrateCache(progressCallback, writeLog);

            OnInitializationComplete?.Invoke(this);

            IsInitializing = false;

            if (ExecutionMode == AeEngineExecutionMode.Play
                || ExecutionMode == AeEngineExecutionMode.AttachedDebugging)
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
                Events.Once(() => Menus.Show(new AeMenuStartNewGame(this)));
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

        private void HydrateCache(Action<string, float>? progressCallback, WriteLogDelegate? writeLog = null)
        {
            progressCallback?.Invoke("Hydrating sprites...", 0);
            AeReflection.BuildReflectionCacheOfType<AeSprite>(progressCallback, writeLog);
            progressCallback?.Invoke("Hydrating AI machines...", 0);
            AeReflection.BuildReflectionCacheOfType<AeAIStateMachine>(progressCallback, writeLog);

            Assets.LoadAllAssets(progressCallback, writeLog);
        }

        /// <summary>
        /// Shuts down the engine and releases associated resources.
        /// </summary>
        /// <remarks>Call this method to stop the engine and dispose of communication, rendering, and
        /// sprite resources. After calling this method, the engine cannot be restarted without reinitialization. This
        /// method is safe to call multiple times; subsequent calls have no effect if the engine is already
        /// stopped.</remarks>
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

        /// <summary>
        /// Determines whether the world clock is currently paused.
        /// </summary>
        /// <returns>A value indicating whether the world clock is paused. Returns <see langword="true"/> if the world clock is
        /// paused; otherwise, <see langword="false"/>.</returns>
        public bool IsPaused() => _worldClock?.IsPaused() == true;

        /// <summary>
        /// Toggles the paused state of the world clock, pausing it if running or resuming it if paused.
        /// </summary>
        /// <remarks>Calling this method has no effect if the world clock is not initialized. Use this
        /// method to control time progression in scenarios such as simulation or game loops.</remarks>
        public void TogglePause() => _worldClock?.TogglePause();

        /// <summary>
        /// Pauses the world clock, temporarily halting its progression.
        /// </summary>
        /// <remarks>Call this method to suspend time updates managed by the world clock. To resume, use
        /// the corresponding resume method if available. This operation has no effect if the world clock is already
        /// paused or not initialized.</remarks>
        public void Pause() => _worldClock?.Pause();

        /// <summary>
        /// Resumes the world clock if it is currently paused.
        /// </summary>
        /// <remarks>Call this method to continue time progression after it has been paused. If the world
        /// clock is not paused, calling this method has no effect.</remarks>
        public void Resume() => _worldClock?.Resume();
    }
}
