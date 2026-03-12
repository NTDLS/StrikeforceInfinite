using System;

namespace Ae.Engine.Metadata
{
    /// <summary>
    /// Specifies metadata for an asset class, including its display name, description, applicability, and whether it
    /// can be selected by users.
    /// </summary>
    /// <remarks>Apply this attribute to a class to provide asset classification information used for display,
    /// filtering, and selection in asset management systems. Only one instance of this attribute can be applied to a
    /// class.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AssetClassAttribute
        : Attribute
    {
        /// <summary>
        /// Gets the user-friendly name associated with the object.
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Gets a value indicating whether this item can be selected by the user.
        /// </summary>
        public bool UserSelectable { get; }

        /// <summary>
        /// Gets the textual description associated with the current instance.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets or sets the asset type to which this configuration applies.
        /// </summary>
        public AeBaseAssetType ApplicableTo { get; set; }

        /// <summary>
        /// Initializes a new instance of the AssetClassAttribute class with the specified friendly name, description,
        /// applicable asset type, and user selection option.
        /// </summary>
        /// <param name="friendlyName">The display name used to represent the asset class in user interfaces.</param>
        /// <param name="description">A brief description of the asset class, providing context or details for users.</param>
        /// <param name="applicableTo">The asset type to which this asset class attribute applies.</param>
        /// <param name="userSelectable">A value indicating whether the asset class can be selected by users. Set to <see langword="true"/> if user
        /// selection is allowed; otherwise, <see langword="false"/>.</param>
        public AssetClassAttribute(string friendlyName, string description, AeBaseAssetType applicableTo, bool userSelectable)
        {
            FriendlyName = friendlyName;
            Description = description;
            ApplicableTo = applicableTo;
            UserSelectable = userSelectable;
        }
    }
}
