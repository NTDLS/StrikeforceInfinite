namespace Si.Library.Metadata
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AssetCategoryAttribute : Attribute
    {
        public string FriendlyName { get; }
        public bool UserSelectable { get; }
        public string Description { get; }

        public AssetCategoryAttribute(string friendlyName, string description, bool userSelectable)
        {
            FriendlyName = friendlyName;
            Description = description;
            UserSelectable = userSelectable;
        }
    }
}
