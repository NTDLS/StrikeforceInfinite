using SharpDX.DirectWrite;

namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Provides a set of preconfigured text formats for common UI elements such as menus, loading screens, and debug
    /// displays.
    /// </summary>
    /// <remarks>This class centralizes access to standard text formatting used throughout the application,
    /// ensuring consistent appearance for various interface components. Each property exposes a specific text format
    /// intended for a particular UI context. The formats are initialized during construction and are read-only after
    /// creation.</remarks>
    public class AePrecreatedTextFormats
    {
        /// <summary>
        /// Gets the text formatting settings for the general menu.
        /// </summary>
        public TextFormat MenuGeneral { get; private set; }
        /// <summary>
        /// Gets the formatted text used as the menu title.
        /// </summary>
        public TextFormat MenuTitle { get; private set; }
        /// <summary>
        /// Gets the text formatting applied to the menu item.
        /// </summary>
        public TextFormat MenuItem { get; private set; }
        /// <summary>
        /// Gets the text input item formatted according to the specified text format.
        /// </summary>
        public TextFormat TextInputItem { get; private set; }
        /// <summary>
        /// Gets the text format used for displaying large blocker elements.
        /// </summary>
        public TextFormat LargeBlocker { get; private set; }
        /// <summary>
        /// Gets the text format used to display the radar position indicator.
        /// </summary>
        public TextFormat RadarPositionIndicator { get; private set; }
        /// <summary>
        /// Gets the text format used to display real-time player statistics.
        /// </summary>
        public TextFormat RealtimePlayerStats { get; private set; }
        /// <summary>
        /// Gets the text format used for displaying loading messages.
        /// </summary>
        public TextFormat Loading { get; private set; }
        /// <summary>
        /// Gets the text format used for debug output.
        /// </summary>
        public TextFormat Debug { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AePrecreatedTextFormats class with predefined text formats for various UI
        /// elements.
        /// </summary>
        /// <remarks>The constructor sets up commonly used text formats for UI components such as debug
        /// output, loading screens, menu items, and player statistics. Each format is configured with specific font
        /// settings and word wrapping options to match its intended usage.</remarks>
        /// <param name="factory">The factory used to create text format instances. Cannot be null.</param>
        public AePrecreatedTextFormats(Factory factory)
        {
            //Digital-7 Mono

            Debug = new TextFormat(factory, "Consolas", 10) { WordWrapping = WordWrapping.NoWrap };
            Loading = new TextFormat(factory, "Consolas", 30) { WordWrapping = WordWrapping.NoWrap };
            LargeBlocker = new TextFormat(factory, "Orbitronio", 50) { WordWrapping = WordWrapping.NoWrap };
            MenuGeneral = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap };
            MenuTitle = new TextFormat(factory, "Orbitronio", 72) { WordWrapping = WordWrapping.NoWrap };
            MenuItem = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap };
            RadarPositionIndicator = new TextFormat(factory, "Digital-7 Mono", 16) { WordWrapping = WordWrapping.NoWrap };
            RealtimePlayerStats = new TextFormat(factory, "Consolas", 16) { WordWrapping = WordWrapping.NoWrap };
            TextInputItem = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap, };
        }
    }
}
