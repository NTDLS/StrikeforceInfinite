using System.Reflection;

namespace Ae.AssetExplorer
{
    internal static class Program
    {
        public static bool NoSplash { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
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

            Application.Run(new FormMain());
        }

        private static void PrintAeTypes()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
                })
                .Where(t => (t.IsClass || t.IsInterface) && (t.Name.StartsWith("Ae") || t.Name.StartsWith("IAe")))
                .ToList();

            foreach (var type in types)
            {
                Console.WriteLine(type?.Name);
            }
        }
    }
}
