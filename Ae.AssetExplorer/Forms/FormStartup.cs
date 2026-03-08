using System.Runtime.InteropServices;

namespace Ae.AssetExplorer
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

        private bool _rampingUp = true;

        public FormStartup()
        {
            InitializeComponent();

            if (BackgroundImage != null)
            {
                Width = BackgroundImage.Width;
                Height = BackgroundImage.Height;
            }

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

            Shown += (object? sender, EventArgs e) =>
                {
                    var timer = new System.Windows.Forms.Timer()
                    {
                        Enabled = true,
                        Interval = 10,
                    };

                    timer.Tick += (object? sender, EventArgs e) =>
                    {
                        if (_rampingUp)
                        {
                            Opacity += 0.05;
                            if (Opacity >= 1)
                            {
                                timer.Stop();
                                _rampingUp = false;

                                Task.Delay(1000).ContinueWith(_ =>
                                {
                                    if (!IsDisposed)
                                    {
                                        Invoke(new Action(() => timer.Start()));
                                    }
                                });
                            }
                        }
                        else
                        {
                            Opacity -= 0.05;
                            if (Opacity <= 0)
                            {
                                timer.Stop();
                                Close();
                            }
                        }
                    };
                };
        }
    }
}
