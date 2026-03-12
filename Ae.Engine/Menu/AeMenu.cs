using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.MenuItem;
using Ae.Engine.Sprite.TextBlock;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ae.Engine.Menu
{
    /// <summary>
    /// A menu instance. Allows for setting title text, adding items and managing selections.
    /// </summary>
    public class AeMenu(AeEngine engine)
    {
        /// <summary>
        /// Gets the engine instance used for executing automation tasks.
        /// </summary>
        public AeEngine Engine { get; private set; } = engine;
        private DateTime _lastInputHandled = DateTime.UtcNow;

        /// <summary>
        /// Gets the collection of menu items contained in the sprite menu.
        /// </summary>
        public List<AeSpriteMenuItem> Items { get; private set; } = new();

        /// <summary>
        /// Gets a value indicating whether the item is currently queued for deletion.
        /// </summary>
        public bool QueuedForDeletion { get; private set; }

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid UID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Returns the first menu item with the specified tag name, or null if no matching item is found.
        /// </summary>
        /// <param name="name">The tag name to search for among menu items. Cannot be null.</param>
        /// <returns>The first menu item whose tag matches the specified name, or null if no such item exists.</returns>
        public AeSpriteMenuItem? FirstMenuItemByTag(string name) => Items.FirstOrDefault(o => o.SpriteTag == name);

        /// <summary>
        /// Returns a collection of menu items that have the specified tag.
        /// </summary>
        /// <param name="name">The tag name used to filter menu items. Cannot be null.</param>
        /// <returns>An enumerable collection of menu items whose tag matches the specified name. The collection will be empty if
        /// no items match.</returns>
        public IEnumerable<AeSpriteMenuItem> AllMenuItemsByTag(string name) => Items.Where(o => o.SpriteTag == name);

        /// <summary>
        /// Retrieves the menu item of the specified type that matches the given key.
        /// </summary>
        /// <remarks>Throws an exception if no menu item with the specified key exists. Use this method
        /// when you expect exactly one item to match the key.</remarks>
        /// <typeparam name="T">The type of menu item to retrieve. Must inherit from AeSpriteMenuItem.</typeparam>
        /// <param name="key">The key used to identify the menu item. Cannot be null.</param>
        /// <returns>The menu item of type T with the specified key.</returns>
        public T MenuItemByKey<T>(string key) where T : AeSpriteMenuItem
            => (T)Items.First(o => o.SpriteTag == key);

        /// <summary>
        /// Returns a list of menu items that are both visible and selectable.
        /// </summary>
        /// <remarks>Selectable items include those of type <see cref="AeMenuItemType.SelectableItem"/> or
        /// <see cref="AeMenuItemType.SelectableTextInput"/>. Use this method to retrieve items that users can interact
        /// with in the menu.</remarks>
        /// <returns>A list of <see cref="AeSpriteMenuItem"/> objects representing the visible menu items that can be selected.
        /// The list will be empty if no such items are available.</returns>
        public List<AeSpriteMenuItem> VisibleSelectableItems() =>
            Items.Where(o => o.IsVisible == true
            && (o.ItemType == AeMenuItemType.SelectableItem || o.ItemType == AeMenuItemType.SelectableTextInput)).ToList();

        #region Events.

        /// <summary>
        /// Represents a method that handles selection change events for a menu item.
        /// </summary>
        /// <param name="item">The menu item whose selection state has changed.</param>
        public delegate void SelectionChangedEvent(AeSpriteMenuItem item);
        /// <summary>
        /// The player moved the selection cursor.
        /// </summary>
        public event SelectionChangedEvent? OnSelectionChanged;

        /// <summary>
        /// Invokes the selection changed event for the specified menu item.
        /// </summary>
        /// <param name="item">The menu item that was selected. Cannot be null.</param>
        public void InvokeSelectionChanged(AeSpriteMenuItem item) => OnSelectionChanged?.Invoke(item);

        /// <summary>
        /// The player hit enter to select the currently highlighted menu item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return true to close the current menu.</returns>
        public delegate bool ExecuteSelectionEvent(AeSpriteMenuItem item);
        /// <summary>
        /// The player hit enter to select the currently highlighted menu item.
        /// </summary>
        public event ExecuteSelectionEvent? OnExecuteSelection;

        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        /// <returns>Return true to close the current menu.</returns>
        public delegate bool EscapeEvent();
        /// <summary>
        /// The player has hit the escape key.
        /// </summary>
        public event EscapeEvent? OnEscape;

        /// <summary>
        /// Represents a method that handles cleanup events.
        /// </summary>
        /// <remarks>Use this delegate to define callback methods that execute when a cleanup operation is
        /// triggered. Typical scenarios include resource disposal or finalization steps in application
        /// components.</remarks>
        public delegate void CleanupEvent();

        /// <summary>
        /// Called when the menu is being destroyed. This is a good place to cleanup.
        /// </summary>
        public event CleanupEvent? OnCleanup;

        #endregion

        /// <summary>
        /// Determines whether the escape event handler is assigned.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if an escape event handler is present; otherwise, <see langword="false"/>.</returns>
        public bool HandlesEscape() => OnEscape != null;

        /// <summary>
        /// Closes the menu and schedules it for deletion, preventing further input and triggering cleanup events.
        /// </summary>
        /// <remarks>After calling this method, the menu will be unloaded and any registered cleanup
        /// actions will be invoked. No further input will be processed for this menu. This method should be used when
        /// the menu is no longer needed or before disposing of related resources.</remarks>
        public void Close()
        {
            QueuedForDeletion = true; //Just so we ignore any input until the menu is deleted.
            Engine.Events.Add(() => OnCleanup?.Invoke());
            Engine.Menus.Unload(this);
        }

        /// <summary>
        /// Centers an array of text blocks horizontally on the canvas, positioning them with the specified vertical
        /// coordinate and spacing.
        /// </summary>
        /// <remarks>This method adjusts the X position of each text block so that the group is centered
        /// horizontally. The Y position of each block should be set separately if needed.</remarks>
        /// <param name="textBlocks">An array of text blocks to be positioned. The order of the array determines their placement from left to
        /// right.</param>
        /// <param name="y">The vertical coordinate, in pixels, at which to position the text blocks.</param>
        /// <param name="spacing">The horizontal spacing, in pixels, between each text block. Defaults to 5.</param>
        public void CenterHorizontally(AeSpriteTextBlock[] textBlocks, float y, int spacing = 5)
        {
            var totalWidth = textBlocks.Sum(o => o.Size.Width) + (textBlocks.Length * spacing);

            var currentScaledScreenBounds = Engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = (Engine.Display.TotalCanvasSize.Width / 2) - (totalWidth / 2);

            foreach (var block in textBlocks)
            {
                block.X = offsetX;
                offsetX += block.Size.Width + spacing;
            }
        }

        /// <summary>
        /// Adds a title item to the menu at the specified location with the given text.
        /// </summary>
        /// <remarks>The title item uses the menu's title text format and a red brush for rendering. Use
        /// this method to visually distinguish section headers or titles within the menu.</remarks>
        /// <param name="location">The position within the menu where the title item will be placed.</param>
        /// <param name="text">The text to display for the title item.</param>
        /// <returns>An instance of AeSpriteMenuItem representing the newly added title item.</returns>
        public AeSpriteMenuItem AddTitleItem(AeVector location, string text)
        {
            var item = new AeSpriteMenuItem(Engine, this, Engine.Rendering.TextFormats.MenuTitle, Engine.Rendering.Materials.Brushes.Red, location)
            {
                Text = text,
                ItemType = AeMenuItemType.Title
            };
            AddMenuItem(item);
            return item;
        }

        /// <summary>
        /// Adds a text block menu item at the specified location with the given text.
        /// </summary>
        /// <remarks>The text block is styled using the menu's general text format and a red brush. Use
        /// the returned AeSpriteMenuItem to further configure the item if needed.</remarks>
        /// <param name="location">The position where the text block menu item will be placed.</param>
        /// <param name="text">The text to display in the menu item.</param>
        /// <returns>An instance of AeSpriteMenuItem representing the newly added text block.</returns>
        public AeSpriteMenuItem AddTextBlock(AeVector location, string text)
        {
            var item = new AeSpriteMenuItem(Engine, this, Engine.Rendering.TextFormats.MenuGeneral, Engine.Rendering.Materials.Brushes.Red, location)
            {
                Text = text,
                ItemType = AeMenuItemType.TextBlock
            };
            AddMenuItem(item);
            return item;
        }

        /// <summary>
        /// Adds a selectable menu item at the specified location with the given key and display text.
        /// </summary>
        /// <remarks>The added item will be configured as a selectable menu entry. Use the returned
        /// AeSpriteMenuItem to further customize or interact with the item after it is added.</remarks>
        /// <param name="location">The position of the menu item within the menu layout.</param>
        /// <param name="key">The unique identifier for the menu item. Used to reference or distinguish the item within the menu.</param>
        /// <param name="text">The text to display for the menu item.</param>
        /// <returns>An instance of AeSpriteMenuItem representing the newly added selectable menu item.</returns>
        public AeSpriteMenuItem AddSelectableItem(AeVector location, string key, string text)
        {
            var item = new AeSpriteMenuItem(Engine, this, Engine.Rendering.TextFormats.MenuItem, Engine.Rendering.Materials.Brushes.OrangeRed, location)
            {
                SpriteTag = key,
                Text = text,
                ItemType = AeMenuItemType.SelectableItem
            };
            AddMenuItem(item);
            return item;
        }

        /// <summary>
        /// Adds a selectable text input menu item at the specified location.
        /// </summary>
        /// <remarks>Use this method to create interactive text input fields within a sprite menu. The
        /// menu item will be selectable and can be identified using the provided key.</remarks>
        /// <param name="location">The position where the text input menu item will be placed.</param>
        /// <param name="key">The unique tag used to identify the menu item.</param>
        /// <param name="text">The initial text displayed in the input field. Defaults to an empty string.</param>
        /// <param name="characterLimit">The maximum number of characters allowed in the input field. Defaults to 100.</param>
        /// <returns>An instance of AeSpriteMenuSelectableTextInput representing the added menu item.</returns>
        public AeSpriteMenuSelectableTextInput AddSelectableTextInput(AeVector location, string key, string text = "", int characterLimit = 100)
        {
            var item = new AeSpriteMenuSelectableTextInput(Engine, this, Engine.Rendering.TextFormats.TextInputItem, Engine.Rendering.Materials.Brushes.LawnGreen, location)
            {
                SpriteTag = key,
                Text = text,
                CharacterLimit = characterLimit,
                ItemType = AeMenuItemType.SelectableTextInput
            };
            AddMenuItem(item);
            return item;
        }

        /// <summary>
        /// Adds a menu item to the collection of menu items.
        /// </summary>
        /// <param name="item">The menu item to add to the collection. Cannot be null.</param>
        public void AddMenuItem(AeSpriteMenuItem item) => Items.Add(item);

        /// <summary>
        /// Processes user input for the menu, handling navigation, selection, text entry, and menu actions such as
        /// executing or closing the menu.
        /// </summary>
        /// <remarks>This method responds to key presses for menu navigation (up, down, left, right),
        /// selection (Enter), cancellation (Escape), and text input for selectable text fields. It enforces timing
        /// constraints to prevent rapid input handling and ensures menu actions are executed asynchronously to avoid
        /// blocking the main thread. Selection changes and menu actions trigger associated event handlers if
        /// defined.</remarks>
        public void HandleInput()
        {
            if (QueuedForDeletion)
            {
                Thread.Sleep(1);
                return;
            }

            var selectedTextInput = Items.OfType<AeSpriteMenuSelectableTextInput>().Where(o => o.Selected).FirstOrDefault();

            Engine.Input.CollectDetailedKeyInformation(selectedTextInput != null);

            //Text typing is not subject to _lastInputHandled limits because it is based on cycled keys, not depressed keys.
            if (selectedTextInput != null)
            {
                //Since we do allow for backspace repetitions, we will enforce a _lastInputHandled limit.
                if (Engine.Input.DepressedKeys.Contains(Key.Back))
                {
                    if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds >= 100)
                    {
                        _lastInputHandled = DateTime.UtcNow;
                        selectedTextInput.Backspace();
                        Engine.Audio.Click?.Play();
                    }
                    return;
                }

                if (Engine.Input.TypedString?.Length > 0)
                {
                    Engine.Audio.Click?.Play();
                    selectedTextInput.Append(Engine.Input.TypedString);
                }
            }

            if ((DateTime.UtcNow - _lastInputHandled).TotalMilliseconds < 200)
            {
                return; //We have to keep the menus from going crazy.
            }

            if (Engine.Input.IsKeyPressed(AePlayerKey.Enter))
            {
                Engine.Audio.Click?.Play();

                _lastInputHandled = DateTime.UtcNow;

                var selectedItem = (from o in Items where o.ItemType == AeMenuItemType.SelectableItem && o.Selected == true select o).FirstOrDefault();
                if (selectedItem != null)
                {
                    //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                    //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                    //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.

                    Engine.Events.Add(() =>
                    {
                        if (OnExecuteSelection?.Invoke(selectedItem) == true)
                        {
                            Close();
                        }
                    });
                }
            }
            else if (Engine.Input.IsKeyPressed(AePlayerKey.Escape))
            {
                Engine.Audio.Click?.Play();

                _lastInputHandled = DateTime.UtcNow;

                //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                //  
                Engine.Events.Add(() =>
                {
                    if (OnEscape?.Invoke() == true)
                    {
                        Close();
                    }
                });
            }

            if (Engine.Input.IsKeyPressed(AePlayerKey.Right)
                || Engine.Input.IsKeyPressed(AePlayerKey.Down)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.Reverse)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise)
                )
            {
                _lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in Items
                             where o.ItemType == AeMenuItemType.SelectableItem || o.ItemType == AeMenuItemType.SelectableTextInput
                             select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == AeMenuItemType.SelectableItem || item.ItemType == AeMenuItemType.SelectableTextInput)
                        {
                            if (item.Selected)
                            {
                                selectIndex = i + 1;
                                item.Selected = false;
                                previouslySelectedIndex = i;
                            }
                        }
                    }

                    if (selectIndex >= items.Count)
                    {
                        selectIndex = items.Count - 1;
                    }

                    items[selectIndex].Selected = true;

                    if (selectIndex != previouslySelectedIndex)
                    {
                        var selectedItem = (from o in Items
                                            where (o.ItemType == AeMenuItemType.SelectableItem || o.ItemType == AeMenuItemType.SelectableTextInput) && o.Selected == true
                                            select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            Engine.Audio.Click?.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Engine.Events.Add(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }

            if (Engine.Input.IsKeyPressed(AePlayerKey.Left)
                || Engine.Input.IsKeyPressed(AePlayerKey.Up)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                //|| _engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise)
                )
            {
                _lastInputHandled = DateTime.UtcNow;

                int selectIndex = 0;

                var items = (from o in Items
                             where o.ItemType == AeMenuItemType.SelectableItem || o.ItemType == AeMenuItemType.SelectableTextInput
                             select o).ToList();
                if (items != null && items.Count > 0)
                {
                    int previouslySelectedIndex = -1;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item.ItemType == AeMenuItemType.SelectableItem || item.ItemType == AeMenuItemType.SelectableTextInput)
                        {
                            if (item.Selected)
                            {
                                selectIndex = i - 1;
                                previouslySelectedIndex = i;
                                item.Selected = false;
                            }
                        }
                    }

                    if (selectIndex < 0)
                    {
                        selectIndex = 0;
                    }

                    items[selectIndex].Selected = true;

                    if (selectIndex != previouslySelectedIndex)
                    {
                        var selectedItem = (from o in Items
                                            where (o.ItemType == AeMenuItemType.SelectableItem || o.ItemType == AeMenuItemType.SelectableTextInput) && o.Selected == true
                                            select o).FirstOrDefault();
                        if (selectedItem != null)
                        {
                            Engine.Audio.Click?.Play();

                            //Menu executions may block execution if run in the same thread. For example, the menu execution may be looking to remove all
                            //  items from the screen and wait for them to be removed. Problem is, the same thread that calls the menuExecution is the same
                            //  one that removes items from the screen, therefor the "while(itemsExist)" loop would never finish.
                            //  
                            Engine.Events.Add(() => OnSelectionChanged?.Invoke(selectedItem));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders all visible items to the specified Direct2D render target for the given epoch. Highlights the
        /// selected item if present.
        /// </summary>
        /// <remarks>If a visible item is selected, a rectangle is drawn around it to indicate selection.
        /// Only items with IsVisible set to <see langword="true"/> are rendered.</remarks>
        /// <param name="renderTarget">The Direct2D render target to which the items will be rendered. Must not be null.</param>
        /// <param name="epoch">The epoch value representing the current time or frame for rendering. Used to synchronize item rendering.</param>
        internal virtual void Render(SharpDX.Direct2D1.RenderTarget renderTarget, float epoch)
        {
            foreach (var item in Items.Where(o => o.IsVisible == true))
            {
                item.Render(renderTarget, epoch);
            }

            var selectedItem = (from o in Items where o.IsVisible == true && o.Selected == true select o).FirstOrDefault();
            if (selectedItem != null)
            {
                Engine.Rendering.DrawRectangle(renderTarget, selectedItem.RawBounds, Engine.Rendering.Materials.Colors.LawnGreen, 2, 2, 0);
            }
        }
    }
}
