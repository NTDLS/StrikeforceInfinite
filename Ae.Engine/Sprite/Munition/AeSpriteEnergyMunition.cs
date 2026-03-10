using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    [AssetClass("Munition - Energy Type", "", AeBaseAssetType.Image, true)]
    internal class AeSpriteEnergyMunition
        : AeSpriteMunition
    {
        public AeSpriteEnergyMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
