using Si.Client.Forms;
using Si.Client.Hardware;
using Si.Engine;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Si.Library.SiConstants;

namespace Si.Client
{
    public partial class FormRenderTarget : Form
    {
        private readonly List<SpriteBase> highlightedSprites = new();
        private readonly ToolTip _interrogationTip = new();
        private readonly EngineCore _engine;
        private readonly bool _fullScreen = false;

        public FormRenderTarget()
        {
            InitializeComponent();

            var drawingSurface = new Control();
            Controls.Add(drawingSurface);
            _engine = new EngineCore(drawingSurface, SiEngineInitializationType.None);
        }

        public FormRenderTarget(Screen screen)
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            var settings = EngineCore.LoadSettings();

            if (settings.FullScreen)
            {
                this.SetFullScreenOnMonitor(screen);
            }
            else
            {
                this.CenterFormOnScreen(screen, settings.Resolution);
            }

            var drawingSurface = new Control
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(drawingSurface);

            _engine = new EngineCore(drawingSurface, SiEngineInitializationType.Play);

            _engine.EnableDebugging(new FormInterrogation(_engine));

            _engine.OnShutdown += (EngineCore sender) =>
            {   //If the engine is stopped, close the main form.
                Invoke((MethodInvoker)delegate
                {
                    Close();
                });
            };

            Shown += (object? sender, EventArgs e)
                => _engine.StartEngine();

            FormClosed += (sender, e)
                => _engine.ShutdownEngine();

            drawingSurface.MouseEnter += (object? sender, EventArgs e) => { if (_fullScreen) { Cursor.Hide(); } };
            drawingSurface.MouseLeave += (object? sender, EventArgs e) => { if (_fullScreen) { Cursor.Show(); } };

            drawingSurface.GotFocus += (object? sender, EventArgs e) => _engine.Display.SetIsDrawingSurfaceFocused(true);
            drawingSurface.LostFocus += (object? sender, EventArgs e) => _engine.Display.SetIsDrawingSurfaceFocused(false);

            drawingSurface.KeyUp += FormRenderTarget_KeyUp;

            if (settings.EnableSpriteInterrogation)
            {
                drawingSurface.MouseDown += FormRenderTarget_MouseDown;
                drawingSurface.MouseMove += FormRenderTarget_MouseMove;
            }
        }

        #region Debug interactions.
        private void FormRenderTarget_MouseMove(object? sender, MouseEventArgs e)
        {
            _engine.AddRenderLoopInterjection(RenderLoopInterjectionLifetime.Once, () =>
            {
                float x = e.X + _engine.Display.OverdrawSize.Width / 2;
                float y = e.Y + _engine.Display.OverdrawSize.Height / 2;

                //Debug.Print($"x{x:n1}, y{y:n1} => Player x{_engine.Player.Sprite.X:n1},x{_engine.Player.Sprite.Y:n1}");

                foreach (var sprite in highlightedSprites)
                {
                    sprite.IsHighlighted = false;
                }

                highlightedSprites.Clear();

                var sprites = _engine.Sprites.RenderLocationIntersections(new SiVector(x, y), new SiVector(1, 1)).ToList();
                if (_engine.Player.Sprite.RenderLocationIntersectsAABB(new SiVector(x, y), new SiVector(1, 1)))
                {
                    sprites.Add(_engine.Player.Sprite);
                }

                foreach (var sprite in sprites.Where(o => o.IsHighlighted == false))
                {
                    highlightedSprites.Add(sprite);
                    sprite.IsHighlighted = true;
                }
            });
        }

        private void FormRenderTarget_MouseDown(object? sender, MouseEventArgs e)
        {
            float x = e.X + _engine.Display.OverdrawSize.Width / 2;
            float y = e.Y + _engine.Display.OverdrawSize.Height / 2;

            List<SpriteBase>? sprites = null;

            _engine.AddRenderLoopInterjection(RenderLoopInterjectionLifetime.Once, () =>
            {
                sprites = _engine.Sprites.RenderLocationIntersections(new SiVector(x, y), new SiVector(1, 1), true).ToList();
                if (_engine.Player.Sprite.RenderLocationIntersectsAABB(new SiVector(x, y), new SiVector(1, 1)))
                {
                    sprites.Add(_engine.Player.Sprite);
                }
            }).Wait();

            if (sprites?.Count > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var menu = new ContextMenuStrip();

                    var watchMenu = new ToolStripMenuItem("Watch");
                    watchMenu.DropDownItemClicked += WatchMenu_ItemClicked;
                    menu.Items.Add(watchMenu);
                    foreach (var sprite in sprites)
                    {
                        watchMenu.DropDownItems.Add(sprite.GetType().Name).Tag = sprite;
                    }

                    var deleteMenu = new ToolStripMenuItem("Delete");
                    deleteMenu.DropDownItemClicked += DeleteMenu_ItemClicked;
                    menu.Items.Add(deleteMenu);
                    foreach (var sprite in sprites)
                    {
                        deleteMenu.DropDownItems.Add(sprite.GetType().Name).Tag = sprite;
                    }

                    var location = new Point((int)e.X + 10, (int)e.Y);
                    menu.Show(_engine.Display.DrawingSurface, location);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    /*
                    var text = new StringBuilder();

                    text.AppendLine($"Type: {sprite.GetType().Name}");
                    text.AppendLine($"UID: {sprite.UID}");
                    text.AppendLine($"Location: {sprite.Location}");

                    if (sprite is SpriteEnemyBase enemy)
                    {
                        text.AppendLine($"Hit Points: {enemy.HullHealth:n0}");
                        text.AppendLine($"Is Locked-on: {enemy.IsLockedOnHard}");
                        text.AppendLine($"Is Locked-on (Soft): {enemy.IsLockedOnSoft:n0}");
                        text.AppendLine($"Shield Points: {enemy.ShieldHealth:n0}");
                        text.AppendLine($"Speed: {enemy.Speed:n2}");
                        text.AppendLine($"Angle: {enemy.Orientation.Degrees:n2}° {enemy.Orientation:n2}");
                        //text.AppendLine($"Throttle Percent: {enemy.Velocity.ForwardVelocity:n2}");

                        if (enemy.CurrentAIController != null)
                        {
                            text.AppendLine($"AI: {enemy.CurrentAIController.GetType().Name}");
                        }
                    }

                    if (text.Length > 0)
                    {
                        var location = new Point((int)e.X, (int)e.Y - sprite.Size.Height);
                        _interrogationTip.Show(text.ToString(), _engine.Display.DrawingSurface, location, 5000);
                    }
                    */
                }
            }
        }

        private void WatchMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not SpriteBase sprite) return;

            new Thread(o =>
            {
                using var form = new FormInterrogationSpriteWatch(_engine, sprite);
                form.ShowDialog();
            }).Start();
        }

        private void DeleteMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not SpriteBase sprite) return;

            sprite.QueueForDelete();
        }

        #endregion

        private void FormRenderTarget_KeyUp(object? sender, KeyEventArgs e)
        {
            _engine.Input.HandleSingleKeyPress(e.KeyCode);

            if (e.KeyCode == Keys.Escape)
            {
                //We do not want the escape key to interrupt menus.
                if (_engine.Menus.Current?.HandlesEscape() != true)
                {
                    _engine.Pause();

                    if (MessageBox.Show("Are you sure you want to quit?", "Afraid to go on?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Close();
                    }
                    else
                    {
                        _engine.Resume();
                    }
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Prevent painting to avoid flickering.
        }
    }
}
