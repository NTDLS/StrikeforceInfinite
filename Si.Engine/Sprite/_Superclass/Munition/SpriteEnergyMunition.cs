using Si.Engine.Sprite._Superclass.Interactive;
using Si.Library.Mathematics;
using Si.Library.Metadata;

namespace Si.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    [AssetCategory("Munition - Energy Type", "", true)]
    internal class SpriteEnergyMunition
        : SpriteMunition
    {
        public SpriteEnergyMunition(SiEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
