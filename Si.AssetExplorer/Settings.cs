using NTDLS.Persistence;

namespace Si.AssetExplorer
{
    /// <summary>
    /// Settings that are saved to disk.
    /// </summary>
    internal class Settings
    {
        private static Settings? _instance;
        internal static Settings Instance
        {
            get
            {
                _instance ??= LocalUserApplicationData.LoadFromDisk(Constants.AppName, new Settings());
                return _instance;
            }
            set
            {
                _instance = value;
                LocalUserApplicationData.SaveToDisk(Constants.AppName, _instance);
            }
        }

        public static void Save()
        {
            LocalUserApplicationData.SaveToDisk(Constants.AppName, Instance);
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        //public PaletteMode Theme { get; set; } = Theming.IsWindowsDarkMode() ? PaletteMode.SparklePurple : PaletteMode.ProfessionalSystem;
    }
}
