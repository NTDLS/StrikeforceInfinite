using Ae.Engine.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Rendering;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using SharpDX;
using System;
using System.Drawing;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the creation, emission, and motion of sprite-based particles within the game world, providing methods
    /// for generating particle effects such as blasts, cones, and clouds.
    /// </summary>
    /// <remarks>Use this controller to add, emit, or animate particles for visual effects. Supports
    /// configurable emission patterns, colors, sizes, and cleanup modes. Designed for integration with multiplayer
    /// action recording and advanced particle behaviors. Thread safety and performance depend on the underlying engine
    /// and sprite manager implementations.</remarks>
    public class ParticleSpriteTickController
        : VectoredTickControllerBase<AeSpriteParticle>
    {
        /// <summary>
        /// Initializes a new instance of the ParticleSpriteTickController class to manage particle sprite updates
        /// within the specified engine and sprite manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides the context for particle sprite operations. Cannot be null.</param>
        /// <param name="manager">The sprite manager responsible for handling sprite objects. Cannot be null.</param>
        public ParticleSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Updates the motion state of all visible particles for the current world clock tick and records their
        /// multiplayer action vectors.
        /// </summary>
        /// <remarks>This method applies motion updates to each visible particle and records their action
        /// vectors for multiplayer synchronization. It should be called once per world clock tick to ensure consistent
        /// state across clients.</remarks>
        /// <param name="epoch">The current time value, in seconds, representing the world clock tick to apply motion updates.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust particle motion calculations for the current tick.</param>
        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, cameraDisplacement);
                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(particle.GetMultiPlayActionVector());
            }
        }

        /// <summary>
        /// Adds multiple items at or near the specified location with the given color and optional size.
        /// </summary>
        /// <remarks>Each item is placed at a randomly offset position within a range of -20 to 20 units
        /// from the base location. This method is useful for adding clusters of items with slight positional
        /// variation.</remarks>
        /// <param name="location">The base location where items will be added. Each item will be placed at a position offset randomly from
        /// this location.</param>
        /// <param name="color">The color to apply to each added item.</param>
        /// <param name="count">The number of items to add. Must be non-negative.</param>
        /// <param name="size">The optional size to apply to each item. If not specified, a default size will be used.</param>
        public void AddAt(AeVector location, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(location + AeRandom.Between(-20, 20), color, size);
            }
        }

        /// <summary>
        /// Adds multiple sprites at randomized locations near the specified sprite's position.
        /// </summary>
        /// <remarks>Each sprite is placed at a location offset randomly within a range of -20 to 20 units
        /// from the base sprite's position.</remarks>
        /// <param name="sprite">The sprite whose location is used as the base for placement.</param>
        /// <param name="color">The color to apply to each added sprite.</param>
        /// <param name="count">The number of sprites to add.</param>
        /// <param name="size">The optional size to assign to each sprite. If null, the default size is used.</param>
        public void AddAt(AeSprite sprite, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(sprite.Location + AeRandom.Between(-20, 20), color, size);
            }
        }

        /// <summary>
        /// Creates and inserts a new sprite particle at the location of the specified sprite with the given color and
        /// optional size.
        /// </summary>
        /// <param name="sprite">The sprite whose location will be used for the new particle. Cannot be null.</param>
        /// <param name="color">The color to apply to the new particle.</param>
        /// <param name="size">The size of the new particle. If null, a default size of 1x1 is used.</param>
        /// <returns>A new instance of AeSpriteParticle representing the inserted particle.</returns>
        public AeSpriteParticle AddAt(AeSprite sprite, Color4 color, Size? size = null)
        {
            var obj = new AeSpriteParticle(Engine, sprite.Location, size ?? new Size(1, 1), color);
            SpriteManager.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Creates and adds a new sprite particle at the specified location with the given color and optional size.
        /// </summary>
        /// <param name="location">The position where the sprite particle will be placed.</param>
        /// <param name="color">The color to apply to the sprite particle.</param>
        /// <param name="size">The size of the sprite particle. If null, a default size of 1x1 is used.</param>
        /// <returns>The newly created sprite particle instance.</returns>
        public AeSpriteParticle AddAt(AeVector location, Color4 color, Size? size = null)
        {
            var obj = new AeSpriteParticle(Engine, location, size ?? new Size(1, 1), color)
            {
                IsVisible = true
            };
            SpriteManager.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Creates and adds a new sprite particle at the specified location with an optional size.
        /// </summary>
        /// <remarks>The created sprite particle is immediately visible and managed by the sprite
        /// manager.</remarks>
        /// <param name="location">The position where the sprite particle will be placed.</param>
        /// <param name="size">The size of the sprite particle. If null, a default size of 1x1 is used.</param>
        /// <returns>The newly created sprite particle instance positioned at the specified location.</returns>
        public AeSpriteParticle AddAt(AeVector location, Size? size = null)
        {
            var obj = new AeSpriteParticle(Engine, location, size ?? new Size(1, 1))
            {
                IsVisible = true
            };
            SpriteManager.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Triggers a particle blast effect at the specified sprite's location with a maximum number of particles.
        /// </summary>
        /// <param name="at">The sprite whose location will be used as the origin for the particle blast.</param>
        /// <param name="maxParticleCount">The maximum number of particles to generate for the blast effect.</param>
        public void ParticleBlastAt(AeSprite at, int maxParticleCount)
        {
            Engine.Events.Add(() => ParticleBlastAt(at.Location, maxParticleCount));
        }

        /// <summary>
        /// Creates a random number of blasts consisting of "hot" colored particles at a given location.
        /// </summary>
        public void ParticleBlastAt(AeVector location, int maxParticleCount)
        {
            for (int i = 0; i < AeRandom.Between(maxParticleCount / 2, maxParticleCount); i++)
            {
                var particle = AddAt(location, new Size(AeRandom.Between(1, 2), AeRandom.Between(1, 2)));
                particle.Shape = AeParticleShape.FilledEllipse;
                particle.Pattern = AeParticleColorType.Solid;
                //particle.GradientStartColor = SiRenderingUtility.GetRandomHotColor();
                //particle.GradientEndColor = SiRenderingUtility.GetRandomHotColor();
                particle.Color = AeRenderingUtility.GetRandomHotColor();
                particle.CleanupMode = AeParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = AeRandom.Between(0.001f, 0.01f);
                particle.Speed *= AeRandom.Between(1, 3.5f);
                particle.VectorType = AeParticleVectorType.Default;
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
                p.VectorType = AeParticleVectorType.FollowOrientation;
                p.Orientation.Degrees = AeMath.WrapDegreesUnsigned(angle);
                p.Speed = AeRandom.Between(minSpeed, maxSpeed);
                p.Shape = AeParticleShape.FilledEllipse;
                p.Pattern = AeParticleColorType.Solid;
                p.CleanupMode = AeParticleCleanupMode.FadeToBlack;
                p.FadeToBlackReductionAmount = AeRandom.Between(0.01f, 0.02f);
                p.RotationSpeed = AeRandom.Between(-250f, 250f);
            }
        }

        /// <summary>
        /// Creates a cloud of particles at the location of the specified sprite.
        /// </summary>
        /// <param name="particleCount">The number of particles to generate in the cloud. Must be a non-negative integer.</param>
        /// <param name="at">The sprite whose location will be used as the origin for the particle cloud. Cannot be null.</param>
        public void ParticleCloud(int particleCount, AeSprite at)
            => ParticleCloud(particleCount, at.Location);

        /// <summary>
        /// Creates a cloud of particles at the specified location with randomized appearance and movement properties.
        /// </summary>
        /// <remarks>Each particle is assigned a random shape, color, orientation, speed, and rotation.
        /// Particles fade to black over time. This method is useful for visual effects requiring a burst or cluster of
        /// particles.</remarks>
        /// <param name="particleCount">The number of particles to generate in the cloud. Must be non-negative.</param>
        /// <param name="location">The location at which the particle cloud is created.</param>
        public void ParticleCloud(int particleCount, AeVector location)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var particle = AddAt(location, AeRenderingUtility.GetRandomHotColor(), new Size(5, 5));

                switch (AeRandom.Between(1, 3))
                {
                    case 1:
                        particle.Shape = AeParticleShape.Triangle;
                        break;
                    case 2:
                        particle.Shape = AeParticleShape.FilledEllipse;
                        break;
                    case 3:
                        particle.Shape = AeParticleShape.HollowEllipse;
                        break;
                }

                particle.CleanupMode = AeParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = 0.001f;
                particle.RotationSpeed = AeRandom.Between(-25f, 25f);
                particle.VectorType = AeParticleVectorType.FollowOrientation;
                particle.Orientation.Degrees = AeRandom.Between(0.0f, 359.0f);
                particle.Speed = AeRandom.Between(20, 350f);
            }
        }
    }
}
