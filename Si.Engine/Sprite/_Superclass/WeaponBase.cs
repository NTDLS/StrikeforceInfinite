using NTDLS.Helpers;
using Si.Audio;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    public class WeaponBase
        : SpriteBase
    {
        protected SpriteInteractive Owner { get; private set; }
        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected SiAudioClip? _fireSound;

        public List<WeaponsLock> LockedTargets { get; set; } = new();
        public int RoundsFired { get; set; }
        public int RoundQuantity { get; set; }

        public WeaponBase(EngineCore engine, SpriteInteractive owner, string? assetKey)
            : base(engine, assetKey)
        {
            Owner = owner;

            if (!string.IsNullOrEmpty(Metadata.SoundAssetKey))
            {
                _fireSound = Engine.Assets.GetAudio(Metadata.SoundAssetKey, Metadata.SoundVolume ?? 0);
            }
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

        public SpriteMunition CreateMunition(SiVector? location = null, SpriteInteractive? lockedTarget = null)
        {
            if (Owner == null)
            {
                throw new Exception("Weapon is not owned.");
            }

            string? munitionAssetKey = null;

            int? spriteCount = Metadata.EnsureNotNull().MunitionAssetKeys?.Length;

            if (Metadata.MunitionAssetKeys != null && spriteCount > 0)
                munitionAssetKey = Metadata.MunitionAssetKeys[SiRandom.Between(0, spriteCount.Value - 1)];

            if (munitionAssetKey == null)
                throw new Exception($"Weapon {Metadata.Name} does not have a munition sprite path defined.");

            var munitionSpriteMeta = Engine.Assets.GetMetadata(munitionAssetKey);

            var munitionSpriteType = SiReflection.GetTypeByName(munitionSpriteMeta.Class
                ?? throw new Exception($"The munition sprite {munitionAssetKey} does not have a type defined in its metadata."));

            var munitionSprite = (SpriteMunition)Activator.CreateInstance(munitionSpriteType,
                [Engine, this, Owner, munitionAssetKey, lockedTarget, location ?? Owner.Location]).EnsureNotNull();

            return munitionSprite;

            /*
            switch (munitionSpriteMeta.MunitionType)
            {
                case MunitionType.Projectile:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new ProjectileMunitionBase(_engine, this, Owner, munitionAssetKey, location ?? Owner.Location);
                    }
                case MunitionType.Energy:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new EnergyMunitionBase(_engine, this, Owner, munitionAssetKey, location ?? Owner.Location);
                    }
                case MunitionType.Seeking:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new SeekingMunitionBase(_engine, this, Owner, munitionAssetKey, location ?? Owner.Location);
                    }
                case MunitionType.Locking:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new LockingMunitionBase(_engine, this, Owner, munitionAssetKey, lockedTarget, location ?? Owner.Location);
                    }
                default:
                    throw new Exception($"The weapon type {Metadata.MunitionType} is not implemented.");
            }
            */
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
                    hardLock.Sprite.IsLockedOnHard = true;
                    hardLock.Sprite.IsLockedOnSoft = false;
                }

                foreach (var softLock in LockedTargets.Skip(Metadata.MaxLocks ?? 0))
                {
                    softLock.LockType = SiWeaponsLockType.Soft;
                    softLock.Sprite.IsLockedOnHard = false;
                    softLock.Sprite.IsLockedOnSoft = true;
                }

                var lockedTargets = LockedTargets.Select(o => o.Sprite);

                foreach (var potentialTarget in potentialTargets.Where(o => !lockedTargets.Contains(o)))
                {
                    potentialTarget.IsLockedOnHard = false;
                    potentialTarget.IsLockedOnSoft = false;
                }
            }
            else if (Owner is SpriteEnemy enemy)
            {
                Engine.Player.Sprite.IsLockedOnSoft = false;
                Engine.Player.Sprite.IsLockedOnHard = false;

                if (Metadata.MaxLockDistance > 0 && Owner.IsPointingAt(Engine.Player.Sprite, Metadata.MaxLockOnAngle ?? 0))
                {
                    var distance = Owner.DistanceTo(Engine.Player.Sprite);
                    if (distance.IsBetween(Metadata.MinLockDistance ?? 0, Metadata.MaxLockDistance.Value))
                    {
                        Engine.Player.Sprite.IsLockedOnHard = true;
                        Engine.Player.Sprite.IsLockedOnSoft = false;

                        LockedTargets.Add(new WeaponsLock(Engine.Player.Sprite, Owner.DistanceTo(Engine.Player.Sprite))
                        {
                            LockType = SiWeaponsLockType.Hard
                        });
                    }
                }
            }
        }

        public virtual bool Fire(SiVector location)
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanFire)
            {
                RoundsFired++;
                RoundQuantity--;
                _fireSound?.Play();
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
                RoundsFired++;
                RoundQuantity--;
                _fireSound?.Play();
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
                if (RoundQuantity > 0)
                {
                    result = (DateTime.Now - _lastFired).TotalMilliseconds > (Metadata.FireDelayMilliseconds ?? 0);
                    if (result)
                    {
                        _lastFired = DateTime.Now;
                    }
                }
                return result;
            }
        }
    }
}
