using Ae.Engine.DataModels;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive;
using System;
using System.Diagnostics;

namespace Ae.Engine.TickController.PlayerSpriteTickController
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController
        : PlayerSpriteTickControllerBase<AeSpritePlayer>
    {
        private readonly AeEngine _engine;
        private readonly Stopwatch _inputDelay = new();

        /// <summary>
        /// Gets or sets the statistics associated with the player.
        /// </summary>
        public PlayerStats Stats { get; set; } = new(); //This should be saved.

        /// <summary>
        /// Gets or sets the sprite player used to render animated sprites.
        /// </summary>
        public AeSpritePlayer Sprite { get; set; }

        /// <summary>
        /// Initializes a new instance of the PlayerSpriteTickController class using the specified engine.
        /// </summary>
        /// <remarks>The player sprite is created and managed based on the engine's execution mode. In
        /// play mode, the sprite is added to the engine's collection and is initially invisible. In edit mode, a
        /// visible placeholder sprite is created but not added to the collection.</remarks>
        /// <param name="engine">The engine instance used to manage game state and sprite operations. Cannot be null.</param>
        public PlayerSpriteTickController(AeEngine engine)
            : base(engine)
        {
            Sprite = new AeSpritePlayer(engine); //We want to make sure this is never null.

            engine.OnInitializationComplete += (AeEngine engine) =>
            {
                //This is where the player is created.
                if (engine.ExecutionMode == AeEngineExecutionMode.Play
                    || engine.ExecutionMode == AeEngineExecutionMode.AttachedDebugging)
                {
                    Sprite = engine.Sprites.Add<AeSpritePlayer>("Sprites/Player/Ships/Debug", (o) =>
                    {
                        o.IsVisible = false;
                    });
                }
                else
                {
                    // In edit mode, the player is just a placeholder and is not added to the collection.
                    Sprite = engine.Sprites.Create<AeSpritePlayer>("Sprites/Player/Ships/Debug", (o) =>
                    {
                        o.IsVisible = true;
                    });
                }
            };

            _engine = engine;
            _inputDelay.Restart();
        }

        /// <summary>
        /// Replaces the current player sprite with a new instance using the specified asset key.
        /// </summary>
        /// <remarks>This method removes the existing player sprite from the collection and adds a new one
        /// based on the provided asset key. The new sprite is initially invisible. Use this method to update or reset
        /// the player sprite during gameplay.</remarks>
        /// <param name="assetKey">The key identifying the asset to use for the new player sprite. Cannot be null or empty.</param>
        public void InstantiatePlayerClass(string assetKey)
        {
            //Remove the player from the sprite collection.
            Sprite.QueueForDelete();
            Sprite.Cleanup();
            Sprite = Engine.Sprites.Add<AeSpritePlayer>(assetKey, (o) =>
            {
                o.IsVisible = false;
            });
        }

        private float _forwardVelocity = 0;
        private float _boostForwardVelocity = 0;
        private float _lateralVelocity = 0;

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public override AeVector ExecuteWorldClockTick(float epoch)
        {
            if (_engine.ExecutionMode == AeEngineExecutionMode.Edit)
            {
                //We don't want the player to move at all in edit mode, so just return a zero vector.
                //Otherwise this can also cause micro-changes to the camera position.
                return AeVector.Zero();
            }

            Sprite.IsLockedOnSoft = false;
            Sprite.IsLockedOn = false;

            if (Sprite.IsVisible)
            {
                #region Weapons Selection and Fire.

                if (Engine.Input.IsKeyPressed(AePlayerKey.SwitchWeaponLeft))
                {
                    if (_inputDelay.ElapsedMilliseconds > 200)
                    {
                        _engine.Player?.Sprite?.SelectPreviousAvailableUsableSecondaryWeapon();
                        _inputDelay.Restart();
                    }
                }
                if (Engine.Input.IsKeyPressed(AePlayerKey.SwitchWeaponRight))
                {
                    if (_inputDelay.ElapsedMilliseconds > 200)
                    {
                        _engine.Player?.Sprite?.SelectNextAvailableUsableSecondaryWeapon();
                        _inputDelay.Restart();
                    }
                }

                Sprite.SelectedSecondaryWeapon?.ApplyIntelligence(epoch);

                if (Engine.Input.IsKeyPressed(AePlayerKey.PrimaryFire))
                {
                    if (Sprite.PrimaryWeapon != null && Sprite.PrimaryWeapon.Fire())
                    {
                        if (Sprite.PrimaryWeapon?.MunitionQuantity == 25)
                        {
                            Sprite.AmmoLowSound?.Play();
                        }
                        if (Sprite.PrimaryWeapon?.MunitionQuantity == 0)
                        {
                            Sprite.AmmoEmptySound?.Play();
                        }
                    }
                }

                if (Engine.Input.IsKeyPressed(AePlayerKey.SecondaryFire))
                {
                    if (Sprite.SelectedSecondaryWeapon != null && Sprite.SelectedSecondaryWeapon.Fire())
                    {
                        if (Sprite.SelectedSecondaryWeapon?.MunitionQuantity == 25)
                        {
                            Sprite.AmmoLowSound?.Play();
                        }
                        if (Sprite.SelectedSecondaryWeapon?.MunitionQuantity == 0)
                        {
                            Sprite.AmmoEmptySound?.Play();
                            Sprite.SelectFirstAvailableUsableSecondaryWeapon();
                        }
                    }
                }

                #endregion

                // We have to do some creative stuff here since we allow forward/reverse and right/left strafing.
                // No other sprite can strafe, so we're going to make all of this a special case. In the end, the
                //  gathered inputs here are baked into the player sprite's Travel.Velocity just like any other sprite.

                float throttleFloor = 0.01f;
                float throttleCap = 0.70f; // 70% will be considered max throttle in any direction, this is because
                                           //   the combined forward and lateral can only be as much as 0.707 each.
                float velocityRampUp = Engine.Settings.PlayerVelocityRampUp * epoch;
                float velocityRampDown = Engine.Settings.PlayerVelocityRampDown * epoch;

                #region Forward and Reverse.

                float targetForwardAmount = (Engine.Input.GetAnalogAxisValue(AePlayerKey.Reverse, AePlayerKey.Forward) / throttleCap).Clamp(-1, 1);

                if (targetForwardAmount > throttleFloor)
                {
                    if (_forwardVelocity <= targetForwardAmount) //The target forward throttle is more than we have applied: ramp-up.
                    {
                        _forwardVelocity = (_forwardVelocity + velocityRampUp).Clamp(-1, targetForwardAmount); //Make player forward velocity build-up.
                    }
                    else //The target forward throttle is less than we have applied: ramp-down.
                    {
                        _forwardVelocity = (_forwardVelocity - velocityRampDown).Clamp(targetForwardAmount, 1);
                    }
                }
                else if (targetForwardAmount < -throttleFloor)
                {
                    if (_forwardVelocity >= targetForwardAmount) //The target reverse throttle is more than we have applied: ramp-up.
                    {
                        _forwardVelocity = (_forwardVelocity - velocityRampUp).Clamp(targetForwardAmount, 1); //Make player forward velocity build-up.
                    }
                    else //The target reverse throttle is less than we have applied: ramp-down.
                    {
                        _forwardVelocity = (_forwardVelocity + velocityRampDown).Clamp(targetForwardAmount, 1);
                    }
                }
                else //No forward input was received, ramp down the forward velocity.
                {
                    if (Math.Abs(velocityRampDown) >= Math.Abs(_forwardVelocity))
                    {
                        _forwardVelocity = 0; //Don't overshoot the stop.
                    }
                    else _forwardVelocity -= _forwardVelocity > 0 ? velocityRampDown : -velocityRampDown;
                }

                #endregion

                #region Forward Speed-Boost.

                if (Engine.Input.IsKeyPressed(AePlayerKey.SpeedBoost)
                    && _forwardVelocity >= throttleFloor
                    && Sprite.RenewableResources.Observe(Sprite.BoostResourceName) > 0)
                {
                    var boostAmount = Sprite.RenewableResources.Consume(Sprite.BoostResourceName, epoch);

                    if (_boostForwardVelocity < (Sprite.MaxThrottle - 1.0f))
                    {
                        _boostForwardVelocity += boostAmount;
                    }
                }
                else if (_boostForwardVelocity > 0)
                {
                    //Ramp down the over-throttle.
                    _boostForwardVelocity -= velocityRampDown;
                }

                _boostForwardVelocity = _boostForwardVelocity.Clamp(0, Sprite.MaxThrottle - 1.0f);

                #endregion

                #region Laterial Strafing.

                float targetLateralAmount = (Engine.Input.GetAnalogAxisValue(AePlayerKey.StrafeLeft, AePlayerKey.StrafeRight) / throttleCap).Clamp(-1, 1);

                if (targetLateralAmount >= throttleFloor) //Strafe right.
                {
                    if (_lateralVelocity <= targetLateralAmount) //The target lateral throttle is more than we have applied: ramp-up.
                    {
                        _lateralVelocity = (_lateralVelocity + velocityRampUp).Clamp(-1, targetLateralAmount); //Make player lateral velocity build-up.
                    }
                    else //The target lateral throttle is less than we have applied: ramp-down.
                    {
                        _lateralVelocity = (_lateralVelocity - velocityRampDown).Clamp(targetLateralAmount, 1);
                    }
                }
                else if (targetLateralAmount <= -throttleFloor) //Strafe left.
                {
                    if (_lateralVelocity >= targetLateralAmount) //The target reverse lateral throttle is more than we have applied: ramp-up.
                    {
                        _lateralVelocity = (_lateralVelocity - velocityRampUp).Clamp(targetLateralAmount, 1); //Make player forward velocity build-up.
                    }
                    else //The target reverse lateral throttle is less than we have applied: ramp-down.
                    {
                        _lateralVelocity = (_lateralVelocity + velocityRampDown).Clamp(targetLateralAmount, 1);
                    }
                }
                else //No lateral input was received, ramp down the lateral velocity.
                {
                    if (Math.Abs(velocityRampDown) >= Math.Abs(_lateralVelocity))
                    {
                        _lateralVelocity = 0; //Don't overshoot the stop.
                    }
                    else _lateralVelocity -= _lateralVelocity > 0 ? velocityRampDown : -velocityRampDown;
                }

                if (_lateralVelocity > 0.8)
                {
                }

                #endregion

                #region Rotation.

                float targetRotationDegrees = (Engine.Input.GetAnalogAxisValue(AePlayerKey.RotateCounterClockwise, AePlayerKey.RotateClockwise) / throttleCap).Clamp(-1, 1);

                Sprite.RotateOrientation(Engine.Settings.MaxPlayerRotationSpeedDegrees * targetRotationDegrees, epoch);

                #endregion

                #region Sounds and Animation.

                if (_boostForwardVelocity > 0)
                    Sprite.ShipEngineBoostSound?.Play();
                else Sprite.ShipEngineBoostSound?.Fade();

                if (_forwardVelocity >= throttleFloor)
                    Sprite.ShipEngineRoarSound?.Play();
                else Sprite.ShipEngineRoarSound?.Fade();

                if (Sprite.ThrusterAnimation != null)
                {
                    Sprite.ThrusterAnimation.IsVisible = (targetForwardAmount >= throttleFloor);
                }

                if (Sprite.BoosterAnimation != null)
                {
                    Sprite.BoosterAnimation.IsVisible =
                        (targetForwardAmount >= throttleFloor)
                        && Engine.Input.IsKeyPressed(AePlayerKey.SpeedBoost)
                        && _boostForwardVelocity > 0
                        && Sprite.RenewableResources.IsCoolingDown(Sprite.BoostResourceName) == false;
                }

                #endregion
            }

            Sprite.RenewableResources.RenewAllResources(epoch);

            Sprite.Throttle = 1 + _boostForwardVelocity;

            Sprite.MovementVector = (Sprite.MakeMovementVectorFromOrientation() * _forwardVelocity) //Forward / Reverse
                + (Sprite.MakeMovementVectorFromAngle(Sprite.Orientation.RadiansSigned + 90.ToRadians()) * _lateralVelocity);  //Lateral strafing.

            Sprite.PerformCollisionDetection(epoch);

            var cameraDisplacement = Sprite.MovementVector * epoch;

            //Scroll the background.
            Engine.Display.CameraPosition += cameraDisplacement;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.Location += cameraDisplacement;

            return cameraDisplacement;
        }

        /// <summary>
        /// Resets the player sprite and displays relevant UI elements and sounds.
        /// </summary>
        /// <remarks>This method makes the player sprite and associated UI elements visible, and plays
        /// status sounds if available. Use this method to reinitialize the player's state and ensure all related
        /// visuals and audio cues are active. Calling this method is typically appropriate after a game reset or when
        /// the player needs to be shown again.</remarks>
        public void ResetAndShow()
        {
            Sprite.Reset();

            Engine.Sprites.TextBlocks.PlayerStatsText.IsVisible = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.IsVisible = true;
            Sprite.ShipEngineIdleSound?.Play();
            Sprite.AllSystemsGoSound?.Play();
        }

        /// <summary>
        /// Displays the player sprite and associated UI elements, enabling relevant sounds and visual components.
        /// </summary>
        /// <remarks>Call this method to make the player sprite visible, show player statistics, render
        /// the radar, and play system activation sounds. This method is typically used when transitioning the player
        /// into an active or visible state within the game.</remarks>
        public void Show()
        {
            Engine.Sprites.TextBlocks.PlayerStatsText.IsVisible = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.IsVisible = true;
            Sprite.ShipEngineIdleSound?.Play();
            Sprite.AllSystemsGoSound?.Play();
        }

        /// <summary>
        /// Hides the player sprite and associated UI elements, disabling their visibility and stopping related sounds.
        /// </summary>
        /// <remarks>Call this method to remove the player sprite and its status display from view, and to
        /// stop any engine sounds. This is typically used when the player should no longer be visible or active in the
        /// game scene.</remarks>
        public void Hide()
        {
            Engine.Sprites.TextBlocks.PlayerStatsText.IsVisible = false;
            Engine.Sprites.RenderRadar = false;
            Sprite.IsVisible = false;
            Sprite.ShipEngineIdleSound?.Stop();
            Sprite.ShipEngineRoarSound?.Stop();
        }
    }
}
