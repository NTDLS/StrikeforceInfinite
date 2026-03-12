using Ae.Engine.Mathematics;
using System;
using System.Drawing;

namespace Ae.Engine.DataModels
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    public class AeEngineSettings
    {
        /// <summary>
        /// Server Address.
        /// </summary>
        public string ServerAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Server port.
        /// </summary>
        public int ServerPort { get; set; } = 42719;

        /// <summary>
        /// Gets or sets the identifier of the graphics adapter to use for rendering operations.
        /// </summary>
        public int GraphicsAdapterId { get; set; } = 0;
        /// <summary>
        /// Gets or sets the number of threads used for munition traversal operations.
        /// </summary>
        /// <remarks>Increasing the number of threads may improve performance on systems with multiple
        /// processor cores. The value should be chosen based on available hardware and workload requirements.</remarks>
        public int MunitionTraversalThreads { get; set; } = AeMath.LesserOf(Environment.ProcessorCount * 2, 16);
        /// <summary>
        /// Gets or sets the number of threads allocated for world clock operations.
        /// </summary>
        public int WorldClockThreads { get; set; } = 10;
        /// <summary>
        /// Gets or sets a value indicating whether sprite interrogation features are enabled.
        /// </summary>
        public bool EnableSpriteInterrogation { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether developer mode features are enabled.
        /// </summary>
        /// <remarks>When enabled, additional diagnostic information and debugging tools may be available.
        /// Use caution in production environments, as developer mode can expose sensitive information or affect
        /// performance.</remarks>
        public bool EnableDeveloperMode { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether natural bounds should be visually highlighted.
        /// </summary>
        public bool HighlightNaturalBounds { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether all sprites should be highlighted.
        /// </summary>
        public bool HighlightAllSprites { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether collision areas should be visually highlighted.
        /// </summary>
        public bool HighlightCollisions { get; set; } = false;

        /// <summary>
        /// Gets or sets the resolution of the image or display area.
        /// </summary>
        /// <remarks>The resolution is represented as a <see cref="Size"/> structure, specifying the width
        /// and height in pixels. Changing this property may affect rendering quality and performance depending on the
        /// context in which it is used.</remarks>
        public Size Resolution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the world clock thread should run with elevated priority.
        /// </summary>
        /// <remarks>Set this property to <see langword="true"/> to increase the scheduling priority of
        /// the world clock thread, which may improve timing accuracy but can impact overall application
        /// performance.</remarks>
        public bool ElevatedWorldClockThreadPriority { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether the world clock operates in multithreaded mode.
        /// </summary>
        /// <remarks>When enabled, the world clock may perform operations concurrently to improve
        /// performance. Disable this option if thread safety is not required or if single-threaded operation is
        /// preferred.</remarks>
        public bool MultithreadedWorldClock { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the display is in full screen mode.
        /// </summary>
        public bool FullScreen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether background music is enabled during playback.
        /// </summary>
        public bool PlayMusic { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether speed scale factoring is enabled.
        /// </summary>
        public bool EnableSpeedScaleFactoring { get; set; } = true;

        /// <summary>
        /// Gets or sets the rate at which the player's velocity increases over time.
        /// </summary>
        public float PlayerVelocityRampUp { get; set; } = 3.75f;
        /// <summary>
        /// Gets or sets the factor used to reduce the player's velocity over time.
        /// </summary>
        public float PlayerVelocityRampDown { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets the maximum hull health value for the entity.
        /// </summary>
        public int MaxHullHealth { get; set; } = 100000;
        /// <summary>
        /// Gets or sets the maximum shield health value allowed for the entity.
        /// </summary>
        public int MaxShieldHealth { get; set; } = 100000;

        /// <summary>
        /// Gets or sets the maximum boost amount that a player can receive.
        /// </summary>
        public float MaxPlayerBoostAmount { get; set; } = 10000;
        /// <summary>
        /// Gets or sets the maximum rotation speed, in degrees per second, allowed for the player.
        /// </summary>
        public float MaxPlayerRotationSpeedDegrees { get; set; } = 140.0f;

        /// <summary>
        /// Gets or sets the number of stars to display in the initial frame.
        /// </summary>
        public int InitialFrameStarCount { get; set; } = 25;
        /// <summary>
        /// Gets or sets the target number of stars to detect in a delta frame analysis.
        /// </summary>
        public int DeltaFrameTargetStarCount { get; set; } = 200;

        /// <summary>
        /// After the frame has been generated, if it takes less time than the framerate - yield the time instead of rending the next frame too early.
        /// this is really just an effort to keep epoch time reasonably close to frame time.
        /// </summary>
        public bool YieldRemainingFrameTime { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether vertical synchronization (VSync) is enabled for rendering.
        /// </summary>
        /// <remarks>Enabling vertical synchronization can help prevent screen tearing by synchronizing
        /// the frame rate with the display's refresh rate. Disabling VSync may result in higher frame rates but can
        /// cause visual artifacts.</remarks>
        public bool VerticalSync { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether anti-aliasing is enabled for rendering.
        /// </summary>
        /// <remarks>Enabling anti-aliasing can improve visual quality by smoothing jagged edges in
        /// graphics. Disabling it may increase rendering performance at the cost of image quality.</remarks>
        public bool AntiAliasing { get; set; } = true;

        /// <summary>
        /// Ensure that the average framerate is within sane limits. This is especially important for vSync since we want to make sure a frame is available for the GPU.
        /// </summary>
        public bool FineTuneFramerate { get; set; } = true;
        /// <summary>
        /// Gets or sets the target frame rate for rendering operations.
        /// </summary>
        /// <remarks>Adjusting this value allows control over the maximum number of frames rendered per
        /// second. Setting a higher frame rate may improve visual smoothness but can increase resource usage.</remarks>
        public float TargetFrameRate { get; set; } = 70;

        /// <summary>
        /// Gets or sets the maximum distance, in units, that a munition can travel from the scene before it is removed.
        /// </summary>
        public float MunitionSceneDistanceLimit { get; set; } = 2500;

        /// <summary>
        /// How much larger than the screen (NaturalScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public float OverdrawScale { get; set; } = 1.5f;

        /// <summary>
        /// Introduces a delay between frames for debugging purposes.
        /// 0 = disabled.
        /// </summary>
        public int DebugThrottleMs { get; set; } = 0;
    }
}
