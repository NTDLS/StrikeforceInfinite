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
        /// <summary>
        /// Represents the image associated with the locked-on state. May be null if no image is set.
        /// </summary>
        /// <remarks>This field is intended for use by derived classes to manage or display a locked-on
        /// visual indicator. Access should be controlled to ensure thread safety if used in multi-threaded
        /// scenarios.</remarks>
        protected Bitmap? _lockedOnImage;
        /// <summary>
        /// Represents the bitmap instance currently locked for operations on the soft image.
        /// </summary>
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
                    Engine.Audio.LockedOnBlip();
                }
                _isLockedOn = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the mass of the object - defaults from the metadata but can be changed at any time.
        /// This is used for collision response calculations.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Gets or sets the collection of renewable resources associated with the entity.
        /// </summary>
        public AeRenewableResources RenewableResources { get; set; } = new();

        /// <summary>
        /// Gets the collection of weapons associated with the sprite.
        /// </summary>
        public List<AeSpriteWeapon> Weapons { get; private set; } = new();

        /// <summary>
        /// Initializes a new instance of the AeSpriteInteractive class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance that manages game state and resources for this sprite.</param>
        /// <param name="assetKey">The asset key identifying the sprite's visual resources. Can be null to use default assets.</param>
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

        /// <summary>
        /// Initializes a new instance of the AeSpriteInteractive class with the specified engine and bitmap.
        /// </summary>
        /// <remarks>If the engine's assets are already loaded, additional images related to sprite
        /// locking are initialized. The constructor also sets up AI controllers for the sprite.</remarks>
        /// <param name="engine">The engine instance used to manage assets and game logic for this sprite.</param>
        /// <param name="bitmap">The bitmap image to be used as the visual representation of the sprite.</param>
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
        public AeAIStateMachine? CurrentAIController { get; private set; }

        /// <summary>
        /// Dictionary of AI controller AssetKeys and their instances.
        /// </summary>
        private readonly Dictionary<string, AeAIStateMachine> _aiControllers = new();

        private void SetupAIControllers()
        {
            if (Metadata.AIControllers != null)
            {
                foreach (var aiControllerAssetKey in Metadata.AIControllers)
                {
                    var aiControllerMetadata = Engine.Assets.GetMetadata(aiControllerAssetKey);

                    var aiControllerType = AeReflection.GetTypeByName(aiControllerMetadata.DynamicTypeName ??
                        throw new Exception($"The AI controller '{aiControllerAssetKey}' does not have a DynamicTypeName defined in its metadata."));

                    var aiController = Activator.CreateInstance(aiControllerType, [Engine, this]) as AeAIStateMachine
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

        /// <summary>
        /// Removes all weapons from the collection.
        /// </summary>
        /// <remarks>Use this method to reset the weapons list to an empty state. After calling this
        /// method, the collection will contain no items.</remarks>
        public void ClearWeapons() => Weapons.Clear();

        /// <summary>
        /// Adds a weapon to the collection using the specified asset key and munition count. If the weapon already
        /// exists, increases its munition quantity.
        /// </summary>
        /// <param name="assetKey">The unique key identifying the weapon asset to add. Must correspond to a valid asset in the engine's asset
        /// collection.</param>
        /// <param name="munitionCount">The number of munitions to assign to the weapon. Must be a non-negative integer.</param>
        /// <exception cref="Exception">Thrown if the asset metadata for the specified asset key does not exist, or if the asset does not have a
        /// class or controller defined in its metadata.</exception>
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

        /// <summary>
        /// Calculates the total quantity of available munitions across all weapons.
        /// </summary>
        /// <returns>The sum of munitions available for all weapons. Returns 0 if no weapons are present.</returns>
        public int TotalAvailableWeaponMunitions() => (from o in Weapons select o.MunitionQuantity).Sum();

        /// <summary>
        /// Calculates the total number of munitions fired by all weapons.
        /// </summary>
        /// <returns>The sum of munitions fired across all weapons. Returns 0 if there are no weapons.</returns>
        public int TotalWeaponFiredMunitions() => (from o in Weapons select o.MunitionsFired).Sum();

        /// <summary>
        /// Determines whether the collection contains a weapon with the specified asset key.
        /// </summary>
        /// <param name="assetKey">The unique asset key identifying the weapon to search for. Cannot be null.</param>
        /// <returns>true if a weapon with the specified asset key exists in the collection; otherwise, false.</returns>
        public bool HasWeapon(string assetKey)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey) != null;

        /// <summary>
        /// Determines whether the specified weapon exists and has available ammunition.
        /// </summary>
        /// <param name="assetKey">The asset key identifying the weapon to check. Cannot be null.</param>
        /// <returns>true if the weapon with the specified asset key exists and has ammunition; otherwise, false.</returns>
        public bool HasWeaponAndAmmo(string assetKey)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey)?.MunitionQuantity > 0;

        /// <summary>
        /// Attempts to fire the weapon associated with the specified asset key.
        /// </summary>
        /// <remarks>If no weapon with the specified asset key exists, or if the weapon cannot be fired,
        /// the method returns false. This method does not throw an exception if the asset key is not found.</remarks>
        /// <param name="assetKey">The unique asset key identifying the weapon to fire. Cannot be null or empty.</param>
        /// <returns>true if the weapon was found and successfully fired; otherwise, false.</returns>
        public bool FireWeapon(string assetKey)
            => Weapons.SingleOrDefault(o => o.Metadata?.AssetKey == assetKey)?.Fire() == true;

        /// <summary>
        /// Attempts to fire the weapon identified by the specified asset key at the given location.
        /// </summary>
        /// <remarks>If no weapon matching the specified asset key is found, or if the weapon cannot be
        /// fired at the given location, the method returns false.</remarks>
        /// <param name="assetKey">The unique asset key that identifies the weapon to fire. Cannot be null.</param>
        /// <param name="location">The target location where the weapon should be fired.</param>
        /// <returns>true if the weapon was successfully fired; otherwise, false.</returns>
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

        internal override void Render(RenderTarget renderTarget, float epoch)
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

        /// <summary>
        /// Attempts to register a hit on the object by the specified munition at the given position.
        /// </summary>
        /// <remarks>If the hit is successful and the object's hull health reaches zero or below, the
        /// object will explode. This method does not throw exceptions for invalid input; callers should ensure
        /// parameters are valid.</remarks>
        /// <param name="munition">The munition attempting to hit the object. Must not be null.</param>
        /// <param name="hitTestPosition">The position to test for a hit, typically representing the impact location.</param>
        /// <returns>true if the munition successfully hits the object; otherwise, false.</returns>
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

        /// <summary>
        /// Triggers the explosion effects for this object, including visual, audio, and particle effects based on its
        /// metadata.
        /// </summary>
        /// <remarks>Explosion effects are determined by the object's metadata, such as explosion type,
        /// particle blast amount, fragment creation, and screen shake intensity. This method schedules the explosion
        /// effects to occur and invokes the base implementation. The effects may include random explosion animations,
        /// particle blasts, fragment creation, screen shake, and explosion sounds.</remarks>
        public override void Explode()
        {
            Engine.Events.Add(() =>
            {
                switch (Metadata.ExplosionType)
                {
                    case AeExplosionType.MediumFire:
                        Engine.Sprites.Animations.AddRandomMediumFireExplosionAt(this);
                        break;
                    case AeExplosionType.LargeFire:
                        Engine.Sprites.Animations.AddRandomLargeFireExplosionAt(this);
                        break;
                    case AeExplosionType.SmallFire:
                        Engine.Sprites.Animations.AddRandomSmallFireExplosionAt(this);
                        break;
                    case AeExplosionType.MicroFire:
                        Engine.Sprites.Animations.AddRandomMicroFireExplosionAt(this);
                        break;
                    case AeExplosionType.Energy:
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

            // It is important to remeber that need to verify the visibility of sprites that are colliding
            //     because the collection of collidables is a snapshot from the start of the tick and the
            //     visibility can change between that snapshot and this calculation.
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
        internal void RespondToCollisions(OverlappingKinematicBodyPair collisionPair)
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
