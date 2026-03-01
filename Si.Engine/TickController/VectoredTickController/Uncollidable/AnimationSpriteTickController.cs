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

        /*
        public SpriteAnimation Add(string imageFrames)
        {
            SpriteAnimation obj = new SpriteAnimation(Engine, imageFrames);
            SpriteManager.Add(obj);
            return obj;
        }
        */

        /// <summary>
        /// Very small fiery explosion.
        /// </summary>
        public void AddRandomMicroFireExplosionAt(SpriteBase positionOf)
        {
            const string assetPath = "Sprites/Animation/Explode/Micro Fire Explosions";
            int assetCount = 4;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Add($"{assetPath}/{selectedAssetIndex}");
            animation.Location = positionOf.Location.Clone();
        }

        /// <summary>
        /// Small fiery explosion.
        /// </summary>
        public void AddRandomSmallFireExplosionAt(SpriteBase positionOf)
        {
            const string assetPath = "Sprites/Animation/Explode/Small Fire Explosions";
            int assetCount = 2;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Add($"{assetPath}/{selectedAssetIndex}");
            animation.Location = positionOf.Location.Clone();
        }

        /// <summary>
        /// Medium fiery explosion.
        /// </summary>
        /// <param name="PositionOf"></param>
        public void AddRandomMediumFireExplosionAt(SpriteBase PositionOf)
        {
            const string assetPath = "Sprites/Animation/Explode/Medium Fire Explosions";
            int assetCount = 7;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Add($"{assetPath}/{selectedAssetIndex}");
            animation.Location = PositionOf.Location.Clone();
        }

        /// <summary>
        /// Somewhat large fiery explosion.
        /// </summary>
        /// <param name="PositionOf"></param>
        public void AddRandomLargeFireExplosionAt(SpriteBase PositionOf)
        {
            const string assetPath = "Sprites/Animation/Explode/Large Fire Explosions";
            int assetCount = 7;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Add($"{assetPath}/{selectedAssetIndex}");
            animation.Location = PositionOf.Location.Clone();
        }

        /// <summary>
        /// Fairly large colorful energy-looking explosions.
        /// </summary>
        public void AddRandomEnergyExplosionAt(SpriteBase PositionOf)
        {
            const string assetPath = "Sprites/Animation/Explode/Energy Explosions";
            int assetCount = 8;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Add($"{assetPath}/{selectedAssetIndex}");
            animation.Location = PositionOf.Location.Clone();
        }
    }
}
