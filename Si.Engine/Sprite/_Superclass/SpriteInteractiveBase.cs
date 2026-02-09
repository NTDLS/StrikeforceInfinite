using NTDLS.Helpers;
using SharpDX.Direct2D1;
using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.SupportingClasses;
using Si.Engine.Sprite.SupportingClasses.Metadata;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Si.Engine.Sprite._Superclass
{
    /// <summary>
    /// A sprite that the player can see, probably shoot and destroy and might even shoot back.
    /// </summary>
    public class SpriteInteractiveBase : SpriteBase
    {
        #region Locking Indicator.

        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        protected Bitmap _lockedOnImage;
        protected Bitmap _lockedOnSoftImage;
        private bool _isLockedOn = false;

        public bool IsLockedOnHard //The object is the subject of a foreign weapons lock.
        {
            get => _isLockedOn;
            set
            {
                if (_isLockedOn == false && value == true)
                {
                    //TODO: This should not play every loop.
                    _engine.Audio.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

        #endregion

        public SiRenewableResources RenewableResources { get; set; } = new();
        private InteractiveSpriteMetadata? _metadata = null;
        public InteractiveSpriteMetadata Metadata => _metadata ?? throw new NullReferenceException();
        public List<WeaponBase> Weapons { get; private set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="imagePath"></param>
        public SpriteInteractiveBase(EngineCore engine, string? imagePath)
            : base(engine)
        {
            _engine = engine;

            _lockedOnImage = _engine.Assets.GetBitmap(@"Sprites\Weapon\Locked On.png");
            _lockedOnSoftImage = _engine.Assets.GetBitmap(@"Sprites\Weapon\Locked Soft.png");

            if (imagePath != null)
            {
                SetImageAndLoadMetadata(imagePath);
            }
        }

        public SpriteInteractiveBase(EngineCore engine, Bitmap bitmap)
            : base(engine)
        {
            _engine = engine;

            _lockedOnImage = _engine.Assets.GetBitmap(@"Sprites\Weapon\Locked On.png");
            _lockedOnSoftImage = _engine.Assets.GetBitmap(@"Sprites\Weapon\Locked Soft.png");

            SetImage(bitmap);
        }

        #region Artificial Intelligence.

        public IAIController? CurrentAIController { get; set; }
        private readonly Dictionary<Type, IAIController> _aiControllers = new();

        public void AddAIController(IAIController controller)
            => _aiControllers.Add(controller.GetType(), controller);

        public void ClearAIControllers()
        {
            _aiControllers.Clear();
            CurrentAIController = null;
        }

        public IAIController GetAIController<T>() where T : IAIController
            => _aiControllers[typeof(T)];

        public void SetCurrentAIController<T>() where T : IAIController
        {
            CurrentAIController = GetAIController<T>();
        }

        #endregion

        /// <summary>
        /// Sets the sprites image, sets speed, shields, adds attachments and weapons
        /// from a .json file in the same path with the same name as the sprite image.
        /// </summary>
        /// <param name="spriteImagePath"></param>
        private void SetImageAndLoadMetadata(string spriteImagePath)
        {
            _metadata = _engine.Assets.GetMetaData<InteractiveSpriteMetadata>(spriteImagePath);

            SetImage(spriteImagePath);

            // Set standard variables here:
            Speed = Metadata.Speed;
            Throttle = Metadata.Throttle;
            MaxThrottle = Metadata.MaxThrottle;

            SetHullHealth(Metadata.Hull);
            SetShieldHealth(Metadata.Shields);

            Metadata.Weapons?.ForEach(weapon => {
                AddWeapon(weapon.Type.EnsureNotNull(), weapon.MunitionCount);
            });

            Metadata.Attachments?.ForEach(attachment => {
                AttachOfType(attachment.Type, attachment.LocationRelativeToOwner);
            });

            if (this is SpriteAttachment attach)
            {
                attach.OrientationType = Metadata.OrientationType;
                attach.PositionType = Metadata.PositionType;
            }

            if (this is SpritePlayerBase player)
            {
                if (Metadata?.PrimaryWeapon?.Type != null)
                {
                    player.SetPrimaryWeapon(Metadata.PrimaryWeapon.Type, Metadata.PrimaryWeapon.MunitionCount);
                    player.SelectFirstAvailableUsableSecondaryWeapon();
                }
            }
        }

        /// <summary>
        /// The total velocity multiplied by the given mass.
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public float TotalMomentum()
            => TotalVelocity * Metadata.Mass;

        /// <summary>
        /// Number that defines how much motion a sprite is in.
        /// </summary>
        public float TotalVelocity
            => OrientationMovementVector.SumAbs();

        /// <summary>
        /// The total velocity multiplied by the given mass, except for the mass is returned when the velocity is 0;
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public float TotalMomentumWithRestingMass()
        {
            var totalRelativeVelocity = TotalVelocity;
            if (totalRelativeVelocity == 0)
            {
                return Metadata.Mass;
            }
            return TotalVelocity * Metadata.Mass;
        }

        #region Weapons selection and evaluation.

        public void ClearWeapons() => Weapons.Clear();

        public void AddWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);
            if (weaponType == null)
            {
                throw new Exception($"The type '{weaponTypeName}' does not exist in the reflection cache.");
            }

            var weapon = Weapons.Where(o => o.GetType() == weaponType).SingleOrDefault();
            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, [_engine, this]).EnsureNotNull();
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public void AddWeapon<T>(int munitionCount) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceOf<T>([_engine, this]).EnsureNotNull();
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public int TotalAvailableWeaponRounds() => (from o in Weapons select o.RoundQuantity).Sum();
        public int TotalWeaponFiredRounds() => (from o in Weapons select o.RoundsFired).Sum();

        public bool HasWeapon<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasWeaponAndAmmo<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool FireWeapon<T>() where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire() == true;
        }

        public bool FireWeapon<T>(SiVector location) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire(location) == true;
        }

        public WeaponBase? GetWeaponOfType<T>() where T : WeaponBase
        {
            return Weapons.OfType<T>().FirstOrDefault();
        }

        #endregion

        #region Attachments.

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment Attach(string imagePath)
        {
            var attachment = _engine.Sprites.Attachments.Add(this, imagePath);
            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment Attach<T>(string imagePath) where T : SpriteAttachment
        {
            var attachment = _engine.Sprites.Attachments.AddTypeOf<T>(this, imagePath);
            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment AttachOfType<T>() where T : SpriteAttachment
        {
            var attachment = _engine.Sprites.Attachments.AddTypeOf<T>(this);
            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment AttachOfType(string typeName, SiVector locationRelativeToOwner)
        {
            var attachment = _engine.Sprites.Attachments.AddTypeOf(typeName, this, locationRelativeToOwner);
            Attachments.Add(attachment);
            return attachment;
        }

        #endregion

        public override void Render(RenderTarget renderTarget)
        {
            base.Render(renderTarget);

            if (IsVisible)
            {
                if (_lockedOnImage != null && IsLockedOnHard)
                {
                    DrawImage(renderTarget, _lockedOnImage, 0);
                }
                else if (_lockedOnImage != null && IsLockedOnSoft)
                {
                    DrawImage(renderTarget, _lockedOnSoftImage, 0);
                }
            }
        }

        public override bool TryMunitionHit(MunitionBase munition, SiVector hitTestPosition)
        {
            if (IntersectsAABB(hitTestPosition))
            {
                Hit(munition);
                if (HullHealth <= 0)
                {
                    Explode();
                }
                return true;
            }
            return false;
        }

        public override void Explode()
        {
            _engine.Events.Add(() =>
            {
                _engine.Sprites.Animations.AddRandomExplosionAt(this);
                _engine.Sprites.Particles.ParticleBlastAt(this, SiRandom.Between(200, 800));
                _engine.Sprites.CreateFragmentsOf(this);
                _engine.Rendering.AddScreenShake(4, 800);
                _engine.Audio.PlayRandomExplosion();
            });

            base.Explode();
        }

        /// <summary>
        /// Provides a way to make basic decisions about the sprite that do not necessarily have anything to do with movement.
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="displacementVector"></param>
        public virtual void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            CurrentAIController?.ApplyIntelligence(epoch, displacementVector);
            Weapons?.ForEach(o => o.ApplyIntelligence(epoch));
        }

        /// <summary>
        /// Performs collision detection for this one sprite using the passed in collection of collidable bodies.
        /// 
        /// This is called before ApplyMotion().
        /// </summary>
        public virtual void PerformCollisionDetection(float epoch)
        {
            if (!Metadata.CollisionDetection || IsDeadOrExploded || !IsVisible)
            {
                return;
            }

            //HEY PAT!
            // - [] This function (PerformCollisionDetection) is called before ApplyMotion().
            // - [] _engine.Collisions.Collidables contains all objects that have CollisionDetection enabled.
            // - [] Each element in collidables[] has a Position property which is the location where
            //      the sprite will be AFTER the next call to ApplyMotion() (e.g. the sprite has not
            //      yet moved but this will tell you where it will be when it next moves).
            //      We should? be able to use this to detect a collision and back each of the sprites
            //      velocities off... right?
            // - [x] Note that thisCollidable also contains the predicted location after the move.
            // - [] How the hell do we handle collateral collisions? Please tell me we don't have to iterate.... 
            // - [x] Turns out a big problem is going to be that each colliding sprite will have two separate handlers.
            //      this might make it difficult.... not sure yet.
            // - [x] I think we need to determine the angle of the "collider" and do the bounce math on that.
            // - [x] I added sprite mass, velocity and momentum. This should help us determine who's gonna get moved and by what amount.
            // - [x] One issue we have is that if a sprite is moving away from the collision, then this code
            //      will reverse that and move the sprite into the collision causing them to overlap and become stuck.

            //IsHighlighted = true;

            var thisCollidable = new PredictedKinematicBody(this, _engine.Display.RenderWindowPosition, epoch);

            foreach (var other in _engine.Collisions.Collidables)
            {
                if (thisCollidable.Sprite == other.Sprite || _engine.Collisions.IsAlreadyHandled(thisCollidable.Sprite, other.Sprite))
                {
                    continue;
                }

                if (thisCollidable.IntersectsSAT(other))
                {
                    //The items recorded to this collection are rendered to the screen via
                    //  EngineCore.RenderEverything() when Engine.Settings.HighlightCollisions is true.
                    var collisionPair = _engine.Collisions.CreateAndRecord(thisCollidable, other);

                    //Comment this out to see the collision overlaps.
                    RespondToCollisions(collisionPair);
                }
            }
        }

        /// <summary>
        /// Changes the movement vector of two sprites involved in a collision.
        /// </summary>
        /// <param name="collisionPair"></param>
        public void RespondToCollisions(OverlappingKinematicBodyPair collisionPair)
        {
            var A = collisionPair.Body1.Sprite;
            var B = collisionPair.Body2.Sprite;

            float mA = A.Metadata.Mass;
            float mB = B.Metadata.Mass;

            // normal from A -> B (pick one direction and stick to it).
            var n = (B.Location - A.Location).Normalize();

            var vA = A.OrientationMovementVector;
            var vB = B.OrientationMovementVector;

            var rv = vB - vA; // relative velocity of B w.r.t A
            float velAlongNormal = rv.Dot(n);

            if (velAlongNormal > 0f)
                return; // separating

            float restitution = 1.0f; // 1=perfectly elastic; try 0.2..0.8 for game-feel
            float invMassA = (mA <= 0f) ? 0f : 1f / mA;
            float invMassB = (mB <= 0f) ? 0f : 1f / mB;

            float j = -(1f + restitution) * velAlongNormal;
            j /= (invMassA + invMassB);

            var impulse = j * n;

            // Apply impulses
            A.OrientationMovementVector = vA - impulse * invMassA;
            B.OrientationMovementVector = vB + impulse * invMassB;

            // I don't want players to bounce too much.
            if (A is SpritePlayerBase) A.OrientationMovementVector = (A.OrientationMovementVector + vA) * 0.5f;
            if (B is SpritePlayerBase) B.OrientationMovementVector = (B.OrientationMovementVector + vB) * 0.5f;
        }
    }
}
