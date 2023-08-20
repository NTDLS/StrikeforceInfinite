﻿using AI2D.Actors;
using AI2D.Actors.Enemies;
using AI2D.Engine;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AI2D
{
    public partial class FormMain : Form
    {
        private Core _core;
        private bool _fullScreen = false;

        //This really shouldn't be necessary! :(
        protected override CreateParams CreateParams
        {
            get
            {
                //Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                // For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public FormMain()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            if (_fullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.ShowInTaskbar = true;
                //this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            this.BackColor = Color.FromArgb(1, 1, 10);

            _core = new Core(this, new Size(this.Width, this.Height));
            _core.OnStop += _core_OnStop;

#if DEBUG
            _core.Display.DrawingSurface.MouseDown += DrawingSurface_MouseDown;
            _core.Display.DrawingSurface.MouseMove += DrawingSurface_MouseMove;
#endif
        }

#if DEBUG
        List<ActorBase> highlightedActors = new();

        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            foreach (var actor in highlightedActors)
            {
                actor.Highlight = false;
            }

            highlightedActors.Clear();

            var actors = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1));
            if (_core.Actors.Player.Intersects(new Point<double>(x, y), new Point<double>(1, 1)))
            {
                actors.Add(_core.Actors.Player);
            }

            foreach (var actor in actors)
            {
                highlightedActors.Add(actor);
                actor.Highlight = true;
            }
        }

        private ToolTip _interrogationTip = new ToolTip();

        private void DrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X;
            double y = e.Y;

            var actors = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1));
            if (_core.Actors.Player.Intersects(new Point<double>(x, y), new Point<double>(1, 1)))
            {
                actors.Add(_core.Actors.Player);
            }

            var actor = actors.FirstOrDefault();

            if (actor != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var menu = new ContextMenuStrip();

                    menu.ItemClicked += Menu_ItemClicked;
                    if (actor is EnemyBase)
                    {
                        menu.Items.Add("Save Brain").Tag = actor;
                        menu.Items.Add("View Brain").Tag = actor;
                    }
                    menu.Items.Add("Delete").Tag = actor;

                    var location = new Point((int)e.X + 10, (int)e.Y);
                    menu.Show(_core.Display.DrawingSurface, location);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    var text = new StringBuilder();

                    text.AppendLine($"Type: {actor.GetType().Name}");
                    text.AppendLine($"UID: {actor.UID}");
                    text.AppendLine($"X,Y: {actor.X:n2},{actor.X:n2}");

                    if (actor is EnemyBase)
                    {
                        var enemy = (EnemyBase)actor;

                        text.AppendLine($"Collision Damage: {enemy.CollisionDamage:n0}");
                        text.AppendLine($"Hit Points: {enemy.HitPoints:n0}");
                        text.AppendLine($"Is Locked-on: {enemy.IsLockedOn}");
                        text.AppendLine($"Is Locked-on (Soft): {enemy.IsLockedOnSoft:n0}");
                        text.AppendLine($"Shield Points: {enemy.ShieldPoints:n0}");
                        text.AppendLine($"MaxSpeed: {enemy.Velocity.MaxSpeed:n2}");
                        text.AppendLine($"Angle: {enemy.Velocity.Angle.Degrees:n2}° {enemy.Velocity.Angle:n2}");
                        text.AppendLine($"Throttle Percent: {enemy.Velocity.ThrottlePercentage:n2}");
                        text.AppendLine($"PrimaryWeapon: {enemy.SelectedPrimaryWeapon?.GetType()?.Name}");
                        text.AppendLine($"SelectedSecondaryWeapon: {enemy.SelectedSecondaryWeapon?.GetType()?.Name}");

                        if (enemy.CurrentAIController != null)
                        {
                            text.AppendLine($"AI: {enemy.CurrentAIController.GetType().Name}");
                        }
                    }

                    if (text.Length > 0)
                    {
                        var location = new Point((int)e.X, (int)e.Y - actor.Size.Height);
                        _interrogationTip.Show(text.ToString(), _core.Display.DrawingSurface, location, 5000);
                    }
                }
            }
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (sender == null) return;
            var menu = (ContextMenuStrip)sender;

            menu.Close();

            var actor = e.ClickedItem?.Tag as ActorBase;
            if (actor == null) return;

            if (e.ClickedItem?.Text == "Delete")
            {
                actor.QueueForDelete();
            }
            else if (e.ClickedItem?.Text == "Save Brain")
            {
                if (actor is EnemyBase)
                {
                    var enemy = (EnemyBase)actor;

                    bool wasPaused = _core.IsPaused();
                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }

                    using (var fbd = new FolderBrowserDialog())
                    {
                        if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            foreach (var aiController in enemy.AIControllers)
                            {
                                var fullPath = Path.Combine(fbd.SelectedPath, $"{aiController.Key}.txt");
                                aiController.Value.Network.Save(fullPath);
                            }
                        }
                    }

                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }
                }
            }
            else if (e.ClickedItem?.Text == "View Brain")
            {
                if (actor is EnemyBase)
                {
                    var enemy = (EnemyBase)actor;

                    bool wasPaused = _core.IsPaused();
                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }

                    var builder = new StringBuilder();

                    foreach (var aiController in enemy.AIControllers)
                    {
                        builder.AppendLine($"<!-- {aiController.Key} -->");
                        builder.AppendLine(aiController.Value.Network.Serialize());
                    }

                    using (var form = new FormViewBrain(builder.ToString()))
                    {
                        form.ShowDialog();
                    }

                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }
                }
            }
        }
#endif

        private void _core_OnStop(Core sender)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            _core.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _core.Stop();
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(Types.PlayerKey.SpeedBoost, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(Types.PlayerKey.Forward, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(Types.PlayerKey.RotateCounterClockwise, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(Types.PlayerKey.Reverse, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(Types.PlayerKey.RotateClockwise, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(Types.PlayerKey.PrimaryFire, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(Types.PlayerKey.SecondaryFire, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(Types.PlayerKey.Escape, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(Types.PlayerKey.Left, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(Types.PlayerKey.Right, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(Types.PlayerKey.Up, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(Types.PlayerKey.Down, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(Types.PlayerKey.Enter, Types.KeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _core.Pause();

                if (MessageBox.Show("Are you sure you want to quit?", "Afraid to go on?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.Close();
                }
                else
                {
                    _core.Resume();
                }
            }

            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(Types.PlayerKey.SpeedBoost, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(Types.PlayerKey.Forward, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(Types.PlayerKey.RotateCounterClockwise, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(Types.PlayerKey.Reverse, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(Types.PlayerKey.RotateClockwise, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(Types.PlayerKey.PrimaryFire, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(Types.PlayerKey.SecondaryFire, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(Types.PlayerKey.Escape, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(Types.PlayerKey.Left, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(Types.PlayerKey.Right, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(Types.PlayerKey.Up, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(Types.PlayerKey.Down, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(Types.PlayerKey.Enter, Types.KeyPressState.Up);
        }

        private void FormMain_MouseEnter(object sender, EventArgs e)
        {
            if (_fullScreen)
            {
                Cursor.Hide();
            }
        }

        private void FormMain_MouseLeave(object sender, EventArgs e)
        {
            if (_fullScreen)
            {
                Cursor.Show();
            }
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_core.Actors.Render(), 0, 0);
        }
    }
}
