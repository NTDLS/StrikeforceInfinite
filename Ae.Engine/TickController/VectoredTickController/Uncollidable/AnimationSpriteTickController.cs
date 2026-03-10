using Ae.Engine.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Animation;
using Ae.Engine.Sprite.Base;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class AnimationSpriteTickController
        : VectoredTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyMotion(epoch, cameraDisplacement);
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
            Add(assetKeys.OneOf(), (sprite) =>
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
            Add(assetKeys.OneOf(), (sprite) =>
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
            Add(assetKeys.OneOf(), (sprite) =>
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
            Add(assetKeys.OneOf(), (sprite) =>
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
            Add(assetKeys.OneOf(), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }
    }
}
