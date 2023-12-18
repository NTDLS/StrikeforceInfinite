﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Loudouts;
using NebulaSiege.Game.Sprites.Enemies.Bosses.BaseClasses;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Utility.ExtensionMethods;
using NebulaSiege.Game.Weapons;

namespace NebulaSiege.Game.Sprites.Enemies.Bosses
{
    /// <summary>
    /// 100% Experimental
    /// </summary>
    internal class SpriteEnemyDevastator : SpriteEnemyBossBase
    {
        public const int hullHealth = 100;
        public const int bountyMultiplier = 15;

        private readonly SpriteAttachment _leftCannon;
        private readonly SpriteAttachment _rightCannon;

        private SpriteAttachment _turret;

        private readonly SpriteAttachment _leftGun;
        private readonly SpriteAttachment _rightGun;

        private readonly SpriteAttachment _rightThrust;
        private readonly SpriteAttachment _leftThrust;

        private readonly double _initialMaxpeed;

        readonly string _imagesPath = @"Graphics\Enemy\Devastator\";

        public SpriteEnemyDevastator(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            _leftCannon = Attach(_imagesPath + "Gun.Cannon.Left.png", true, 3);
            _rightCannon = Attach(_imagesPath + "Gun.Cannon.Right.png", true, 3);
            _leftGun = Attach(_imagesPath + "Gun.Left.png", true, 3);
            _rightGun = Attach(_imagesPath + "Gun.Right.png", true, 3);
            _leftThrust = Attach(_imagesPath + "Jet.Left.png", true, 3);
            _rightThrust = Attach(_imagesPath + "Jet.Right.png", true, 3);

            SetImage(_imagesPath + "Hull.png");

            ShipClass = HgEnemyClass.Devastator;

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Devastator ←\n"
                       + "TODO: Add a description\n",
                    MaxSpeed = 3.5,
                    MaxBoost = 1.5,
                    HullHealth = 2500,
                    ShieldHealth = 3000,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            _initialMaxpeed = Velocity.MaxSpeed;
        }

        public override void AfterCreate()
        {
            _turret = Attach(_imagesPath + "Gun.Turret.png", true, 3);
            base.AfterCreate();
        }

        public override void VelocityChanged()
        {
            if (_leftThrust != null && _rightThrust != null)
            {
                bool visibleThrust = Velocity.ThrottlePercentage > 0;

                if (_leftThrust.IsDead == false)
                {
                    _leftThrust.Visable = visibleThrust;
                }
                if (_rightThrust.IsDead == false)
                {
                    _rightThrust.Visable = visibleThrust;
                }
            }
        }

        public override void PositionChanged()
        {
            if (_leftGun != null && _rightGun != null)
            {
                if (_leftGun.IsDead == false)
                {
                    var pointLeft = HgMath.PointFromAngleAtDistance360(Velocity.Angle - 90, new NsPoint(25, 25));
                    _leftGun.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _leftGun.X = X + pointLeft.X;
                    _leftGun.Y = Y + pointLeft.Y;
                }

                if (_rightGun.IsDead == false)
                {
                    var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle + 90, new NsPoint(25, 25));
                    _rightGun.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _rightGun.X = X + pointRight.X;
                    _rightGun.Y = Y + pointRight.Y;
                }

                if (_leftThrust.IsDead == false)
                {
                    var pointLeft = HgMath.PointFromAngleAtDistance360(Velocity.Angle - 135, new NsPoint(35, 35));
                    _leftThrust.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _leftThrust.X = X + pointLeft.X;
                    _leftThrust.Y = Y + pointLeft.Y;
                }

                if (_rightThrust.IsDead == false)
                {
                    var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle + 135, new NsPoint(35, 35));
                    _rightThrust.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _rightThrust.X = X + pointRight.X;
                    _rightThrust.Y = Y + pointRight.Y;
                }

