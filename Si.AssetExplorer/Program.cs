using Si.Library;

namespace Si.AssetExplorer
{
    internal static class Program
    {
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

            //CreateMetaFiles(@"C:\NTDLS\StrikeforceInfinite\Assets");

            Application.Run(new FormMain());
        }

        public static void CreateMetaFiles(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
                throw new DirectoryNotFoundException(rootDirectory);

            foreach (var file in Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    // Skip files that are already meta files
                    if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string metaPath = file + ".meta";

                    if (!File.Exists(metaPath))
                    {
                        // Create empty JSON file
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