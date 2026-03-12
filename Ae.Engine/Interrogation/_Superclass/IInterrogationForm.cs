using Ae.Engine.Sprite;

namespace Ae.Engine.Interrogation._Superclass
{
    /// <summary>
    /// Defines the contract for a form that displays interrogation text and manages its visibility and content within
    /// an application.
    /// </summary>
    /// <remarks>Implementations of this interface provide methods to display, update, and clear interrogation
    /// text, as well as to control the form's visibility. The interface is intended for use in scenarios where text
    /// output and interaction with engine and sprite objects are required, such as in game or simulation
    /// environments.</remarks>
    public interface IInterrogationForm
    {
        /// <summary>
        /// Begins monitoring the specified sprite within the given engine context.
        /// </summary>
        /// <param name="engine">The engine instance in which the sprite will be watched. Cannot be null.</param>
        /// <param name="sprite">The sprite to monitor for changes or events. Cannot be null.</param>
        public void StartWatch(AeEngine engine, IAeSprite sprite);

        /// <summary>
        /// Writes the specified text to the output using the given color.
        /// </summary>
        /// <param name="text">The text to be written to the output. Cannot be null.</param>
        /// <param name="color">The color to use when displaying the text.</param>
        public void WriteLine(string text, System.Drawing.Color color);

        /// <summary>
        /// Writes the specified text to the output using the given color.
        /// </summary>
        /// <param name="text">The text to be written to the output. Cannot be null.</param>
        /// <param name="color">The color to use when displaying the text.</param>
        public void Write(string text, System.Drawing.Color color);

        /// <summary>
        /// Clears all text content from the current instance.
        /// </summary>
        /// <remarks>Use this method to reset the text state, removing any existing content. After calling
        /// this method, the instance will contain no text. This operation does not affect other properties or
        /// settings.</remarks>
        public void ClearText();

        /// <summary>
        /// Displays the associated user interface element or window.
        /// </summary>
        public void Show();

        /// <summary>
        /// Hides the current element from view.
        /// </summary>
        public void Hide();
    }
}
