﻿using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite.Weapon;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyStarbaseGarrisonTurret : SpriteAttachment
    {
        public bool FireToggler { get; set; }

        public SpriteEnemyStarbaseGarrisonTurret(EngineCore engine)
            : base(engine)
        {
            InitializeSpriteFromMetadata($@"Sprites\Enemy\Starbase\Garrison\Turret.png");
            TakesDamage = true;
            SetHullHealth(10);
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            // Since the turret.BaseLocation is relative to the top-left corner of the base sprite, we need
            // to get the position relative to the center of the base sprite image so that we can rotate around that.
            var turretOffset = LocationRelativeToOwner - (OwnerSprite.Size / 2.0f);

            // Apply the rotated offsets to get the new turret location relative to the base sprite center.
            Location = OwnerSprite.Location + turretOffset.Rotate(OwnerSprite.Velocity.ForwardAngle.Radians);

            if (DistanceTo(_engine.Player.Sprite) < 1000)
            {
                //Rotate the turret toward the player.
                var deltaAngltToPlayer = DeltaAngleDegrees(_engine.Player.Sprite);
                if (deltaAngltToPlayer < 1)
                {
                    Velocity.ForwardAngle.Degrees -= 0.25f;
                }
                else if (deltaAngltToPlayer > 1)
                {
                    Velocity.ForwardAngle.Degrees += 0.25f;
                }

                if (deltaAngltToPlayer.IsBetween(-10, 10))
                {
                    if (FireToggler)
                    {
                        var pointRight = Location + SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(21, 21));
                        FireToggler = !FireWeapon<WeaponLancer>(pointRight);
                    }
                    else
                    {
                        var pointLeft = Location + SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(21, 21));
                        FireToggler = FireWeapon<WeaponLancer>(pointLeft);
                    }
                }
            }
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
