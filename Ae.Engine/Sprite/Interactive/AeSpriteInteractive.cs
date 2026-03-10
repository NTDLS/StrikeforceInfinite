using Ae.Engine.AI;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Mathematics.KinematicBody;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Munition;
using Microsoft.CodeAnalysis;
using NTDLS.Helpers;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Interactive
{
    /// <summary>
    /// A sprite that can have weapons, be controlled by an AI controller,
    /// be the subject of a foreign weapons lock and be hit by munitions.
    /// They can also be subject to collision detection and have a mass that affects how they respond to collisions.
    /// </summary>
    [AssetClass("Interactive", "", AeBaseAssetType.Image, true)]
    public class AeSpriteInteractive
        : AeSprite
    {
        #region Locking Indicator.

        /// <summary>
        /// This is just graphics candy, the sprite would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        /// </summary>
        public bool IsLockedOnSoft { get; set; }
        protected Bitmap? _lockedOnImage;
        protected Bitmap? _lockedOnSoftImage;
        private bool _isLockedOn = false;

        /// <summary>
        /// The sprite is the subject of a foreign weapons lock.
        /// </summary>
        public bool IsLockedOn
        {
            get => _isLockedOn;
            set
            {
                if (_isLockedOn == false && value == true)
                {
                    //TODO: This should not play every loop.
                    Engine.Audio.LockedOnBlip?.Play();
                }
                _isLockedOn = value;
            }
        }

        #endregion

        public float Mass { get; set; }

        public AeRenewableResources RenewableResources { get; set; } = new();
        public List<AeSpriteWeapon> Weapons { get; private set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="assetKey"></param>
        public AeSpriteInteractive(AeEngine engine, string? assetKey)
            : base(engine, assetKey)
        {
            Mass = AeRandom.Between(Metadata.Mass, 0);

            if (Engine.Assets.IsLoaded)
            {
                _lockedOnImage = Engine.Assets.GetBitmap("Sprites/Weapon/Locking/Locked On");
                _lockedOnSoftImage = Engine.Assets.GetBitmap("Sprites/Weapon/Locking/Locked Soft");
            }
            SetupAIControllers();
        }

        public AeSpriteInteractive(AeEngine engine, Bitmap bitmap)
            : base(engine, null)
        {
            if (Engine.Assets.IsLoaded)
            {
                _lockedOnImage = Engine.Assets.GetBitmap("Sprites/Weapon/Locking/Locked On.png");
                _lockedOnSoftImage = Engine.Assets.GetBitmap("Sprites/Weapon/Locking/Locked Soft.png");
            }

            SetBitmap(bitmap);
            SetupAIControllers();
        }

        #region Artificial Intelligence.

        /// <summary>
        /// The current AI controller that is controlling the sprite's ApplyIntelligence() behavior, this can be switched at any time to change the sprite's behavior.
        /// </summary>
        public AeIAIController? CurrentAIController { get; private set; }

        /// <summary>
        /// Dictionary of AI controller AssetKeys and their instances.
        /// </summary>
        private readonly Dictionary<string, AeIAIController> _aiControllers = new();

        public void SetupAIControllers()
        {
            if (Metadata.AIControllers != null)
            {
                foreach (var aiControllerAssetKey in Metadata.AIControllers)
                {
                    var aiControllerMetadata = Engine.Assets.GetMetadata(aiControllerAssetKey);

                    var aiControllerType = AeReflection.GetTypeByName(aiControllerMetadata.DynamicTypeName ??
                        throw new Exception($"The AI controller '{aiControllerAssetKey}' does not have a DynamicTypeName defined in its metadata."));

                    var aiController = Activator.CreateInstance(aiControllerType, [Engine, this]) as AeIAIController
                        ?? throw new Exception($"The AI controller class '{aiControllerAssetKey}' could not be instantiated for sprite '{Metadata.AssetKey}'.");

                    _aiControllers.Add(aiControllerAssetKey, aiController);
                }

                //If a default AI controller is specified, set it as the current controller.
                if (string.IsNullOrEmpty(Metadata.DefaultAIController) == false)
                {
                    SetCurrentAIController(Metadata.DefaultAIController);
                }
            }
        }

        /// <summary>
        /// Sets the current AI controller for the sprite based on the specified asset key.
        /// </summary>
        /// <param name="aiControllerAssetKey">The asset key of the AI controller to set as the current controller. This key must correspond to an existing
        /// AI controllers AssetKey in the collection.</param>
        /// <exception cref="Exception">Thrown if the AI controller specified by <paramref name="aiControllerAssetKey"/> does not exist for the
        /// current sprite.</exception>
        public void SetCurrentAIController(string aiControllerAssetKey)
        {
            if (!_aiControllers.TryGetValue(aiControllerAssetKey, out var aiController) && aiController != null)
            {
                throw new Exception($"The AI controller '{aiControllerAssetKey}' does not exist for sprite '{Metadata.AssetKey}'.");
            }
            CurrentAIController = aiController;
        }

        /// <summary>
        /// Clears all AI controllers and clears the currently selected AI controller.
        /// </summary>
        public void ClearAIControllers()
        {
            _aiControllers.Clear();
            CurrentAIController = null;
        }

        #endregion

        /// <summary>
        /// The total velocity multiplied by the given mass.
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public float TotalMomentum()
            => TotalVelocity * Mass;

        /// <summary>
        /// Number that defines how much motion a sprite is in.
        /// </summary>
        public float TotalVelocity
            => MovementVector.SumAbs();

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
                return Mass;
            }
            return TotalVelocity * Mass;
        }

        #region Weapons selection and evaluation.

        public void ClearWeapons() => Weapons.Clear();

        public void AddWeapon(string assetKey, int munitionCount)
        {
            var asset = Engine.Assets.GetAsset(assetKey)
                ?? throw new Exception($"The metadata for the weapon sprite '{assetKey}' does not exist.");

            var weapon = Weapons.SingleOrDefault(o => o.Metadata?.Name == asset.Metadata.Name);
            if (weapon == null)
            {
                var className = (string.IsNullOrEmpty(asset.ControllerName) ? asset.Metadata.Class : asset.ControllerName)
                    ?? throw new Exception($"The sprite {assetKey} does not have a class or controller defined in its metadata.");
                var type = AeReflection.GetTypeByName(className);

                weapon = (AeSpriteWeapon)Activator.CreateInstance(type, [Engine, this, assetKey]).EnsureNotNull();
                weapon.MunitionQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.MunitionQuantity += munitionCount;
            }
        }

        public int TotalAvailableWeaponMunitions() => (from o in Weapons select o.MunitionQuantity).Sum();
        public int TotalWeaponFiredMunitions() => (from o in Weapons select o.MunitionsFired).Sum();

        public bool HasWeapon(string assetKey)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey) != null;

        public bool HasWeaponAndAmmo(string assetKey)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey)?.MunitionQuantity > 0;

        public bool FireWeapon(string assetKey)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey)?.Fire() == true;

        public bool FireWeapon(string assetKey, AeVector location)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey)?.Fire(location) == true;

        #endregion

        #region Attachments.

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another
        /// sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public AeSpriteAttachment AttachOfType(string assetKey, AeVector locationRelativeToOwner, Action<AeSpriteAttachment>? initializationProc = null)
        {
            var attachment = Engine.Sprites.Attachments.AddAttachment(assetKey, this, locationRelativeToOwner);
            initializationProc?.Invoke(attachment);
            Attachments.Add(attachment);
            return attachment;
        }

        #endregion

        public override void Render(RenderTarget renderTarget, float epoch)
        {
            base.Render(renderTarget, epoch);

            if (IsVisible)
            {
                if (_lockedOnImage != null && IsLockedOn)
                {
                    DrawImage(renderTarget, _lockedOnImage, 0);
                }
                else if (_lockedOnSoftImage != null && IsLockedOnSoft)
                {
                    DrawImage(renderTarget, _lockedOnSoftImage, 0);
                }
            }
        }

        public override bool TryMunitionHit(AeSpriteMunition munition, AeVector hitTestPosition)
        {
            if (IntersectsAabb(hitTestPosition))
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
            Engine.Events.Add(() =>
            {
                switch (Metadata.ExplosionType)
                {
                    case ExplosionType.MediumFire:
                        Engine.Sprites.Animations.AddRandomMediumFireExplosionAt(this);
                        break;
                    case ExplosionType.LargeFire:
                        Engine.Sprites.Animations.AddRandomLargeFireExplosionAt(this);
                        break;
                    case ExplosionType.SmallFire:
                        Engine.Sprites.Animations.AddRandomSmallFireExplosionAt(this);
                        break;
                    case ExplosionType.MicroFire:
                        Engine.Sprites.Animations.AddRandomMicroFireExplosionAt(this);
                        break;
                    case ExplosionType.Energy:
                        Engine.Sprites.Animations.AddRandomEnergyExplosionAt(this);
                        break;
                }

                if (Metadata.ParticleBlastOnExplodeAmount?.IsValid() == true)
                    Engine.Sprites.Particles.ParticleBlastAt(this, AeRandom.Between(Metadata.ParticleBlastOnExplodeAmount.Min, Metadata.ParticleBlastOnExplodeAmount.Max));

                if (Metadata.FragmentOnExplode == true)
                    Engine.Sprites.CreateFragmentsOf(this);

                if (Metadata.ScreenShakeOnExplodeAmount?.IsValid() == true)
                    Engine.Rendering.AddScreenShake(Metadata.ScreenShakeOnExplodeAmount.Min, Metadata.ScreenShakeOnExplodeAmount.Max);

                Engine.Audio.PlayRandomExplosion();
            });

            base.Explode();
        }

        /// <summary>
        /// Provides a way to make basic decisions about the sprite that do not necessarily have anything to do with movement.
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="cameraDisplacement"></param>
        public virtual void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            CurrentAIController?.ApplyIntelligence(epoch, cameraDisplacement);
            Weapons?.ForEach(o => o.ApplyIntelligence(epoch));
        }

        /// <summary>
        /// Performs collision detection for this one sprite using the passed in collection of collidable bodies.
        /// 
        /// This is called before ApplyMotion().
        /// </summary>
        public virtual void PerformCollisionDetection(float epoch)
        {
            if (Metadata.CollisionDetection != true || IsDeadOrExploded || !IsVisible)
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

            var thisCollidable = new PredictedKinematicBody(this, Engine.Display.CameraPosition, epoch);

            /// It is important to remeber that need to verify the visibility of sprites that are colliding
            ///     because the collection of collidables is a snapshot from the start of the tick and the
            ///     visibility can change between that snapshot and this calculation.
            foreach (var other in Engine.Collisions.Collidables.Where(o => o.Sprite.IsVisible))
            {
                if (thisCollidable.Sprite == other.Sprite || Engine.Collisions.IsAlreadyHandled(thisCollidable.Sprite, other.Sprite))
                {
                    continue;
                }

                if (thisCollidable.IntersectsSAT(other))
                {
                    //The items recorded to this collection are rendered to the screen via
                    //  EngineCore.RenderEverything() when Engine.Settings.HighlightCollisions is true.
                    var collisionPair = Engine.Collisions.CreateAndRecord(thisCollidable, other);

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

            float mA = A.Mass;
            float mB = B.Mass;

            // normal from A -> B (pick one direction and stick to it).
            var n = (B.Location - A.Location).Normalize();

            var vA = A.MovementVector;
            var vB = B.MovementVector;

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
            A.MovementVector = vA - impulse * invMassA;
            B.MovementVector = vB + impulse * invMassB;

            // I don't want players to bounce too much.
            if (A is AeSpritePlayer) A.MovementVector = (A.MovementVector + vA) * 0.5f;
            if (B is AeSpritePlayer) B.MovementVector = (B.MovementVector + vB) * 0.5f;
        }
    }
}
