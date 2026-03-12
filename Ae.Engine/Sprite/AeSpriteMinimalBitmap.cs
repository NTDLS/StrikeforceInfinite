using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using System;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    [AssetClass("Bitmap", "", AeBaseAssetType.Image, true)]
    public class AeSpriteMinimalBitmap
        : AeSprite
    {
        /// <summary>
        /// The max travel distance from the creation x,y before the sprite is automatically deleted.
        /// This is ignored unless the CleanupModeOption is Distance.
        /// </summary>
        public float MaxDistance { get; set; } = 1000;

        /// <summary>
        /// The amount of brightness to reduce the color by each time the particle is rendered.
        /// This is ignored unless the CleanupModeOption is FadeToBlack.
        /// This should be expressed as a number between 0-1 with 0 being no reduction per frame and 1 being 100% reduction per frame.
        /// </summary>
        public float FadeToBlackReductionAmount { get; set; } = 0.01f;

        /// <summary>
        /// Gets or sets the type of vector representation used for the particle.
        /// </summary>
        public AeParticleVectorType VectorType { get; set; } = AeParticleVectorType.Default;

        /// <summary>
        /// Gets or sets the cleanup mode used for particle management.
        /// </summary>
        /// <remarks>Use this property to specify how particles are removed or retained during simulation.
        /// The selected mode determines the criteria and timing for particle cleanup, which can affect performance and
        /// visual results.</remarks>
        public AeParticleCleanupMode CleanupMode { get; set; } = AeParticleCleanupMode.None;

        /// <summary>
        /// Initializes a new instance of the AeSpriteMinimalBitmap class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage rendering and resource operations for the sprite.</param>
        /// <param name="assetKey">The unique key identifying the bitmap asset to be associated with this sprite.</param>
        public AeSpriteMinimalBitmap(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AeSpriteMinimalBitmap class using the specified engine and Direct2D
        /// bitmap.
        /// </summary>
        /// <param name="engine">The engine instance that manages rendering and sprite operations.</param>
        /// <param name="bitmap">The Direct2D bitmap to associate with the sprite. Cannot be null.</param>
        public AeSpriteMinimalBitmap(AeEngine engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine, null)
        {
            SetBitmap(bitmap);
        }

        /// <summary>
        /// Updates the particle's orientation and position based on the specified time interval and camera
        /// displacement, and applies cleanup logic according to the configured mode.
        /// </summary>
        /// <remarks>If the cleanup mode is set to DistanceOffScreen, the particle will be queued for
        /// deletion when it moves outside the maximum allowed distance from the visible canvas. The method also
        /// recalculates the movement vector when the particle is configured to follow its orientation.</remarks>
        /// <param name="epoch">The time interval, in seconds, over which to apply motion updates to the particle.</param>
        /// <param name="cameraDisplacement">The displacement vector representing the camera's movement, used to adjust the particle's position relative
        /// to the camera.</param>
        /// <exception cref="NotImplementedException">Thrown if the cleanup mode is set to FadeToBlack, as this mode is not currently implemented.</exception>
        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            Orientation.Degrees += RotationSpeed * epoch;

            if (VectorType == AeParticleVectorType.FollowOrientation)
            {
                RecalculateMovementVectorFromAngle(Orientation.RadiansSigned);
            }

            base.ApplyMotion(epoch, cameraDisplacement);

            if (CleanupMode == AeParticleCleanupMode.FadeToBlack)
            {
                throw new NotImplementedException();
                /*
                Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                // Check if the particle color is below a certain threshold and remove it.
                if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                {
                    QueueForDelete();
                }
                */
            }
            else if (CleanupMode == AeParticleCleanupMode.DistanceOffScreen)
            {
                if (Engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }
    }
}
