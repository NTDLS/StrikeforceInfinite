namespace Ae.AssetExplorer
{
    internal static class Program
    {
        public static bool NoSplash { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string []args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            NoSplash = args.Any(o => o.Equals("/nosplash", StringComparison.InvariantCultureIgnoreCase));

                using var mutex = new Mutex(true, Constants.AppName, out var createdNewMutex);
            if (!createdNewMutex)
            {
                MessageBox.Show("Another instance is already running.", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            Settings.Save(); //Create a default persisted state if one does not exist.

            //CreateMetaFiles(@"C:\NTDLS\StrikeforceInfinite\Assets");

            Application.Run(new FormMain());
        }

        public static void CreateMetaFiles(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
                throw new DirectoryNotFoundException(rootDirectory);

            foreach (var file in Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories))
            {
                try
                {
                    if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string metaPath = file + ".meta";

                    if (!File.Exists(metaPath))
                    {
                        using (File.Create(metaPath)) { }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed: {file} - {ex.Message}");
                }
            }
        }
    }
}