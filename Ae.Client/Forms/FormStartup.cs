using Ae.Client.Hardware;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ae.Client
{
    public partial class FormStartup
        : Form
    {
        internal class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            public const int WM_NCLBUTTONDOWN = 0xA1;
            public const int HTCAPTION = 0x2;
        }

        public Screen CurrentScreen { get; private set; }

        public FormStartup()
        {
            InitializeComponent();
            CurrentScreen = Screen.FromPoint(Cursor.Position);
            this.CenterFormOnScreen(CurrentScreen);

            if (BackgroundImage != null)
            {
                Width = BackgroundImage.Width;
                Height = BackgroundImage.Height;
            }

            AcceptButton = buttonStart;
            CancelButton = buttonExit;

            buttonStart.Visible = false;
            buttonExit.Visible = false;
            buttonSettings.Visible = false;
            TopMost = false;
            StartPosition = FormStartPosition.CenterScreen;
            Opacity = 0;
            // Set a unique color as the transparency key to make the form's background transparent.
            TransparencyKey = Color.FromArgb(12, 10, 12);
            BackColor = TransparencyKey;

            MouseDown += (object? sender, MouseEventArgs e) =>
            {
                //Allow dragging the form by clicking anywhere on it.
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, (IntPtr)NativeMethods.HTCAPTION, IntPtr.Zero);
                }
            };

            Move += (object? sender, EventArgs e) => CurrentScreen = this.GetCurrentScreen();

            Shown += (object? sender, EventArgs e) =>
                {
                    var timer = new Timer()
                    {
                        Enabled = true,
                        Interval = 10,
                    };

                    timer.Tick += (object? sender, EventArgs e) =>
                    {
                        Opacity += 0.05;
                        if (Opacity >= 1)
                        {
                            Task.Delay(1000).ContinueWith(_ =>
                            {
                                if (!IsDisposed)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        buttonStart.Visible = true;
                                        buttonExit.Visible = true;
                                        buttonSettings.Visible = true;
                                        buttonStart.Focus();
                                    }));
                                }
                            });

                            timer.Stop();
                        }
                    };

                    timer.Start();
                };
        }

        private void ButtonExit_Click(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonSettings_Click(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
            using var form = new FormSettings(CurrentScreen);
            form.ShowDialog();
        }

        private void ButtonStart_Click(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
