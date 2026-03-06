using Si.Engine.Sprite._Superclass._Root;

namespace Si.Engine.Sprite._Superclass
{
    public class SpriteParticle
        : SpriteBase
    {
        /// <summary>
        /// Used to represent a particle sprite. Note that these are not "fractions" of a larger sprite, they are their
        /// own individual sprites that are designed to be used as particles. They can be used for things like explosions,
        /// smoke, fire, etc. They can also be used for things like debris from destroyed ships, or even just decorative particles like stars or dust.
        /// </summary>
        public SpriteParticle(EngineCore engine, string name = "")
            : base(engine, name)
        {
        }
    }
}
