using NTDLS.Helpers;
using Si.Audio;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    /// <summary>
    /// The player base is a sub-class of the ship base. It is only used by the Player and as a model for menu selections.
    /// </summary>
    public class SpritePlayer
        : SpriteInteractiveBase
    {
        public readonly string BoostResourceName = "SpritePlayerBase:Boost";

        public SiAudioClip? AmmoLowSound { get; private set; }
        public SiAudioClip? AmmoEmptySound { get; private set; }
        public SiAudioClip? ShipEngineRoarSound { get; private set; }
        public SiAudioClip? ShipEngineIdleSound { get; private set; }
        public SiAudioClip? AllSystemsGoSound { get; private set; }
        public SiAudioClip? ShieldFailSound { get; private set; }
        public SiAudioClip? ShieldDownSound { get; private set; }
        public SiAudioClip? ShieldMaxSound { get; private set; }
        public SiAudioClip? ShieldNominalSound { get; private set; }
        public SiAudioClip? SystemsFailingSound { get; private set; }
        public SiAudioClip? HullBreachedSound { get; private set; }
        public SiAudioClip? IntegrityLowSound { get; private set; }
        public SiAudioClip? ShipEngineBoostSound { get; private set; }
        public int MaxHullHealth { get; set; }
        public int MaxShieldPoints { get; set; }
        public SpriteAnimation? ThrusterAnimation { get; private set; }
        public SpriteAnimation? BoosterAnimation { get; private set; }
        public WeaponBase? PrimaryWeapon { get; private set; }
        public WeaponBase? SelectedSecondaryWeapon { get; private set; }

        public SpritePlayer(EngineCore engine)
            : base(engine, (string?)null)
        {
        }

        public SpritePlayer(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            OnHit += SpritePlayer_OnHit;

            AmmoLowSound = _engine.Assets.GetAudio("Sounds/Ship/Ammo Low");
            SystemsFailingSound = _engine.Assets.GetAudio("Sounds/Ship/Systems Failing");
            HullBreachedSound = _engine.Assets.GetAudio("Sounds/Ship/Hull Breached");
            IntegrityLowSound = _engine.Assets.GetAudio("Sounds/Ship/Integrity Low");
            ShieldFailSound = _engine.Assets.GetAudio("Sounds/Ship/Shield Fail");
            ShieldDownSound = _engine.Assets.GetAudio("Sounds/Ship/Shield Down");
            ShieldMaxSound = _engine.Assets.GetAudio("Sounds/Ship/Shield Max");
            ShieldNominalSound = _engine.Assets.GetAudio("Sounds/Ship/Shield Nominal");
            AllSystemsGoSound = _engine.Assets.GetAudio("Sounds/Ship/All Systems Go");
            AmmoLowSound = _engine.Assets.GetAudio("Sounds/Ship/Ammo Low");
            AmmoEmptySound = _engine.Assets.GetAudio("Sounds/Ship/Ammo Empty");
            ShipEngineRoarSound = _engine.Assets.GetAudio("Sounds/Ship/Engine Roar");
            ShipEngineIdleSound = _engine.Assets.GetAudio("Sounds/Ship/Engine Idle");
            ShipEngineBoostSound = _engine.Assets.GetAudio("Sounds/Ship/Engine Boost");

            Orientation = SiVector.One();
            Throttle = 0;

            RenewableResources.Create(BoostResourceName, _engine.Settings.MaxPlayerBoostAmount,
                _engine.Settings.MaxPlayerBoostAmount, 250f, _engine.Settings.MaxPlayerBoostAmount / 10);

            if (ThrusterAnimation == null || ThrusterAnimation.IsQueuedForDeletion == true)
            {
                ThrusterAnimation = _engine.Sprites.Animations.Add("Sprites/Animation/ThrustStandard32x32", (o) =>
                {
                    o.SpriteTag = "PlayerForwardThrust";
                    o.IsVisible = false;
                    o.OwnerUID = UID;
                    o.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
                });
            }

            if (BoosterAnimation == null || BoosterAnimation.IsQueuedForDeletion == true)
            {
                BoosterAnimation = _engine.Sprites.Animations.Add("Sprites/Animation/ThrustBoost32x32", (o) =>
                {
                    o.SpriteTag = "PlayerForwardThrust";
                    o.IsVisible = false;
                    o.OwnerUID = UID;
                    o.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
                });
            }

            CenterInUniverse();
        }

        public override void Cleanup()
        {
            ThrusterAnimation?.QueueForDelete();
            BoosterAnimation?.QueueForDelete();
            base.Cleanup();
        }

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

        public override void OrientationChanged() => UpdateThrustAnimationPositions();

        //The player position does not change, only the background offset changes... hmmmm. :/
        public override void LocationChanged() => UpdateThrustAnimationPositions();

        public string GetLoadoutHelpText()
        {
            string primaryWeapon = "None";

            if (!string.IsNullOrEmpty(Metadata.PrimaryWeaponAssetKey))
            {
                var primaryWeaponMetadata = _engine.Assets.GetMetadata(Metadata.PrimaryWeaponAssetKey);
                primaryWeapon = $"{primaryWeaponMetadata.Name} x{primaryWeaponMetadata.MunitionCount}";
            }

            string secondaryWeapons = string.Empty;
            if (Metadata.WeaponAssetKeys != null)
            {
                foreach (var weaponAssetKey in Metadata.WeaponAssetKeys)
                {
                    var secondaryWeaponMetadata = _engine.Assets.GetMetadata(weaponAssetKey);
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

        public override void AddShieldHealth(int pointsToAdd)
        {
            if (ShieldHealth < _engine.Settings.MaxShieldHealth && ShieldHealth + pointsToAdd >= _engine.Settings.MaxShieldHealth)
            {
                ShieldMaxSound?.Play(); //If we didn't have full shields but now we do, tell the player.
            }

            base.AddShieldHealth(pointsToAdd);
        }

        private void UpdateThrustAnimationPositions()
        {
            var pointBehind = (Orientation * -1) * new SiVector(40, 40);

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

        public override void MunitionHit(MunitionBase munition)
        {
            Hit(munition);
            if (HullHealth <= 0)
            {
                //Explode(); //We don't auto delete the player because the engine always assumes its valid. 
            }
        }

        public override bool TryMunitionHit(MunitionBase munition, SiVector hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Enemy)
            {
                return IntersectsAabb(hitTestPosition);
            }
            return false;
        }

        private void SpritePlayer_OnHit(SpriteBase sender, SiDamageType damageType, int damageAmount)
        {
            if (damageType == SiDamageType.Shield)
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

        public void SetPrimaryWeapon(string assetKey, int munitionCount)
        {
            var metadata = _engine.Assets.GetMetadata(assetKey)
                ?? throw new Exception($"The metadata for the weapon sprite '{assetKey}' does not exist.");

            var type = SiReflection.GetTypeByName(metadata.Class ?? throw new Exception("Weapon class is not defined."));
            PrimaryWeapon = (WeaponBase)Activator.CreateInstance(type, [_engine, this, assetKey]).EnsureNotNull();
            PrimaryWeapon.RoundQuantity = munitionCount;
        }

        public WeaponBase? SelectPreviousAvailableUsableSecondaryWeapon()
        {
            WeaponBase? previousWeapon = null;

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

        public WeaponBase? SelectNextAvailableUsableSecondaryWeapon()
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

        public WeaponBase? SelectFirstAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in Weapons where o.RoundQuantity > 0 select o).FirstOrDefault();
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

        public WeaponBase? SelectLastAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in Weapons where o.RoundQuantity > 0 select o).LastOrDefault();
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
