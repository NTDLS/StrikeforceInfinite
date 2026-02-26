namespace Si.AssetExplorer
{
    public static class Constants
    {
        public const string AppName = "Si.AssetExplorer";
    }

    public enum PropertyEditorType
    {
        Readonly,
        String,
        Text,
        Integer,
        FloatingPoint,
        Boolean,
        Range,
        Enum, //Can this be combined with Picker
        Picker //Can this be combined with Enum
    }

    public static class AssetExplorerColors
    {
        public static readonly Color Default = Color.Black;
        public static readonly Color Verbose = Color.CornflowerBlue;
        public static readonly Color Information = Color.DarkGoldenrod;
        public static readonly Color Warning = Color.Goldenrod;
        public static readonly Color Error = Color.DarkRed;
    }

    public enum LoggingLevel
    {
        Default,
        Verbose,
        Information,
        Warning,
        Error
    }

    public enum SiTreeNodeType
    {
        Undefined,
        Folder,
        Asset
    }
}
