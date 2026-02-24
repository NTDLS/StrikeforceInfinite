using Si.Engine.AI._Superclass;
using Si.Engine.AI.Logistics;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System.Linq;

namespace Si.Engine.Sprite.Enemy.Peon
{
    public class SpriteEnemyPhoenix
        : SpriteEnemyPeonBase
    {
        public SpriteEnemyPhoenix(EngineCore engine)
            : base(engine, @"Sprites\Enemy\Peon\Phoenix.png")
        {
            AddAIController(new AILogisticsOffScreenReentry(_engine, this));
            //AddAIController(new AILogisticsHostileEngagement(_engine, this, [_engine.Player.Sprite]));
            //AddAIController(new AILogisticsTaunt(_engine, this, [_engine.Player.Sprite]));
            //AddAIController(new AILogisticsMeander(_engine, this, [_engine.Player.Sprite]));

            SetCurrentAIController<AILogisticsOffScreenReentry>();
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            //RotateMovementVector(45, epoch);
            //RotateOrientation(45, epoch);

            base.ApplyIntelligence(epoch, displacementVector);
            ApplyWeaponsLogic();
        }

        private void ApplyWeaponsLogic()
        {
            var playersIAmPointingAt = GetPointingAtOf(_engine.Sprites.AllVisiblePlayers, 2.0f);
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
    }
}
