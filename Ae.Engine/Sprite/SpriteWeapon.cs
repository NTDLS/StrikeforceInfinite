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
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    [AssetClass("Weapon", "", AeBaseAssetType.Image, true)]
    public class SpriteWeapon
        : SpriteBase
    {
        /// <summary>
        /// UTC datetime that the weapon was last fired.
        /// </summary>
        public DateTime LastFired { get; private set; } = DateTime.UtcNow.AddMinutes(-5);

        /// <summary>
        /// The sound that the weapon makes when firing.
        /// </summary>
        public SpriteBase Owner { get; set; }

        public List<WeaponsLock> LockedTargets { get; set; } = new();
        public int MunitionsFired { get; set; }
        public int MunitionQuantity { get; set; }

        public SpriteWeapon(AeEngine engine, SpriteBase owner, string? assetKey)
            : base(engine, assetKey)
        {
            Owner = owner;
        }

        public class WeaponsLock
        {
            public float Distance { get; set; }
            public SpriteInteractive Sprite { get; set; }
            public SiWeaponsLockType LockType { get; set; }

            public WeaponsLock(SpriteInteractive sprite, float distance)
            {
                Sprite = sprite;
                Distance = distance;
            }
        }

        public SpriteMunition CreateMunition(AeVector? location = null, SpriteInteractive? lockedTarget = null)
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

            var munitionSprite = (SpriteMunition)Activator.CreateInstance(type,
                [Engine, this, Owner, munitionAssetKey, lockedTarget, location ?? Owner.Location]).EnsureNotNull();

            return munitionSprite;
        }

        public virtual void ApplyIntelligence(float epoch)
        {
            //We're just doing "locked on" magic here.

            Metadata.EnsureNotNull();

            LockedTargets.Clear();

            if (Owner is SpritePlayer owner)
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
                    hardLock.LockType = SiWeaponsLockType.Hard;
                    hardLock.Sprite.IsLockedOn = true;
                    hardLock.Sprite.IsLockedOnSoft = false;
                }

                foreach (var softLock in LockedTargets.Skip(Metadata.MaxLocks ?? 0))
                {
                    softLock.LockType = SiWeaponsLockType.Soft;
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
            else if (Owner is SpriteEnemy enemy)
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
                            LockType = SiWeaponsLockType.Hard
                        });
                    }
                }
            }
        }

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

        public virtual void Hit()
        {
        }

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
