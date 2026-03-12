using Ae.Engine.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the ticking and animation updates for sprite-based animations within the game world.
    /// </summary>
    /// <remarks>This controller manages the timing and progression of animated sprites, including applying
    /// motion and advancing animation frames. It also provides methods to insert new animations and trigger various
    /// explosion effects at specified sprite locations. Use this class to coordinate sprite animation updates in
    /// response to world clock ticks and to add visual effects such as explosions. Thread safety is not guaranteed;
    /// ensure that calls are made from the appropriate game loop context.</remarks>
    public class AnimationSpriteTickController
        : VectoredTickControllerBase<AeSpriteAnimation>
    {
        /// <summary>
        /// Initializes a new instance of the AnimationSpriteTickController class using the specified engine and sprite
        /// manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides core functionality and services for animation processing.</param>
        /// <param name="manager">The sprite manager responsible for managing sprite objects used by the controller.</param>
        public AnimationSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Advances the state of all visible sprites for the current world clock tick, applying motion and updating
        /// their images.
        /// </summary>
        /// <remarks>This method also records multiplayer motion actions for each sprite if a multiplayer
        /// lobby is active.</remarks>
        /// <param name="epoch">The current time value, in seconds, representing the world clock tick to process.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite motion during this tick.</param>
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
        public void Insert(AeSpriteAnimation animation, AeSprite defaultPosition)
        {
            animation.Location = defaultPosition.Location.Clone();
            SpriteManager.Insert(animation);
        }

        /// <summary>
        /// Very small fiery explosion.
        /// </summary>
        public void AddRandomMicroFireExplosionAt(AeSprite positionOf)
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
        public void AddRandomSmallFireExplosionAt(AeSprite positionOf)
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
        public void AddRandomMediumFireExplosionAt(AeSprite positionOf)
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
        public void AddRandomLargeFireExplosionAt(AeSprite positionOf)
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
        public void AddRandomEnergyExplosionAt(AeSprite positionOf)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Animation/Explode/Energy Explosions");
            Add(assetKeys.OneOf(), (sprite) =>
            {
                sprite.Location = positionOf.Location.Clone();
            });
        }
    }
}
