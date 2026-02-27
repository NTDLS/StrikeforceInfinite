using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Peon._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in their functionality and animations.
    /// </summary>
    public class SpriteEnemyPeonBase
        : SpriteEnemyBase
    {
        public SpriteAnimation ThrusterAnimation { get; internal set; }
        public SpriteAnimation BoosterAnimation { get; internal set; }

        public SpriteEnemyPeonBase(EngineCore engine, string spritePath)
            : base(engine, spritePath)
        {
            RecalculateMovementVectorFromOrientation();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            ThrusterAnimation = _engine.Sprites.Animations.Add(@"Sprites\Animation\ThrustStandard32x32.png", (o) =>
            {
                o.Location = Location.Clone();
                o.Orientation = Orientation.Clone();
                o.IsVisible = true;
                o.OwnerUID = UID;
            });

            BoosterAnimation = _engine.Sprites.Animations.Add(@"Sprites\Animation\ThrustBoost32x32.png", (o) =>
            {
                o.Location = Location.Clone();
                o.Orientation = Orientation.Clone();
                o.IsVisible = true;
                o.OwnerUID = UID;
            });

            UpdateThrustAnimationPositions();
        }

        public override void LocationChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            var pointBehind = (Orientation * -1) * new SiVector(20, 20);

            if (ThrusterAnimation != null && ThrusterAnimation.IsVisible)
            {
                ThrusterAnimation.Orientation = Orientation;
                ThrusterAnimation.Location = Location + pointBehind;
            }
            if (BoosterAnimation != null && BoosterAnimation.IsVisible)
            {
                BoosterAnimation.Orientation = Orientation;
                BoosterAnimation.Location = Location + pointBehind;
            }
        }

        private void EnemyBase_OnVisibilityChanged(SpriteBase sender)
        {
            if (ThrusterAnimation != null)
            {
                ThrusterAnimation.IsVisible = false;
            }
            if (BoosterAnimation != null)
            {
                BoosterAnimation.IsVisible = false;
            }
        }

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity).
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);

            if (ThrusterAnimation != null)
            {
                ThrusterAnimation.IsVisible = MovementVector.Sum() > 0;
            }
            if (BoosterAnimation != null)
            {
                BoosterAnimation.IsVisible = Throttle > 0;
            }
        }

        public override void Cleanup()
        {
            ThrusterAnimation?.QueueForDelete();
            BoosterAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}
