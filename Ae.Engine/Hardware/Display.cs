using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ae.Engine.Hardware
{
    /// <summary>
    /// Provides utility methods for managing and querying display settings, monitor information, and form positioning
    /// in multi-monitor environments.
    /// </summary>
    /// <remarks>The methods in this class assist with tasks such as retrieving monitor refresh rates,
    /// centering forms on specific screens, setting forms to full-screen mode, and determining the current screen for a
    /// control. These utilities are designed to simplify display-related operations in Windows Forms applications,
    /// especially when working with multiple monitors.</remarks>
    public static class Display
    {
        #region DEVMODE struct.

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }


        #endregion

        private const int ENUM_CURRENT_SETTINGS = -1;

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        /// <summary>
        /// Retrieves the refresh rate, in hertz, of the monitor displaying the specified control.
        /// </summary>
        /// <remarks>This method uses the current display settings of the monitor associated with the
        /// specified control. If the monitor information is unavailable, a default value of 60 hertz is
        /// returned.</remarks>
        /// <param name="control">The control whose associated monitor's refresh rate is to be determined. Cannot be null.</param>
        /// <returns>The refresh rate of the monitor, in hertz. Returns 60 if the refresh rate cannot be determined.</returns>
        public static uint GetControlMonitorRefreshRate(Control control)
        {
            var screen = Screen.FromControl(control);
            if (screen != null)
            {
                var deviceMode = new DEVMODE();
                if (EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref deviceMode))
                {
                    return deviceMode.dmDisplayFrequency;
                }
            }

            return 60;
        }

        /// <summary>
        /// Centers the specified form on the given screen, optionally resizing it before positioning.
        /// </summary>
        /// <remarks>This method sets the form's start position to manual and updates its location to be
        /// centered within the bounds of the target screen. If a size is provided, the form's client area is resized
        /// before positioning. Use this method to ensure forms appear centered on multi-monitor setups.</remarks>
        /// <param name="form">The form to be centered. Cannot be null.</param>
        /// <param name="targetScreen">The screen on which to center the form. Cannot be null.</param>
        /// <param name="size">An optional size to set for the form before centering. If specified, the form will be resized to this value.</param>
        public static void CenterFormOnScreen(this Form form, Screen targetScreen, Size? size = null)
        {
            if (size != null)
            {
                form.ClientSize = (Size)size;
            }

            // Get the bounds of the target screen
            var screenBounds = targetScreen.Bounds;

            // Calculate the new position of the form (centered)
            int x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
            int y = screenBounds.Top + (screenBounds.Height - form.Height) / 2;

            // Set the new location without changing the size
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new System.Drawing.Point(x, y);
        }

        /// <summary>
        /// Sets the specified form to full-screen mode on the given monitor, removing window borders and maximizing the
        /// form to occupy the entire screen area.
        /// </summary>
        /// <remarks>This method removes the form's borders and maximizes it to cover the entire area of
        /// the specified monitor. The form will be set to appear in the taskbar and, outside of debug builds, will be
        /// set as topmost. Use this method to ensure the form occupies the full display area of a specific monitor in
        /// multi-monitor setups.</remarks>
        /// <param name="form">The form to be displayed in full-screen mode.</param>
        /// <param name="targetMonitor">The monitor on which the form should be shown in full-screen mode.</param>
        public static void SetFullScreenOnMonitor(this Form form, Screen targetMonitor)
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Normal; // Ensure we can manually set size
            form.StartPosition = FormStartPosition.Manual;
            form.ShowInTaskbar = true;
#if !DEBUG
                form.TopMost = true; //This is a total pain for debugging.
#endif
            // Set the form's location and size to match the target monitor
            form.Bounds = targetMonitor.Bounds;

            // Maximize the form (but ensure no borders)
            form.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Used to determine which screen the control is on.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Screen GetCurrentScreen(this Control control)
        {
            return Screen.FromControl(control);
        }
    }
}
