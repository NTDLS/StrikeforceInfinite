using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a particle sprite with customizable appearance, motion, and cleanup behavior for use in particle
    /// systems.
    /// </summary>
    /// <remarks>AeSpriteParticle provides configurable options for color, shape, movement, and automatic
    /// cleanup based on distance or fading. It supports both solid and gradient color patterns, multiple shapes, and
    /// cleanup modes such as fading to black or removal when traveling beyond a specified distance. This class is
    /// intended for use in visual effects and simulations where dynamic particle behavior is required.</remarks>
    [AssetClass("Particle", "", AeBaseAssetType.Image, true)]
    public class AeSpriteParticle
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
        /// Gets or sets the color pattern used for rendering particles.
        /// </summary>
        public AeParticleColorType Pattern { get; set; } = AeParticleColorType.Solid;

        /// <summary>
        /// Gets or sets the type of vector representation used for the particle.
        /// </summary>
        public AeParticleVectorType VectorType { get; set; } = AeParticleVectorType.Default;

        /// <summary>
        /// Gets or sets the shape used to render the particle.
        /// </summary>
        public AeParticleShape Shape { get; set; } = AeParticleShape.FilledEllipse;

        /// <summary>
        /// Gets or sets the cleanup mode used for particle management.
        /// </summary>
        /// <remarks>Use this property to specify how particles are removed or retained during simulation.
        /// The selected mode determines the criteria and timing for particle cleanup, which can affect performance and
        /// visual results.</remarks>
        public AeParticleCleanupMode CleanupMode { get; set; } = AeParticleCleanupMode.None;

        /// <summary>
        /// The color of the particle when ColorType == Color;
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// The color of the particle when ColorType == Gradient;
        /// </summary>
        public Color4 GradientStartColor { get; set; }

        /// <summary>
        /// The color of the particle when ColorType == Gradient;
        /// </summary>
        public Color4 GradientEndColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteParticle class at the specified location, size, and color.
        /// </summary>
        /// <remarks>The particle is initialized with random rotation speed, speed, and orientation. The
        /// throttle is set to 1 by default.</remarks>
        /// <param name="engine">The engine instance used to manage rendering and particle behavior.</param>
        /// <param name="location">The initial position of the particle in the engine's coordinate space.</param>
        /// <param name="size">The size of the particle, specified as a System.Drawing.Size.</param>
        /// <param name="color">The color of the particle. If null, the engine's default white color is used.</param>
        public AeSpriteParticle(AeEngine engine, AeVector location, Size size, Color4? color = null)
            : base(engine, null)
        {
            SetSize(size);

            Location = location.Clone();

            Color = color ?? engine.Rendering.Materials.Colors.White;
            RotationSpeed = AeRandom.Between(0.01f, 0.09f) * AeRandom.PositiveOrNegative();

            Speed = AeRandom.Between(100f, 400f);
            Orientation.Degrees = AeRandom.Between(0, 359);
            Throttle = 1;
        }

        /// <summary>
        /// Updates the particle's orientation, position, and color based on the specified time interval and camera
        /// displacement. Applies cleanup logic to remove the particle if it meets fade or distance criteria.
        /// </summary>
        /// <remarks>Particles may be removed if their color fades below a threshold or if they move
        /// beyond the maximum allowed distance from the visible canvas, depending on the configured cleanup
        /// mode.</remarks>
        /// <param name="epoch">The time interval, in seconds, over which to apply motion and effects to the particle.</param>
        /// <param name="cameraDisplacement">The displacement vector representing the camera's movement, used to adjust the particle's position relative
        /// to the camera.</param>
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
                if (Pattern == AeParticleColorType.Solid)
                {
                    Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
                else if (Pattern == AeParticleColorType.Gradient)
                {
                    GradientStartColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.
                    GradientEndColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (GradientStartColor.Red < 0.5f && GradientStartColor.Green < 0.5f && GradientStartColor.Blue < 0.5f
                        || GradientEndColor.Red < 0.5f && GradientEndColor.Green < 0.5f && GradientEndColor.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
            }
            else if (CleanupMode == AeParticleCleanupMode.DistanceOffScreen)
            {
                if (Engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }

        internal override void Render(RenderTarget renderTarget, float epoch)
        {
            if (IsVisible)
            {
                switch (Shape)
                {
                    case AeParticleShape.FilledEllipse:
                        if (Pattern == AeParticleColorType.Solid)
                        {
                            Engine.Rendering.DrawSolidEllipse(renderTarget,
                                RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, (float)Orientation.Degrees);
                        }
                        else if (Pattern == AeParticleColorType.Gradient)
                        {
                            Engine.Rendering.DrawGradientEllipse(renderTarget, RenderLocation.X, RenderLocation.Y,
                                Size.Width, Size.Height, GradientStartColor, GradientEndColor, (float)Orientation.Degrees);
                        }
                        break;
                    case AeParticleShape.HollowEllipse:
                        Engine.Rendering.DrawEllipse(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Orientation.Degrees);
                        break;

                    case AeParticleShape.FilledRectangle:
                        {
                            var rect = new RawRectangleF(0, 0, Size.Width, Size.Height);

                            if (Pattern == AeParticleColorType.Solid)
                            {
                                Engine.Rendering.DrawSolidRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                    RenderLocation.Y - Size.Height / 2, rect, Color, 0, (float)Orientation.Degrees);
                            }
                            else if (Pattern == AeParticleColorType.Gradient)
                            {
                                Engine.Rendering.DrawGradientRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                    RenderLocation.Y - Size.Height / 2, rect, GradientStartColor, GradientEndColor, 0, (float)Orientation.Degrees);
                            }
                        }
                        break;

                    case AeParticleShape.HollowRectangle:
                        {
                            var rect = new RawRectangleF(0, 0, Size.Width, Size.Height);
                            Engine.Rendering.DrawRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                RenderLocation.Y - Size.Height / 2, rect, Color, 0, 1, (float)Orientation.Degrees);

                        }
                        break;

                    case AeParticleShape.Triangle:
                        Engine.Rendering.DrawTriangle(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Orientation.Degrees);
                        break;
                }

                if (IsHighlighted)
                {
                    Engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        Engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
                }
            }
        }
    }
}
