using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class AnimationSpriteTickController : VectoredTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.AdvanceImage(epoch);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        /// <summary>
        /// Creates an animation on top of another sprite.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="defaultPosition"></param>
        public void Insert(SpriteAnimation animation, SpriteBase defaultPosition)
        {
            animation.Location = defaultPosition.Location.Clone();
            SpriteManager.Insert(animation);
        }

        /// <summary>
        /// Very small fiery explosion.
        /// </summary>
        public void AddRandomMicroFireExplosionAt(SpriteBase positionOf)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Animation/Explode/Micro Fire Explosions");
            Add(SiRandom.OneOf(assetKeys), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }

        /// <summary>
        /// Small fiery explosion.
        /// </summary>
        public void AddRandomSmallFireExplosionAt(SpriteBase positionOf)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Animation/Explode/Small Fire Explosions");
            Add(SiRandom.OneOf(assetKeys), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }

        /// <summary>
        /// Medium fiery explosion.
        /// </summary>
        /// <param name="PositionOf"></param>
        public void AddRandomMediumFireExplosionAt(SpriteBase positionOf)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Animation/Explode/Medium Fire Explosions");
            Add(SiRandom.OneOf(assetKeys), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }

        /// <summary>
        /// Somewhat large fiery explosion.
        /// </summary>
        /// <param name="PositionOf"></param>
        public void AddRandomLargeFireExplosionAt(SpriteBase positionOf)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Animation/Explode/Large Fire Explosions");
            Add(SiRandom.OneOf(assetKeys), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }

        /// <summary>
        /// Fairly large colorful energy-looking explosions.
        /// </summary>
        public void AddRandomEnergyExplosionAt(SpriteBase positionOf)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Animation/Explode/Energy Explosions");
            Add(SiRandom.OneOf(assetKeys), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }
    }
}
