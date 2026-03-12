using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Interactive.Ship;
using Ae.Engine.Sprite.Munition;
using NTDLS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    [AssetClass("Weapon", "", AeBaseAssetType.Image, true)]
    public class AeSpriteWeapon
        : AeSprite
    {
        /// <summary>
        /// UTC datetime that the weapon was last fired.
        /// </summary>
        public DateTime LastFired { get; private set; } = DateTime.UtcNow.AddMinutes(-5);

        /// <summary>
        /// The sound that the weapon makes when firing.
        /// </summary>
        public AeSprite Owner { get; set; }

        /// <summary>
        /// Gets or sets the collection of targets currently locked by the weapon system.
        /// </summary>
        public List<WeaponsLock> LockedTargets { get; set; } = new();

        /// <summary>
        /// Gets or sets the total number of munitions that have been fired.
        /// </summary>
        public int MunitionsFired { get; set; }

        /// <summary>
        /// Gets or sets the number of munitions available.
        /// </summary>
        public int MunitionQuantity { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteWeapon class with the specified engine, owner, and asset key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage the weapon's behavior and interactions.</param>
        /// <param name="owner">The sprite that owns this weapon. Determines the context in which the weapon operates.</param>
        /// <param name="assetKey">The asset key identifying the visual or configuration resource for the weapon. Can be null if no asset is
        /// associated.</param>
        public AeSpriteWeapon(AeEngine engine, AeSprite owner, string? assetKey)
            : base(engine, assetKey)
        {
            Owner = owner;
        }

        /// <summary>
        /// Represents a weapons lock on a target, including the associated sprite, lock type, and distance to the
        /// target.
        /// </summary>
        /// <remarks>The weapons lock provides information used to determine targeting and engagement
        /// behavior in gameplay scenarios. The lock type indicates the nature of the lock, which may affect weapon
        /// selection or firing conditions.</remarks>
        public class WeaponsLock
        {
            /// <summary>
            /// Gets or sets the distance value represented by this property.
            /// </summary>
            public float Distance { get; set; }

            /// <summary>
            /// Gets or sets the interactive sprite associated with this instance.
            /// </summary>
            public AeSpriteInteractive Sprite { get; set; }

            /// <summary>
            /// Gets or sets the lock type applied to the weapon system.
            /// </summary>
            public AeWeaponsLockType LockType { get; set; }

            /// <summary>
            /// Initializes a new instance of the WeaponsLock class with the specified interactive sprite and lock
            /// distance.
            /// </summary>
            /// <param name="sprite">The interactive sprite to associate with the weapons lock. Cannot be null.</param>
            /// <param name="distance">The distance, in units, at which the weapons lock is established. Must be non-negative.</param>
            public WeaponsLock(AeSpriteInteractive sprite, float distance)
            {
                Sprite = sprite;
                Distance = distance;
            }
        }

        /// <summary>
        /// Creates a new munition sprite instance for this weapon at the specified location and optionally locks it
        /// onto a target.
        /// </summary>
        /// <param name="location">The location where the munition will be spawned. If null, the munition is spawned at the owner's current
        /// location.</param>
        /// <param name="lockedTarget">An optional target for the munition to lock onto. If null, the munition will not be locked onto any target.</param>
        /// <returns>A new instance of AeSpriteMunition representing the created munition sprite.</returns>
        /// <exception cref="Exception">Thrown if the weapon is not owned, if no munition sprite path is defined in the weapon metadata, or if the
        /// sprite asset does not have a class or controller defined in its metadata.</exception>
        public AeSpriteMunition CreateMunition(AeVector? location = null, AeSpriteInteractive? lockedTarget = null)
        {
            if (Owner == null)
            {
                throw new Exception("Weapon is not owned.");
            }

            string? munitionAssetKey = null;

            int? spriteCount = Metadata.EnsureNotNull().MunitionAssetKeys?.Length;

            if (Metadata.MunitionAssetKeys != null && spriteCount > 0)
                munitionAssetKey = Metadata.MunitionAssetKeys.OneOfNullable();

            if (munitionAssetKey == null)
                throw new Exception($"Weapon {Metadata.Name} does not have a munition sprite path defined.");

            var asset = Engine.Assets.GetAsset(munitionAssetKey);

            var className = (string.IsNullOrEmpty(asset.ControllerName) ? asset.Metadata.Class : asset.ControllerName)
                ?? throw new Exception($"The sprite {munitionAssetKey} does not have a class or controller defined in its metadata.");
            var type = AeReflection.GetTypeByName(className);

            var munitionSprite = (AeSpriteMunition)Activator.CreateInstance(type,
                [Engine, this, Owner, munitionAssetKey, lockedTarget, location ?? Owner.Location]).EnsureNotNull();

            return munitionSprite;
        }

        /// <summary>
        /// Applies targeting intelligence for the current epoch, updating weapon lock states based on the owner's
        /// position and available targets.
        /// </summary>
        /// <remarks>This method updates the locked targets for the owner, assigning hard and soft locks
        /// according to proximity and angle constraints. It ensures that lock states are consistent with the owner's
        /// targeting capabilities. Call this method once per update cycle to maintain accurate lock
        /// information.</remarks>
        /// <param name="epoch">The current epoch time, used to determine the timing of lock state updates.</param>
        public virtual void ApplyIntelligence(float epoch)
        {
            //We're just doing "locked on" magic here.

            Metadata.EnsureNotNull();

            LockedTargets.Clear();

            if (Owner is AeSpritePlayer owner)
            {
                var potentialTargets = Engine.Sprites.Enemies.Visible();

                foreach (var potentialTarget in potentialTargets)
                {
                    if (Metadata.MaxLockDistance > 0 && Owner.IsPointingAt(potentialTarget, Metadata.MaxLockOnAngle ?? 0))
                    {
                        var distance = Owner.DistanceTo(potentialTarget);
                        if (distance.IsBetween(Metadata.MinLockDistance ?? 0, Metadata.MaxLockDistance.Value))
                        {
                            LockedTargets.Add(new WeaponsLock(potentialTarget, Owner.DistanceTo(potentialTarget)));
                        }
                    }
                }

                LockedTargets = LockedTargets.OrderBy(o => o.Distance).ToList();

                foreach (var hardLock in LockedTargets.Take(Metadata.MaxLocks ?? 0))
                {
                    hardLock.LockType = AeWeaponsLockType.Hard;
                    hardLock.Sprite.IsLockedOn = true;
                    hardLock.Sprite.IsLockedOnSoft = false;
                }

                foreach (var softLock in LockedTargets.Skip(Metadata.MaxLocks ?? 0))
                {
                    softLock.LockType = AeWeaponsLockType.Soft;
                    softLock.Sprite.IsLockedOn = false;
                    softLock.Sprite.IsLockedOnSoft = true;
                }

                var lockedTargets = LockedTargets.Select(o => o.Sprite);

                foreach (var potentialTarget in potentialTargets.Where(o => !lockedTargets.Contains(o)))
                {
                    potentialTarget.IsLockedOn = false;
                    potentialTarget.IsLockedOnSoft = false;
                }
            }
            else if (Owner is AeSpriteEnemy enemy)
            {
                Engine.Player.Sprite.IsLockedOnSoft = false;
                Engine.Player.Sprite.IsLockedOn = false;

                if (Metadata.MaxLockDistance > 0 && Owner.IsPointingAt(Engine.Player.Sprite, Metadata.MaxLockOnAngle ?? 0))
                {
                    var distance = Owner.DistanceTo(Engine.Player.Sprite);
                    if (distance.IsBetween(Metadata.MinLockDistance ?? 0, Metadata.MaxLockDistance.Value))
                    {
                        Engine.Player.Sprite.IsLockedOn = true;
                        Engine.Player.Sprite.IsLockedOnSoft = false;

                        LockedTargets.Add(new WeaponsLock(Engine.Player.Sprite, Owner.DistanceTo(Engine.Player.Sprite))
                        {
                            LockType = AeWeaponsLockType.Hard
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to fire the weapon at the specified location.
        /// </summary>
        /// <remarks>The method decreases the available munition quantity and plays a firing sound if the
        /// weapon can be fired. The fired munition is added to the engine's sprite collection at the specified
        /// location.</remarks>
        /// <param name="location">The target location where the munition will be fired.</param>
        /// <returns>true if the weapon was successfully fired; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the weapon does not have an owner.</exception>
        public virtual bool Fire(AeVector location)
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanFire)
            {
                MunitionsFired++;
                MunitionQuantity--;
                Sounds?.OneOf()?.Play();
                Engine.Sprites.Munitions.Add(this, location);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to fire the weapon, updating its state and triggering associated effects if firing is permitted.
        /// </summary>
        /// <remarks>Calling this method decreases the munition quantity and increases the fired count if
        /// firing is allowed. Associated sound effects and sprite updates are triggered as part of the firing
        /// process.</remarks>
        /// <returns>true if the weapon was successfully fired; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the weapon does not have an owner.</exception>
        public virtual bool Fire()
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanFire)
            {
                MunitionsFired++;
                MunitionQuantity--;
                Sounds?.OneOf()?.Play();
                Engine.Sprites.Munitions.Add(this);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles a hit event for the current object. Override this method to define custom behavior when the object
        /// is hit.
        /// </summary>
        /// <remarks>This method is intended to be overridden in derived classes to implement specific hit
        /// logic. The base implementation does not perform any action.</remarks>
        public virtual void Hit()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the munition can be fired based on the current quantity and required delay
        /// between firings.
        /// </summary>
        /// <remarks>The property returns <see langword="true"/> if there is at least one munition
        /// available and the required delay since the last firing has elapsed; otherwise, it returns <see
        /// langword="false"/>. Accessing this property may update the last fired timestamp if firing is
        /// permitted.</remarks>
        public bool CanFire
        {
            get
            {
                bool result = false;
                if (MunitionQuantity > 0)
                {
                    result = (DateTime.UtcNow - LastFired).TotalMilliseconds > (Metadata.FireDelayMilliseconds ?? 0);
                    if (result)
                    {
                        LastFired = DateTime.UtcNow;
                    }
                }
                return result;
            }
        }
    }
}
