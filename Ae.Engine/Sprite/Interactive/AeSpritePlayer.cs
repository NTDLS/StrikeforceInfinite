using Ae.Engine.Audio;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Munition;
using NTDLS.Helpers;
using System;
using System.Linq;

namespace Ae.Engine.Sprite.Interactive
{
    /// <summary>
    /// The player base is a sub-class of the ship base. It is only used by the Player and as a model for menu selections.
    /// </summary>
    [AssetClass("Bitmap", "", AeBaseAssetType.Image, true)]
    public class AeSpritePlayer
        : AeSpriteInteractive
    {
        /// <summary>
        /// Represents the resource name used to identify the boost functionality for the SpritePlayerBase component.
        /// </summary>
        public readonly string BoostResourceName = "SpritePlayerBase:Boost";

        /// <summary>
        /// Gets the audio clip that is played when ammunition is low.
        /// </summary>
        public AeAudioClip? AmmoLowSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that is played when the ammunition is empty.
        /// </summary>
        public AeAudioClip? AmmoEmptySound { get; private set; }
        /// <summary>
        /// Gets the audio clip used for the ship engine's roar sound effect.
        /// </summary>
        public AeAudioClip? ShipEngineRoarSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that represents the idle sound of the ship engine.
        /// </summary>
        public AeAudioClip? ShipEngineIdleSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that plays when the ship spawns or is fully repaired, indicating that all systems are operational.
        /// </summary>
        public AeAudioClip? AllSystemsGoSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that plays when a shield activation fails.
        /// </summary>
        public AeAudioClip? ShieldFailSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that plays when the shield is deactivated.
        /// </summary>
        public AeAudioClip? ShieldDownSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that is played when the shield reaches its maximum capacity.
        /// </summary>
        public AeAudioClip? ShieldMaxSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that is played when the shield is operating at its nominal state.
        /// </summary>
        public AeAudioClip? ShieldNominalSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that is played when one or more systems are failing.
        /// </summary>
        public AeAudioClip? SystemsFailingSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that is played when the hull is breached.
        /// </summary>
        public AeAudioClip? HullBreachedSound { get; private set; }
        /// <summary>
        /// Gets the audio clip that is played when integrity is low.
        /// </summary>
        public AeAudioClip? IntegrityLowSound { get; private set; }
        /// <summary>
        /// Gets the audio clip used for the ship engine's boost sound effect.
        /// </summary>
        public AeAudioClip? ShipEngineBoostSound { get; private set; }
        /// <summary>
        /// Gets or sets the maximum hull health value for the entity.
        /// </summary>
        public int MaxHullHealth { get; set; }
        /// <summary>
        /// Gets or sets the maximum number of shield points that can be assigned.
        /// </summary>
        public int MaxShieldPoints { get; set; }
        /// <summary>
        /// Gets the animation used to visually represent the thruster.
        /// </summary>
        public AeSpriteAnimation? ThrusterAnimation { get; private set; }
        /// <summary>
        /// Gets the animation sequence used for the booster effect.
        /// </summary>
        public AeSpriteAnimation? BoosterAnimation { get; private set; }
        /// <summary>
        /// Gets the primary weapon assigned to the sprite.
        /// </summary>
        public AeSpriteWeapon? PrimaryWeapon { get; private set; }
        /// <summary>
        /// Gets the currently selected secondary weapon for the sprite, if any.
        /// </summary>
        public AeSpriteWeapon? SelectedSecondaryWeapon { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AeSpritePlayer class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to control and manage sprite playback. Cannot be null.</param>
        public AeSpritePlayer(AeEngine engine)
            : base(engine, (string?)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AeSpritePlayer class using the specified engine and asset key.
        /// </summary>
        /// <remarks>This constructor sets up audio cues, initializes player orientation and throttle, and
        /// configures boost resources. It also ensures that thrust animations are properly created and associated with
        /// the player. The player is centered in the game universe upon initialization.</remarks>
        /// <param name="engine">The game engine instance that provides access to assets, settings, and sprite management required by the
        /// player.</param>
        /// <param name="assetKey">The asset key identifying the sprite resource to associate with the player.</param>
        public AeSpritePlayer(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            OnHit += SpritePlayer_OnHit;

            AmmoLowSound = Engine.Assets.GetAudio("Sounds/Ship/Ammo Low");
            SystemsFailingSound = Engine.Assets.GetAudio("Sounds/Ship/Systems Failing");
            HullBreachedSound = Engine.Assets.GetAudio("Sounds/Ship/Hull Breached");
            IntegrityLowSound = Engine.Assets.GetAudio("Sounds/Ship/Integrity Low");
            ShieldFailSound = Engine.Assets.GetAudio("Sounds/Ship/Shield Fail");
            ShieldDownSound = Engine.Assets.GetAudio("Sounds/Ship/Shield Down");
            ShieldMaxSound = Engine.Assets.GetAudio("Sounds/Ship/Shield Max");
            ShieldNominalSound = Engine.Assets.GetAudio("Sounds/Ship/Shield Nominal");
            AllSystemsGoSound = Engine.Assets.GetAudio("Sounds/Ship/All Systems Go");
            AmmoLowSound = Engine.Assets.GetAudio("Sounds/Ship/Ammo Low");
            AmmoEmptySound = Engine.Assets.GetAudio("Sounds/Ship/Ammo Empty");
            ShipEngineRoarSound = Engine.Assets.GetAudio("Sounds/Ship/Engine Roar");
            ShipEngineIdleSound = Engine.Assets.GetAudio("Sounds/Ship/Engine Idle");
            ShipEngineBoostSound = Engine.Assets.GetAudio("Sounds/Ship/Engine Boost");

            Orientation = AeVector.One();
            Throttle = 0;

            RenewableResources.Create(BoostResourceName, Engine.Settings.MaxPlayerBoostAmount,
                Engine.Settings.MaxPlayerBoostAmount, 250f, Engine.Settings.MaxPlayerBoostAmount / 10);

            if (ThrusterAnimation == null || ThrusterAnimation.IsQueuedForDeletion == true)
            {
                ThrusterAnimation = Engine.Sprites.Animations.Add("Sprites/Animation/ThrustStandard32x32", (o) =>
                {
                    o.SpriteTag = "PlayerForwardThrust";
                    o.IsVisible = false;
                    o.OwnerUID = UID;
                    o.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
                });
            }

            if (BoosterAnimation == null || BoosterAnimation.IsQueuedForDeletion == true)
            {
                BoosterAnimation = Engine.Sprites.Animations.Add("Sprites/Animation/ThrustBoost32x32", (o) =>
                {
                    o.SpriteTag = "PlayerForwardThrust";
                    o.IsVisible = false;
                    o.OwnerUID = UID;
                    o.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
                });
            }

            CenterInUniverse();
        }

        /// <summary>
        /// Releases resources associated with the thruster and booster animations and performs base cleanup operations.
        /// </summary>
        /// <remarks>Call this method when the object is no longer needed to ensure that associated
        /// animations are properly disposed and base class cleanup is performed. This method should be invoked before
        /// disposing the object or when resetting its state.</remarks>
        public override void Cleanup()
        {
            ThrusterAnimation?.QueueForDelete();
            BoosterAnimation?.QueueForDelete();
            base.Cleanup();
        }

        /// <summary>
        /// Handles changes to the visibility state of the component, updating related animations and sounds
        /// accordingly.
        /// </summary>
        /// <remarks>When the component becomes invisible, associated thruster and booster animations are
        /// hidden, and engine sounds are stopped. This method is typically called by the framework when the visibility
        /// of the component changes.</remarks>
        public override void VisibilityChanged()
        {
            UpdateThrustAnimationPositions();
            if (IsVisible == false)
            {
                ThrusterAnimation?.IsVisible = false;
                BoosterAnimation?.IsVisible = false;
                ShipEngineIdleSound?.Stop();
                ShipEngineRoarSound?.Stop();
            }
        }

        /// <summary>
        /// Handles changes in orientation by updating the positions of thrust animations.
        /// </summary>
        public override void OrientationChanged() => UpdateThrustAnimationPositions();

        /// <summary>
        /// Updates the animation positions in response to a change in the player's location.
        /// </summary>
        /// <remarks>This method should be called when the player's location changes to ensure that
        /// related animations remain synchronized with the new position. The player's position itself is not modified;
        /// only the animation offsets are updated.</remarks>
        //The player position does not change, only the background offset changes... hmmmm. :/
        public override void LocationChanged() => UpdateThrustAnimationPositions();

        /// <summary>
        /// Generates a formatted help text describing the current loadout, including weapon details, shields, hull,
        /// speed, throttle, and description.
        /// </summary>
        /// <remarks>The returned text is intended for display in user interfaces or logs to provide a
        /// readable summary of the loadout configuration. Weapon names and munition counts are included if available;
        /// otherwise, default values are used.</remarks>
        /// <returns>A string containing the formatted loadout information. The string includes the name, primary and secondary
        /// weapons, shields, hull, speed, throttle, and description.</returns>
        public string GetLoadoutHelpText()
        {
            string primaryWeapon = "None";

            if (!string.IsNullOrEmpty(Metadata.PrimaryWeaponAssetKey))
            {
                var primaryWeaponMetadata = Engine.Assets.GetMetadata(Metadata.PrimaryWeaponAssetKey);
                primaryWeapon = $"{primaryWeaponMetadata.Name} x{primaryWeaponMetadata.MunitionCount}";
            }

            string secondaryWeapons = string.Empty;
            if (Metadata.WeaponAssetKeys != null)
            {
                foreach (var weaponAssetKey in Metadata.WeaponAssetKeys)
                {
                    var secondaryWeaponMetadata = Engine.Assets.GetMetadata(weaponAssetKey);
                    secondaryWeapons += $"{secondaryWeaponMetadata.Name} x{secondaryWeaponMetadata.MunitionCount}\n{new string(' ', 20)}";
                }
            }

            string result = $"             Name : {Metadata.Name}\n";
            result += $"   Primary weapon : {primaryWeapon.Trim()}\n";
            result += $"Secondary Weapons : {secondaryWeapons.Trim()}\n";
            result += $"          Shields : {Metadata.Shields:n0}\n";
            result += $"             Hull : {Metadata.Hull:n0}\n";
            result += $"            Speed : {Metadata.Speed:n1}\n";
            result += $"         Throttle : {Metadata.MaxThrottle:n1}\n";
            result += $"\n{Metadata.Description}";

            return result;
        }

        /// <summary>
        /// Resets ship state, health etc while keeping the existing class.
        /// </summary>
        public void Reset()
        {
            ReviveDeadOrExploded();

            //TODO: We should reload metadata and reapply it.
        }

        /// <summary>
        /// Adds the specified number of points to the shield health, up to the maximum allowed value.
        /// </summary>
        /// <remarks>If the shield health reaches its maximum as a result of this operation, a
        /// notification sound may be played to inform the player.</remarks>
        /// <param name="pointsToAdd">The number of shield health points to add. Must be a positive integer.</param>
        public override void AddShieldHealth(int pointsToAdd)
        {
            if (ShieldHealth < Engine.Settings.MaxShieldHealth && ShieldHealth + pointsToAdd >= Engine.Settings.MaxShieldHealth)
            {
                ShieldMaxSound?.Play(); //If we didn't have full shields but now we do, tell the player.
            }

            base.AddShieldHealth(pointsToAdd);
        }

        private void UpdateThrustAnimationPositions()
        {
            var pointBehind = (Orientation * -1) * new AeVector(40, 40);

            if (ThrusterAnimation != null)
            {
                if (IsVisible)
                {
                    ThrusterAnimation.Orientation = Orientation;
                    ThrusterAnimation.Location = Location + pointBehind;
                }
            }

            if (BoosterAnimation != null)
            {
                if (IsVisible)
                {
                    BoosterAnimation.Orientation = Orientation;
                    BoosterAnimation.Location = Location + pointBehind;
                }
            }
        }

        /// <summary>
        /// Handles the event when a munition collides with this object.
        /// </summary>
        /// <remarks>This method processes the hit and updates the object's state accordingly. If the
        /// object's hull health reaches zero, it does not automatically remove the object; the engine assumes the
        /// object remains valid.</remarks>
        /// <param name="munition">The munition that has impacted this object. Cannot be null.</param>
        public override void MunitionHit(AeSpriteMunition munition)
        {
            Hit(munition);
            if (HullHealth <= 0)
            {
                //Explode(); //We don't auto delete the player because the engine always assumes its valid. 
            }
        }

        /// <summary>
        /// Determines whether the specified enemy-fired munition hits the object at the given position.
        /// </summary>
        /// <remarks>This method only evaluates munitions fired from enemies. Munitions from other sources
        /// are ignored.</remarks>
        /// <param name="munition">The munition to test for a hit. Only munitions fired from enemies are considered.</param>
        /// <param name="hitTestPosition">The position to test for a potential hit, typically representing the impact location.</param>
        /// <returns>true if the enemy-fired munition intersects the object's axis-aligned bounding box at the specified
        /// position; otherwise, false.</returns>
        public override bool TryMunitionHit(AeSpriteMunition munition, AeVector hitTestPosition)
        {
            if (munition.FiredFromType == AeFiredFromType.Enemy)
            {
                return IntersectsAabb(hitTestPosition);
            }
            return false;
        }

        private void SpritePlayer_OnHit(AeSprite sender, AeDamageType damageType, int damageAmount)
        {
            if (damageType == AeDamageType.Shield)
            {
                if (ShieldHealth == 0)
                {
                    ShieldDownSound?.Play();
                }
            }

            //This is the hit that took us under the threshold.
            if (HullHealth < 100 && HullHealth + damageAmount > 100)
            {
                IntegrityLowSound?.Play();
            }
            else if (HullHealth < 50 && HullHealth + damageAmount > 50)
            {
                SystemsFailingSound?.Play();
            }
            else if (HullHealth < 20 && HullHealth + damageAmount > 20)
            {
                HullBreachedSound?.Play();
            }
        }

        #region Weapons selection and evaluation.

        /// <summary>
        /// Sets the primary weapon for the entity using the specified asset key and munition count.
        /// </summary>
        /// <remarks>This method replaces the current primary weapon with a new instance based on the
        /// provided asset. Ensure that the asset key references a valid weapon asset and that the munition count is
        /// appropriate for gameplay requirements.</remarks>
        /// <param name="assetKey">The unique identifier for the weapon asset to assign as the primary weapon. Cannot be null or empty.</param>
        /// <param name="munitionCount">The number of munitions to initialize for the primary weapon. Must be a non-negative integer.</param>
        /// <exception cref="Exception">Thrown if the specified asset key does not correspond to a valid weapon asset, or if the asset lacks a
        /// defined class or controller in its metadata.</exception>
        public void SetPrimaryWeapon(string assetKey, int munitionCount)
        {
            var asset = Engine.Assets.GetAsset(assetKey)
                ?? throw new Exception($"The metadata for the weapon sprite '{assetKey}' does not exist.");

            var className = (string.IsNullOrEmpty(asset.ControllerName) ? asset.Metadata.Class : asset.ControllerName)
                ?? throw new Exception($"The sprite {assetKey} does not have a class or controller defined in its metadata.");
            var type = AeReflection.GetTypeByName(className);

            PrimaryWeapon = (AeSpriteWeapon)Activator.CreateInstance(type, [Engine, this, assetKey]).EnsureNotNull();
            PrimaryWeapon.MunitionQuantity = munitionCount;
        }

        /// <summary>
        /// Selects the previous available and usable secondary weapon from the collection, updating the selection
        /// accordingly.
        /// </summary>
        /// <remarks>If the current secondary weapon is at the start of the collection, the selection
        /// wraps to the last available usable secondary weapon. If no usable secondary weapon exists, the method
        /// returns null.</remarks>
        /// <returns>The previous available and usable secondary weapon, or the first or last available weapon if the current
        /// selection is at the beginning or end of the collection. Returns null if no usable secondary weapon is found.</returns>
        public AeSpriteWeapon? SelectPreviousAvailableUsableSecondaryWeapon()
        {
            AeSpriteWeapon? previousWeapon = null;

            foreach (var weapon in Weapons)
            {
                if (weapon == SelectedSecondaryWeapon)
                {
                    if (previousWeapon == null)
                    {
                        return SelectLastAvailableUsableSecondaryWeapon(); //No suitable weapon found after the current one. Go back to the end.
                    }
                    SelectedSecondaryWeapon = previousWeapon;
                    return previousWeapon;
                }

                previousWeapon = weapon;
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No suitable weapon found after the current one. Go back to the beginning.
        }

        /// <summary>
        /// Selects the next available secondary weapon in the collection after the currently selected one.
        /// </summary>
        /// <remarks>If the currently selected secondary weapon is the last in the collection, the method
        /// wraps around and selects the first available usable secondary weapon. The selection updates the current
        /// secondary weapon state.</remarks>
        /// <returns>The next available and usable secondary weapon, or the first available secondary weapon if none is found
        /// after the current selection. Returns null if no usable secondary weapon exists.</returns>
        public AeSpriteWeapon? SelectNextAvailableUsableSecondaryWeapon()
        {
            bool selectNextWeapon = false;

            foreach (var weapon in Weapons)
            {
                if (selectNextWeapon)
                {
                    SelectedSecondaryWeapon = weapon;
                    return weapon;
                }

                if (weapon == SelectedSecondaryWeapon) //Find the current weapon in the collection;
                {
                    selectNextWeapon = true;
                }
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No suitable weapon found after the current one. Go back to the beginning.
        }

        /// <summary>
        /// Selects and returns the first available secondary weapon with usable munitions.
        /// </summary>
        /// <remarks>If a usable secondary weapon is found, it is set as the currently selected secondary
        /// weapon. Otherwise, the selection is cleared.</remarks>
        /// <returns>The first secondary weapon with a positive munition quantity, or null if no such weapon is available.</returns>
        public AeSpriteWeapon? SelectFirstAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in Weapons where o.MunitionQuantity > 0 select o).FirstOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        /// <summary>
        /// Selects and returns the last available secondary weapon with remaining munitions.
        /// </summary>
        /// <remarks>If a usable secondary weapon is found, it is set as the currently selected secondary
        /// weapon. Otherwise, the selection is cleared.</remarks>
        /// <returns>The last usable secondary weapon with a positive munition quantity, or null if none are available.</returns>
        public AeSpriteWeapon? SelectLastAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in Weapons where o.MunitionQuantity > 0 select o).LastOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        #endregion
    }
}