                if (_turret.IsDead == false)
                {
                    var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle, new NsPoint(0, 0));
                    _turret.Velocity.Angle.Degrees = AngleTo360(_core.Player.Sprite);
                    _turret.X = X + pointRight.X;
                    _turret.Y = Y + pointRight.Y;
                }
            }
        }

        #region Artificial Intelligence.

        private enum AIMode
        {
            Approaching,
            Tailing,
            MovingToFallback,
            MovingToApproach,
            LameDuck
        }

        private const double baseDistanceToKeep = 200;
        private double distanceToKeep = baseDistanceToKeep * (HgRandom.Generator.NextDouble() + 1);
        private const double baseFallbackDistance = 800;
        private double fallbackDistance;
        private NsAngle fallToAngle;
        private AIMode mode = AIMode.Approaching;
        private int roundsToFireBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = HgMath.DistanceTo(this, _core.Player.Sprite);

            //We have no engines. :(
            if (_leftThrust.IsDead && _rightThrust.IsDead)
            {
                mode = AIMode.LameDuck;
            }

            //If we get down to one engine, slowly cut the max thrust to half of what it originally was. If we lose both, reduce it to 1.
            int thrustHandicap = (_leftThrust.IsDead ? 0 : 1) + (_rightThrust.IsDead ? 0 : 1);
            if (thrustHandicap == 1 && Velocity.MaxSpeed > _initialMaxpeed / 2)
            {
                Velocity.MaxSpeed -= 0.5;
            }
            if (thrustHandicap == 0 && Velocity.MaxSpeed > 1)
            {
                Velocity.MaxSpeed -= 0.5;
                if (Velocity.MaxSpeed < 1)
                {
                    Velocity.MaxSpeed = 1;
                }
            }

            if (mode == AIMode.LameDuck)
            {
                if (distanceToPlayer > 2500)
                {
                    Explode();
                }

                //Keep pointing at the player.
                var deltaAngle = DeltaAngle(_core.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.Angle -= 1;
                    }
                }

                //Try to stay close.
                if (distanceToPlayer > 300)
                {
                    Velocity.ThrottlePercentage += 0.05;
                    if (Velocity.ThrottlePercentage > 1)
                    {
                        Velocity.ThrottlePercentage = 1;
                    }
                }
                else
                {
                    //Slow to a stop when close.
                    Velocity.ThrottlePercentage -= 0.05;
                    if (Velocity.ThrottlePercentage < 0)
                    {
                        Velocity.ThrottlePercentage = 0;
                    }
                }
            }
            else if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    PointAtAndGoto(_core.Player.Sprite);
                }
                else
                {
                    mode = AIMode.Tailing;
                    roundsToFireBeforeTailing = 15;
                    hpRemainingBeforeTailing = HullHealth;
                }
            }

            if (mode == AIMode.Tailing)
            {
                PointAtAndGoto(_core.Player.Sprite);

                //Stay on the players tail.
                if (distanceToPlayer > distanceToKeep + 300)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.Approaching;
                }
                else
                {
                    Velocity.ThrottlePercentage -= 0.05;
                    if (Velocity.ThrottlePercentage < 0)
                    {
                        Velocity.ThrottlePercentage = 0;
                    }
                }

                //We we get too close, do too much damage or they fire at us enough, they fall back and come in again
                if (distanceToPlayer < distanceToKeep / 2.0
                    || hpRemainingBeforeTailing - HullHealth > 2
                    || roundsToFireBeforeTailing <= 0)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + HgRandom.Between(0, 10));
                    fallbackDistance = baseFallbackDistance * (HgRandom.Generator.NextDouble() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.Angle - fallToAngle;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 1;
                    }
                }

                if (distanceToPlayer > fallbackDistance)
                {
                    mode = AIMode.MovingToApproach;
                }
            }

            if (mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngle(_core.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.Angle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (HgRandom.Generator.NextDouble() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000 && (_rightGun?.IsDead == false || _leftGun?.IsDead == false))
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Sprite, 2.0);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponDualVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Sprite, 2.0);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}