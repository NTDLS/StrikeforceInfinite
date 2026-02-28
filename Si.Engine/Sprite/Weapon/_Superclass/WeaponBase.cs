using NTDLS.Helpers;
using Si.Audio;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Weapon._Superclass
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    public class WeaponBase
        : SpriteBase
    {
        protected SpriteInteractiveBase Owner { get; private set; }
        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected SiAudioClip? _fireSound;

        public List<WeaponsLock> LockedTargets { get; set; } = new();
        public int RoundsFired { get; set; }
        public int RoundQuantity { get; set; }

        public WeaponBase(EngineCore engine, SpriteInteractiveBase owner, string? spritePath)
            : base(engine, spritePath)
        {
            Owner = owner;
            _engine = engine;

            if (!string.IsNullOrEmpty(Metadata.SoundPath))
            {
                _fireSound = _engine.Assets.GetAudio(Metadata.SoundPath, Metadata.SoundVolume ?? 0);
            }
        }

        public class WeaponsLock
        {
            public float Distance { get; set; }
            public SpriteInteractiveBase Sprite { get; set; }
            public SiWeaponsLockType LockType { get; set; }

            public WeaponsLock(SpriteInteractiveBase sprite, float distance)
            {
                Sprite = sprite;
                Distance = distance;
            }
        }

        public MunitionBase CreateMunition(SiVector? location = null, SpriteInteractiveBase? lockedTarget = null)
        {
            if (Owner == null)
            {
                throw new Exception("Weapon is not owned.");
            }

            string? munitionSpritePath = null;

            int? spriteCount = Metadata.EnsureNotNull().MunitionSpritePaths?.Length;

            if (Metadata.MunitionSpritePaths != null && spriteCount > 0)
                munitionSpritePath = Metadata.MunitionSpritePaths[SiRandom.Between(0, spriteCount.Value - 1)];

            if (munitionSpritePath == null)
                throw new Exception($"Weapon {Metadata.Name} does not have a munition sprite path defined.");

            var munitionSpriteMeta = _engine.Assets.GetMetadata(munitionSpritePath);

            var munitionSpriteType = SiReflection.GetTypeByName(munitionSpriteMeta.Class
                ?? throw new Exception($"The munition sprite {munitionSpritePath} does not have a type defined in its metadata."));

            var munitionSprite = (MunitionBase)Activator.CreateInstance(munitionSpriteType,
                [_engine, this, Owner, munitionSpritePath, lockedTarget, location ?? Owner.Location]).EnsureNotNull();

            return munitionSprite;

            /*
            switch (munitionSpriteMeta.MunitionType)
            {
                case MunitionType.Projectile:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new ProjectileMunitionBase(_engine, this, Owner, munitionSpritePath, location ?? Owner.Location);
                    }
                case MunitionType.Energy:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new EnergyMunitionBase(_engine, this, Owner, munitionSpritePath, location ?? Owner.Location);
                    }
                case MunitionType.Seeking:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new SeekingMunitionBase(_engine, this, Owner, munitionSpritePath, location ?? Owner.Location);
                    }
                case MunitionType.Locking:
                    {
                        //TODO: dont instantiate here. Usee SpriteFactory or something.
                        return new LockingMunitionBase(_engine, this, Owner, munitionSpritePath, lockedTarget, location ?? Owner.Location);
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
                var potentialTargets = _engine.Sprites.Enemies.Visible();

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
            else if (Owner is SpriteEnemyBase enemy)
            {
                _engine.Player.Sprite.IsLockedOnSoft = false;
                _engine.Player.Sprite.IsLockedOnHard = false;

                if (Metadata.MaxLockDistance > 0 && Owner.IsPointingAt(_engine.Player.Sprite, Metadata.MaxLockOnAngle ?? 0))
                {
                    var distance = Owner.DistanceTo(_engine.Player.Sprite);
                    if (distance.IsBetween(Metadata.MinLockDistance ?? 0, Metadata.MaxLockDistance.Value))
                    {
                        _engine.Player.Sprite.IsLockedOnHard = true;
                        _engine.Player.Sprite.IsLockedOnSoft = false;

                        LockedTargets.Add(new WeaponsLock(_engine.Player.Sprite, Owner.DistanceTo(_engine.Player.Sprite))
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
                _engine.Sprites.Munitions.Add(this, location);

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
                _engine.Sprites.Munitions.Add(this);

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
