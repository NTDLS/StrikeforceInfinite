using Ae.Engine.Menu;
using SharpDX.Direct2D1;
using System.Collections.Generic;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    /// <summary>
    /// Controls the display and input handling of menu screens within the engine by managing the active menu and
    /// coordinating tick updates.
    /// </summary>
    /// <remarks>This controller is responsible for showing, unloading, and updating menus in response to
    /// engine ticks. Only one menu can be active at a time. Use the Show method to display a new menu, and Unload to
    /// close the current menu. The controller delegates rendering and input handling to the active menu. Thread safety
    /// is not guaranteed; access should be synchronized if used from multiple threads.</remarks>
    public class MenuTickController
        : UnvectoredTickControllerBase<AeMenu>
    {
        /// <summary>
        /// Represents a method that provides access to a collection of menu items.
        /// </summary>
        /// <param name="sprites">The list of menu items to be accessed. Cannot be null.</param>
        public delegate void CollectionAccessor(List<AeMenu> sprites);
        /// <summary>
        /// Represents a method that provides access to a collection of menu items.
        /// </summary>
        public delegate T CollectionAccessorT<T>(List<AeMenu> sprites);

        private AeMenu? _current = null;

        /// <summary>
        /// Gets the currently displayed item.
        /// </summary>
        public AeMenu? Current { get => _current; }

        /// <summary>
        /// Initializes a new instance of the MenuTickController class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to manage menu tick operations. Cannot be null.</param>
        public MenuTickController(AeEngine engine)
            : base(engine) { }

        internal void Render(RenderTarget renderTarget, float epoch)
            => _current?.Render(renderTarget, epoch);

        /// <summary>
        /// Displays the specified menu, replacing any currently active menu.
        /// </summary>
        /// <remarks>If a menu is already active, it will be unloaded before the new menu is
        /// shown.</remarks>
        /// <param name="menu">The menu to display. Cannot be null.</param>
        public void Show(AeMenu menu)
        {
            Unload(_current);
            _current = menu;
        }

        /// <summary>
        /// Closes and unloads the specified menu if it is currently active.
        /// </summary>
        /// <remarks>If the menu has already been queued for deletion, it will not be closed again. This
        /// method sets the current menu reference to null after unloading.</remarks>
        /// <param name="menu">The menu to unload. If the menu is not currently active, no action is taken.</param>
        public void Unload(AeMenu? menu)
        {
            if (_current == menu)
            {
                //QueuedForDeletion is set in MenuBase.Close, so if it is true, then MenuBase.Close has already been called.
                if (_current?.QueuedForDeletion == false)
                {
                    _current.Close();
                }
                _current = null;
            }
        }

        /// <summary>
        /// Advances the world clock by processing input for the current state.
        /// </summary>
        /// <remarks>This method delegates input handling to the current state, if one is set. It is
        /// typically called once per tick to update the simulation or game logic. If no current state is active, the
        /// method performs no action.</remarks>
        public override void ExecuteWorldClockTick() => _current?.HandleInput();
    }
}
