using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Drawing;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite
{
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
        public AeParticleColorType Pattern { get; set; } = AeParticleColorType.Solid;
        public AeParticleVectorType VectorType { get; set; } = AeParticleVectorType.Default;
        public AeParticleShape Shape { get; set; } = AeParticleShape.FilledEllipse;
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

        public override void Render(RenderTarget renderTarget, float epoch)
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
