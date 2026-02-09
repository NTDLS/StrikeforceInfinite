using SharpDX;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class ParticleSpriteTickController : VectoredTickControllerBase<SpriteParticleBase>
    {
        public ParticleSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
                Engine.MultiplayLobby?.ActionBuffer.RecordVector(particle.GetMultiPlayActionVector());
            }
        }

        public void AddAt(SiVector location, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(location + SiRandom.Between(-20, 20), color, size);
            }
        }

        public void AddAt(SpriteBase sprite, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(sprite.Location + SiRandom.Between(-20, 20), color, size);
            }
        }

        public SpriteParticle AddAt(SpriteBase sprite, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, sprite.Location, size ?? new Size(1, 1), color);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle AddAt(SiVector location, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, location, size ?? new Size(1, 1), color)
            {
                IsVisible = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle AddAt(SiVector location, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, location, size ?? new Size(1, 1))
            {
                IsVisible = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        public void ParticleBlastAt(SpriteBase at, int maxParticleCount)
        {
            Engine.Events.Add(() => ParticleBlastAt(at.Location, maxParticleCount));
        }

        /// <summary>
        /// Creates a random number of blasts consisting of "hot" colored particles at a given location.
        /// </summary>
        /// <param name="maxParticleCount"></param>
        /// <param name="at"></param>
        public void ParticleBlastAt(SiVector location, int maxParticleCount)
        {
            for (int i = 0; i < SiRandom.Between(maxParticleCount / 2, maxParticleCount); i++)
            {
                var particle = AddAt(location, new Size(SiRandom.Between(1, 2), SiRandom.Between(1, 2)));
                particle.Shape = ParticleShape.FilledEllipse;
                particle.Pattern = ParticleColorType.Solid;
                //particle.GradientStartColor = SiRenderingUtility.GetRandomHotColor();
                //particle.GradientEndColor = SiRenderingUtility.GetRandomHotColor();
                particle.Color = SiRenderingUtility.GetRandomHotColor();
                particle.CleanupMode = ParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = SiRandom.Between(0.001f, 0.01f);
                particle.Speed *= SiRandom.Between(1, 3.5f);
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
            SiVector nozzleWorldPos,
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
                float t = (float)SiRandom.Between(0, 10000) / 10000f;  // 0..1
                float signed = (float)SiRandom.Between(-10000, 10000) / 10000f; // -1..1

                // bias: raise to power -> more weight near 0
                float biased = MathF.Sign(signed) * MathF.Pow(MathF.Abs(signed), centerBias);

                float angle = centerDirectionDeg + biased * spreadDeg;

                var p = AddAt(nozzleWorldPos, color, size ?? new Size(2, 2));

                p.IsVisible = true;
                p.VectorType = ParticleVectorType.FollowOrientation;
                p.Orientation.Degrees = SiMath.WrapDegreesUnsigned(angle);
                p.Speed = SiRandom.Between(minSpeed, maxSpeed);
                p.Shape = ParticleShape.FilledEllipse;
                p.Pattern = ParticleColorType.Solid;
                p.CleanupMode = ParticleCleanupMode.FadeToBlack;
                p.FadeToBlackReductionAmount = SiRandom.Between(0.01f, 0.02f);
                p.RotationSpeed = SiRandom.Between(-250f, 250f);
            }
        }

        public void ParticleCloud(int particleCount, SpriteBase at)
            => ParticleCloud(particleCount, at.Location);

        public void ParticleCloud(int particleCount, SiVector location)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var particle = AddAt(location, SiRenderingUtility.GetRandomHotColor(), new Size(5, 5));

                switch (SiRandom.Between(1, 3))
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
                particle.RotationSpeed = SiRandom.Between(-25f, 25f);
                particle.VectorType = ParticleVectorType.FollowOrientation;
                particle.Orientation.Degrees = SiRandom.Between(0.0f, 359.0f);
                particle.Speed = SiRandom.Between(20, 350f);
            }
        }
    }
}
