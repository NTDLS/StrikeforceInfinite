using Ae.Engine.Sprite._Superclass.Interactive;
using Ae.Library.Mathematics;
using Ae.Library.Metadata;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    [AssetClass("Munition - Energy Type", "", AeBaseAssetType.Image, true)]
    internal class SpriteEnergyMunition
        : SpriteMunition
    {
        public SpriteEnergyMunition(AeEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
