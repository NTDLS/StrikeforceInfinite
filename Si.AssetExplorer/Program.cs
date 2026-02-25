using Krypton.Toolkit;

namespace Si.AssetExplorer
{
    internal static class Program
    {
        public static KryptonManager ThemeManager = new();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            var mutex = new Mutex(true, Constants.AppName, out var createdNewMutex);
            if (!createdNewMutex)
            {
                MessageBox.Show("Another instance is already running.", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            Settings.Save(); //Create a default persisted state if one does not exist.


            ThemeManager.GlobalPaletteMode = Settings.Instance.Theme;

            Application.Run(new FormMain());

        }
    }
}