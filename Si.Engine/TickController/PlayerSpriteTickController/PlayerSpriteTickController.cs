﻿using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.PlayerSpriteTickController
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController : PlayerSpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _engine;
        private bool _allowLockPlayerAngleToNearbyEnemy = true;

        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(EngineCore engine)
            : base(engine)
        {
            _engine = engine;
        }

        public void InstantiatePlayerClass(Type playerClassType)
        {
            Sprite.Cleanup();
            Sprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerClassType, new object[] { _engine });
            Sprite.Visable = false;
        }

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public override SiPoint ExecuteWorldClockTick(float epoch)
        {
            var displacementVector = new SiPoint();

            Sprite.IsLockedOnSoft = false;
            Sprite.IsLockedOnHard = false;

            if (Sprite.Visable)
            {
                //Sprite.PrimaryWeapon.ApplyIntelligence();
                Sprite.SelectedSecondaryWeapon?.ApplyIntelligence(epoch);

                if (Engine.Input.IsKeyPressed(SiPlayerKey.PrimaryFire))
                {
                    if (Sprite.PrimaryWeapon != null && Sprite.PrimaryWeapon.Fire())
                    {
                        if (Sprite.PrimaryWeapon?.RoundQuantity == 25)
                        {
                            Sprite.AmmoLowSound.Play();
                        }
                        if (Sprite.PrimaryWeapon?.RoundQuantity == 0)
                        {
                            Sprite.AmmoEmptySound.Play();
                        }
                    }
                }

                if (Engine.Input.IsKeyPressed(SiPlayerKey.SecondaryFire))
                {
                    if (Sprite.SelectedSecondaryWeapon != null && Sprite.SelectedSecondaryWeapon.Fire())
                    {
                        if (Sprite.SelectedSecondaryWeapon?.RoundQuantity == 25)
                        {
                            Sprite.AmmoLowSound.Play();
                        }
                        if (Sprite.SelectedSecondaryWeapon?.RoundQuantity == 0)
                        {
                            Sprite.AmmoEmptySound.Play();
                            Sprite.SelectFirstAvailableUsableSecondaryWeapon();
                        }
                    }
                }

                #region LockPlayerAngleToNearbyEnemy.
                //This needs some work. It works, but its odd - the movement is rigid.

                if (Engine.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    if (Engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise) == false
                        && Engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = Engine.Sprites.Enemies.Visible();
                            foreach (var enemy in enemies)
                            {
                                var distanceTo = Sprite.DistanceTo(enemy);
                                if (distanceTo < 500)
                                {
                                    if (Sprite.IsPointingAt(enemy, 50))
                                    {
                                        if (enemy.IsPointingAway(Sprite, 50))
                                        {
                                            var angleTo = Sprite.AngleTo360(enemy);
                                            Sprite.Velocity.Angle.Degrees = angleTo;
                                        }
                                    }
                                }
                            }
                            _allowLockPlayerAngleToNearbyEnemy = false;
                        }
                    }
                    else
                    {
                        _allowLockPlayerAngleToNearbyEnemy = true;
                    }
                }
                #endregion

                #region Forward and Reverse.

                float forwardThrustToAdd = Sprite.Velocity.ForwardMomentium == 0 ? Engine.Settings.PlayerThrustRampUp
                    : Engine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ForwardMomentium);

                //Make player forward momentium "build up" and fade-out.
                if (Engine.Input.IsKeyPressed(SiPlayerKey.Forward))
                {
                    Sprite.Velocity.ForwardMomentium = (Sprite.Velocity.ForwardMomentium + forwardThrustToAdd).Clamp(-1, 1);
                }
                else if (Engine.Input.IsKeyPressed(SiPlayerKey.Reverse))
                {
                    Sprite.Velocity.ForwardMomentium = (Sprite.Velocity.ForwardMomentium - forwardThrustToAdd).Clamp(-1, 1);
                }
                else
                {
                    float thrustToRemove = Sprite.Velocity.ForwardMomentium == 0 ? Engine.Settings.PlayerThrustRampDown
                        : Engine.Settings.PlayerThrustRampDown * Sprite.Velocity.ForwardMomentium;

                    if (Math.Abs(thrustToRemove) >= Math.Abs(Sprite.Velocity.ForwardMomentium))
                    {
                        Sprite.Velocity.ForwardMomentium = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardMomentium -= thrustToRemove;
                }

                #endregion

                #region Forward Boost.

                float forwardBoostThrustToAdd = Sprite.Velocity.ForwardBoostMomentium == 0 ? Engine.Settings.PlayerThrustRampUp
                    : Engine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ForwardBoostMomentium);

                //Make player forward momentium "build up" and fade-out.
                if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) && Engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false)
                {
                    Sprite.Velocity.ForwardBoostMomentium = (Sprite.Velocity.ForwardBoostMomentium + forwardBoostThrustToAdd).Clamp(-1, 1);

                    Sprite.Velocity.AvailableBoost -= Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostMomentium;
                    if (Sprite.Velocity.AvailableBoost < 0)
                    {
                        Sprite.Velocity.AvailableBoost = 0;
                    }

                }
                else
                {
                    float thrustToRemove = Sprite.Velocity.ForwardBoostMomentium == 0 ? Engine.Settings.PlayerThrustRampDown
                        : Engine.Settings.PlayerThrustRampDown * Sprite.Velocity.ForwardBoostMomentium;

                    if (Math.Abs(thrustToRemove) + 0.1 >= Math.Abs(Sprite.Velocity.ForwardBoostMomentium))
                    {
                        Sprite.Velocity.ForwardBoostMomentium = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardBoostMomentium -= thrustToRemove;

                    if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) == false
                        && Sprite.Velocity.AvailableBoost < Engine.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 1000.0f * epoch / 1000.0f).Clamp(0, Engine.Settings.MaxPlayerBoostAmount);

                        if (Sprite.Velocity.IsBoostCoolingDown && Sprite.Velocity.AvailableBoost >= Engine.Settings.PlayerBoostRebuildFloor)
                        {
                            Sprite.Velocity.IsBoostCoolingDown = false;
                        }
                    }
                }

                #endregion

                #region Strafing.

                var strafeAngle = SiPoint.PointFromAngleAtDistance360(new SiAngle(Sprite.Velocity.Angle - SiPoint.RADIANS_90), new SiPoint(1, 1));

                float strafeThrustToAdd = Sprite.Velocity.LateralMomentium == 0 ? Engine.Settings.PlayerThrustRampUp
                    : Engine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.LateralMomentium);

                //Make player lateral momentium "build up" and fade-out.
                if (Engine.Input.IsKeyPressed(SiPlayerKey.StrafeLeft) && !Engine.Input.IsKeyPressed(SiPlayerKey.StrafeRight))
                {
                    Sprite.Velocity.LateralMomentium = (Sprite.Velocity.LateralMomentium + strafeThrustToAdd).Clamp(-1, 1);
                }
                else if (!Engine.Input.IsKeyPressed(SiPlayerKey.StrafeLeft) && Engine.Input.IsKeyPressed(SiPlayerKey.StrafeRight))
                {
                    Sprite.Velocity.LateralMomentium = (Sprite.Velocity.LateralMomentium - strafeThrustToAdd).Clamp(-1, 1);
                }
                else //Ramp down to a stop:
                {
                    float thrustToRemove = Sprite.Velocity.LateralMomentium == 0 ? Engine.Settings.PlayerThrustRampDown
                        : Engine.Settings.PlayerThrustRampDown * Sprite.Velocity.LateralMomentium;

                    if (Math.Abs(thrustToRemove) >= Math.Abs(Sprite.Velocity.LateralMomentium))
                    {
                        Sprite.Velocity.LateralMomentium = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.LateralMomentium -= thrustToRemove;
                }

                #endregion 

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.IsBoostCoolingDown = true;
                }

                var totalForwardThrust =
                    Sprite.Velocity.MaximumSpeed * Sprite.Velocity.ForwardMomentium
                    + Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostMomentium;

                displacementVector +=
                    Sprite.Velocity.Angle * totalForwardThrust +
                    strafeAngle * Sprite.Velocity.MaximumSpeed * Sprite.Velocity.LateralMomentium;

                //We are going to restrict the rotation speed to a percentage of momentium.
                var rotationSpeed = Engine.Settings.MaxPlayerRotationSpeedDegrees
                    * ((Sprite.Velocity.LateralMomentium + Sprite.Velocity.ForwardMomentium) / 2);

                if (Engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) && !Engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0f));
                }
                if (!Engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) && Engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0f);
                }

                #region Sounds and Animation.

                if (Sprite.Velocity.ForwardBoostMomentium > 0.1)
                {
                    Sprite.ShipEngineBoostSound.Play();
                }
                else
                {
                    Sprite.ShipEngineBoostSound.Fade();
                }

                if (Sprite.Velocity.ForwardMomentium > 0.1)
                {
                    Sprite.ShipEngineRoarSound.Play();
                }
                else
                {
                    Sprite.ShipEngineRoarSound.Fade();
                }

                if (Sprite.ThrustAnimation != null)
                {
                    Sprite.ThrustAnimation.Visable = Engine.Input.IsKeyPressed(SiPlayerKey.Forward);
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                        && Engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false;
                }

                #endregion
            }

            //Scroll the background.
            Engine.Display.RenderWindowPosition += displacementVector;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.Location += displacementVector;

            Sprite.RenewableResources.RenewAllResources(epoch);

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            Engine.Sprites.PlayerStatsText.Visable = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Show()
        {
            Engine.Sprites.PlayerStatsText.Visable = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Engine.Sprites.PlayerStatsText.Visable = false;
            Engine.Sprites.RenderRadar = false;
            Sprite.Visable = false;
            Sprite.ShipEngineIdleSound.Stop();
            Sprite.ShipEngineRoarSound.Stop();
        }
    }
}