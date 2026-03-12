using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.TextBlock;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Linq;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls and manages the tick-based updates for text block sprites within the game world, including player
    /// statistics, debug information, and pause state indicators.
    /// </summary>
    /// <remarks>This controller provides centralized management for various text block sprites, ensuring
    /// their positions and visibility are updated appropriately during world clock ticks. It facilitates the creation
    /// and insertion of specialized text blocks, such as radar position indicators and pause notifications. Use this
    /// class to coordinate text display elements that require synchronization with the game’s tick cycle.</remarks>
    public class TextBlocksSpriteTickController
        : VectoredTickControllerBase<AeSpriteTextBlock>
    {
        /// <summary>
        /// Gets the text block displaying the player's statistics.
        /// </summary>
        public AeSpriteTextBlock PlayerStatsText { get; private set; }

        /// <summary>
        /// Gets the text block used for displaying debug information.
        /// </summary>
        public AeSpriteTextBlock DebugText { get; private set; }

        /// <summary>
        /// Gets the text block displayed when the sprite is paused.
        /// </summary>
        public AeSpriteTextBlock PausedText { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TextBlocksSpriteTickController class, managing text block sprites for
        /// player statistics, debug information, and pause state display.
        /// </summary>
        /// <remarks>This constructor pre-creates text block sprites for player statistics, debug
        /// information, and pause state to ensure they are available even when the game is paused. Visibility of these
        /// sprites is managed based on game state.</remarks>
        /// <param name="engine">The engine instance used to access rendering formats, materials, and coordinate system for sprite creation.</param>
        /// <param name="manager">The sprite manager responsible for handling sprite lifecycle and events.</param>
        public TextBlocksSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
            PlayerStatsText = Add(engine.Rendering.TextFormats.RealtimePlayerStats, engine.Rendering.Materials.Brushes.WhiteSmoke, new AeVector(5, 5), true);
            PlayerStatsText.IsVisible = false;
            DebugText = Add(engine.Rendering.TextFormats.RealtimePlayerStats, engine.Rendering.Materials.Brushes.Cyan, new AeVector(5, PlayerStatsText.Y + 100), true);

            //We have to create this ahead of time because we cant create pause text when paused since sprites are created via events.
            PausedText = Add(engine.Rendering.TextFormats.LargeBlocker,
                    engine.Rendering.Materials.Brushes.Red, new AeVector(100, 100), true, "PausedText", "Paused");

            PausedText.IsVisible = false;
        }

        /// <summary>
        /// Updates the positions of all visible, non-fixed text blocks based on the current epoch and camera
        /// displacement.
        /// </summary>
        /// <param name="epoch">The current time value, typically representing the simulation or world clock, used to calculate motion.</param>
        /// <param name="cameraDisplacement">The vector representing the camera's movement since the last tick, used to adjust text block positions.</param>
        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(epoch, cameraDisplacement);
            }
        }

        #region Factories.

        /// <summary>
        /// Creates and inserts a new radar position text block sprite at the specified location.
        /// </summary>
        /// <param name="format">The text format to apply to the radar position text block. Cannot be null.</param>
        /// <param name="color">The brush used to render the text color. Cannot be null.</param>
        /// <param name="location">The location where the radar position text block will be placed.</param>
        /// <returns>A new instance of AeSpriteRadarPositionTextBlock representing the radar position text block at the specified
        /// location.</returns>
        public AeSpriteRadarPositionTextBlock CreateRadarPosition(TextFormat format, SolidColorBrush color, AeVector location)
        {
            var obj = new AeSpriteRadarPositionTextBlock(Engine, format, color, location);
            SpriteManager.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Creates and adds a new sprite text block to the sprite manager at the specified location.
        /// </summary>
        /// <param name="format">The text format to apply to the sprite text block. Determines font, size, and style.</param>
        /// <param name="color">The brush used to render the text color of the sprite text block. Cannot be null.</param>
        /// <param name="location">The position where the sprite text block will be placed. Specifies coordinates in the scene.</param>
        /// <param name="isPositionStatic">A value indicating whether the sprite text block's position remains fixed (<see langword="true"/>) or can
        /// change (<see langword="false"/>).</param>
        /// <returns>An instance of AeSpriteTextBlock representing the newly added sprite text block.</returns>
        public AeSpriteTextBlock Add(TextFormat format, SolidColorBrush color, AeVector location, bool isPositionStatic)
        {
            var obj = new AeSpriteTextBlock(Engine, format, color, location, isPositionStatic);
            SpriteManager.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Creates and adds a new sprite text block to the sprite manager with the specified formatting, color,
        /// location, and tag name.
        /// </summary>
        /// <param name="format">The text formatting to apply to the sprite text block. Cannot be null.</param>
        /// <param name="color">The brush used to render the text color. Cannot be null.</param>
        /// <param name="location">The position where the sprite text block will be placed.</param>
        /// <param name="isPositionStatic">A value indicating whether the sprite text block's position remains fixed (<see langword="true"/>) or can
        /// change (<see langword="false"/>).</param>
        /// <param name="name">The tag name assigned to the sprite text block. Used for identification and retrieval.</param>
        /// <returns>The newly created <see cref="AeSpriteTextBlock"/> instance that was added to the sprite manager.</returns>
        public AeSpriteTextBlock Add(TextFormat format, SolidColorBrush color, AeVector location, bool isPositionStatic, string name)
        {
            var obj = new AeSpriteTextBlock(Engine, format, color, location, isPositionStatic);
            obj.SpriteTag = name;
            SpriteManager.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Creates and adds a new sprite text block to the sprite manager with the specified formatting, color,
        /// location, and text.
        /// </summary>
        /// <remarks>The created sprite text block is immediately inserted into the sprite manager and can
        /// be accessed or manipulated using its tag name. Ensure that the provided parameters meet any constraints to
        /// avoid runtime exceptions.</remarks>
        /// <param name="format">The text formatting to apply to the sprite text block. Cannot be null.</param>
        /// <param name="color">The brush used to render the text color. Cannot be null.</param>
        /// <param name="location">The position of the sprite text block within the scene.</param>
        /// <param name="isPositionStatic">A value indicating whether the sprite's position remains fixed (<see langword="true"/>) or can change (<see
        /// langword="false"/>).</param>
        /// <param name="name">The tag name assigned to the sprite text block. Used for identification and retrieval. Cannot be null or
        /// empty.</param>
        /// <param name="text">The text content to display in the sprite text block. Cannot be null.</param>
        /// <returns>An instance of <see cref="AeSpriteTextBlock"/> representing the newly added sprite text block.</returns>
        public AeSpriteTextBlock Add(TextFormat format, SolidColorBrush color, AeVector location, bool isPositionStatic, string name, string text)
        {
            var obj = new AeSpriteTextBlock(Engine, format, color, location, isPositionStatic);
            obj.SpriteTag = name;
            obj.Text = text;
            SpriteManager.Insert(obj);
            return obj;
        }

        #endregion
    }
}
