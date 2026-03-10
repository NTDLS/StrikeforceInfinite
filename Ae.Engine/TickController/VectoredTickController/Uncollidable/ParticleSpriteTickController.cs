using Ae.Engine.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Rendering;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using SharpDX;
using System;
using System.Drawing;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class ParticleSpriteTickController
        : VectoredTickControllerBase<AeSpriteParticle>
    {
        public ParticleSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, cameraDisplacement);
                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(particle.GetMultiPlayActionVector());
            }
        }

        public void AddAt(AeVector location, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(location + AeRandom.Between(-20, 20), color, size);
            }
        }

        public void AddAt(AeSprite sprite, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(sprite.Location + AeRandom.Between(-20, 20), color, size);
            }
        }

        public AeSpriteParticle AddAt(AeSprite sprite, Color4 color, Size? size = null)
        {
            var obj = new AeSpriteParticle(Engine, sprite.Location, size ?? new Size(1, 1), color);
            SpriteManager.Insert(obj);
            return obj;
        }

        public AeSpriteParticle AddAt(AeVector location, Color4 color, Size? size = null)
        {
            var obj = new AeSpriteParticle(Engine, location, size ?? new Size(1, 1), color)
            {
                IsVisible = true
            };
            SpriteManager.Insert(obj);
            return obj;
        }

        public AeSpriteParticle AddAt(AeVector location, Size? size = null)
        {
            var obj = new AeSpriteParticle(Engine, location, size ?? new Size(1, 1))
            {
                IsVisible = true
            };
            SpriteManager.Insert(obj);
            return obj;
        }

        public void ParticleBlastAt(AeSprite at, int maxParticleCount)
        {
            Engine.Events.Add(() => ParticleBlastAt(at.Location, maxParticleCount));
        }

        /// <summary>
        /// Creates a random number of blasts consisting of "hot" colored particles at a given location.
        /// </summary>
        /// <param name="maxParticleCount"></param>
        /// <param name="at"></param>
        public void ParticleBlastAt(AeVector location, int maxParticleCount)
        {
            for (int i = 0; i < AeRandom.Between(maxParticleCount / 2, maxParticleCount); i++)
            {
                var particle = AddAt(location, new Size(AeRandom.Between(1, 2), AeRandom.Between(1, 2)));
                particle.Shape = ParticleShape.FilledEllipse;
                particle.Pattern = ParticleColorType.Solid;
                //particle.GradientStartColor = SiRenderingUtility.GetRandomHotColor();
                //particle.GradientEndColor = SiRenderingUtility.GetRandomHotColor();
                particle.Color = AeRenderingUtility.GetRandomHotColor();
                particle.CleanupMode = ParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = AeRandom.Between(0.001f, 0.01f);
                particle.Speed *= AeRandom.Between(1, 3.5f);
                particle.VectorType = ParticleVectorType.Default;
            }
        }

        /// <summary>
        /// Emits a cone-shaped burst of particles from the specified world position, with configurable direction,
        /// spread, speed, color, and other properties.
        /// </summary>
        /// <remarks>The spread and center bias parameters allow for fine control over the appearance of
        /// the particle cone, enabling effects ranging from wide sprays to tightly focused bursts.</remarks>
        /// <param name="nozzleWorldPos">The world position from which the particles are emitted.</param>
        /// <param name="centerDirectionDeg">The central direction, in degrees, along which the cone is oriented. Particles are emitted around this
        /// direction.</param>
        /// <param name="spreadDeg">The half-angle of the cone, in degrees. Determines how widely the particles spread from the center
        /// direction.</param>
        /// <param name="count">The number of particles to emit in the cone.</param>
        /// <param name="minSpeed">The minimum speed assigned to emitted particles.</param>
        /// <param name="maxSpeed">The maximum speed assigned to emitted particles.</param>
        /// <param name="color">The color applied to each emitted particle.</param>
        /// <param name="size">The size of each particle. If null, a default size is used.</param>
        /// <param name="centerBias">Controls how tightly particles cluster around the center direction. A value of 1 emits particles uniformly
        /// within the cone; values greater than 1 bias particles more toward the center.</param>
        public void EmitConeAt(
            AeVector nozzleWorldPos,
            float centerDirectionDeg,   // direction the particles should travel
            float spreadDeg,            // half-angle of cone
            int count,
            float minSpeed,
            float maxSpeed,
            Color4 color,
            Size? size = null,
            float centerBias = 2.0f     // 1 = uniform, >1 = tighter around center
        )
        {
            for (int i = 0; i < count; i++)
            {
                // Bias the angle toward 0 (centerline).
                float t = (float)AeRandom.Between(0, 10000) / 10000f;  // 0..1
                float signed = (float)AeRandom.Between(-10000, 10000) / 10000f; // -1..1

                // bias: raise to power -> more weight near 0
                float biased = MathF.Sign(signed) * MathF.Pow(MathF.Abs(signed), centerBias);

                float angle = centerDirectionDeg + biased * spreadDeg;

                var p = AddAt(nozzleWorldPos, color, size ?? new Size(2, 2));

                p.IsVisible = true;
                p.VectorType = ParticleVectorType.FollowOrientation;
                p.Orientation.Degrees = AeMath.WrapDegreesUnsigned(angle);
                p.Speed = AeRandom.Between(minSpeed, maxSpeed);
                p.Shape = ParticleShape.FilledEllipse;
                p.Pattern = ParticleColorType.Solid;
                p.CleanupMode = ParticleCleanupMode.FadeToBlack;
                p.FadeToBlackReductionAmount = AeRandom.Between(0.01f, 0.02f);
                p.RotationSpeed = AeRandom.Between(-250f, 250f);
            }
        }

        public void ParticleCloud(int particleCount, AeSprite at)
            => ParticleCloud(particleCount, at.Location);

        public void ParticleCloud(int particleCount, AeVector location)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var particle = AddAt(location, AeRenderingUtility.GetRandomHotColor(), new Size(5, 5));

                switch (AeRandom.Between(1, 3))
                {
                    case 1:
                        particle.Shape = ParticleShape.Triangle;
                        break;
                    case 2:
                        particle.Shape = ParticleShape.FilledEllipse;
                        break;
                    case 3:
                        particle.Shape = ParticleShape.HollowEllipse;
                        break;
                }

                particle.CleanupMode = ParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = 0.001f;
                particle.RotationSpeed = AeRandom.Between(-25f, 25f);
                particle.VectorType = ParticleVectorType.FollowOrientation;
                particle.Orientation.Degrees = AeRandom.Between(0.0f, 359.0f);
                particle.Speed = AeRandom.Between(20, 350f);
            }
        }
    }
}
