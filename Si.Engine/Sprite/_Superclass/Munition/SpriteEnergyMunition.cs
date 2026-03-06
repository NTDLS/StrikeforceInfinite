using Si.Engine.Sprite._Superclass.Interactive;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    internal class SpriteEnergyMunition
        : SpriteMunition
    {
        public SpriteEnergyMunition(EngineCore engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
