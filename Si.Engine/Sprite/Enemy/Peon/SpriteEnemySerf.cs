using Si.Engine.AI.Logistics;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library.Mathematics;
using System.Linq;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemySerf(EngineCore engine, string assetKey)
        : SpriteEnemy(engine, assetKey)
    {
        private SpriteAnimation? _thrusterAnimation;
        private SpriteAnimation? _boosterAnimation;

        public override void OnMaterialized()
        {
            RecalculateMovementVectorFromOrientation();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            _thrusterAnimation = Engine.Sprites.Animations.Add("Sprites/Animation/ThrustStandard32x32", (o) =>
            {
                o.Location = Location;
                o.Orientation = Orientation;
                o.IsVisible = true;
                o.OwnerUID = UID;
            });

            _boosterAnimation = Engine.Sprites.Animations.Add("Sprites/Animation/ThrustBoost32x32", (o) =>
            {
                o.Location = Location;
                o.Orientation = Orientation;
                o.IsVisible = true;
                o.OwnerUID = UID;
            });

            UpdateThrustAnimationPositions();

            AddAIController(new AILogisticsOffScreenReentry(Engine, this));
            //AddAIController(new AILogisticsHostileEngagement(_engine, this, [_engine.Player.Sprite]));
            //AddAIController(new AILogisticsTaunt(_engine, this, [_engine.Player.Sprite]));
            //AddAIController(new AILogisticsMeander(_engine, this, [_engine.Player.Sprite]));

            SetCurrentAIController<AILogisticsOffScreenReentry>();
            base.OnMaterialized();
        }

        public override void LocationChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            var pointBehind = (Orientation * -1) * new SiVector(20, 20);

            if (_thrusterAnimation != null && _thrusterAnimation.IsVisible)
            {
                _thrusterAnimation.Orientation = Orientation;
                _thrusterAnimation.Location = Location + pointBehind;
            }
            if (_boosterAnimation != null && _boosterAnimation.IsVisible)
            {
                _boosterAnimation.Orientation = Orientation;
                _boosterAnimation.Location = Location + pointBehind;
            }
        }

        private void EnemyBase_OnVisibilityChanged(SpriteBase sender)
        {
            _thrusterAnimation?.IsVisible = false;
            _boosterAnimation?.IsVisible = false;
        }

        public override void ApplyIntelligence(float epoch, SiVector cameraDisplacement)
        {
            base.ApplyIntelligence(epoch, cameraDisplacement);
            ApplyWeaponsLogic();
        }

        private void ApplyWeaponsLogic()
        {
            var playersIAmPointingAt = GetPointingAtOf(Engine.Sprites.AllVisiblePlayers, 2.0f);
            if (playersIAmPointingAt.Any())
            {
                var closestDistance = ClosestDistanceOf(playersIAmPointingAt);

                if (closestDistance < 1000)
                {
                    if (closestDistance > 500 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        FireWeapon<WeaponVulcanCannon>();
                    }
                    else if (closestDistance > 0 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        FireWeapon<WeaponDualVulcanCannon>();
                    }
                }
            }
        }

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity).
        /// </summary>
        /// <param name="cameraDisplacement"></param>
        public override void ApplyMotion(float epoch, SiVector cameraDisplacement)
        {
            base.ApplyMotion(epoch, cameraDisplacement);

            _thrusterAnimation?.IsVisible = MovementVector.Sum() > 0;
            _boosterAnimation?.IsVisible = Throttle > 1;
        }

        public override void Cleanup()
        {
            _thrusterAnimation?.QueueForDelete();
            _boosterAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}
