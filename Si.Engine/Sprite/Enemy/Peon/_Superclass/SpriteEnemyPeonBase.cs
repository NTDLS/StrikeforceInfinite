﻿using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Library.Mathematics.Geometry;
using System;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy.Peon._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyPeonBase : SpriteEnemyBase
    {
        public SpriteAnimation ThrustAnimation { get; internal set; }
        public SpriteAnimation BoostAnimation { get; internal set; }

        public SpriteEnemyPeonBase(EngineCore engine)
            : base(engine)
        {
            Velocity.ForwardMomentium = 1;
            Initialize();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new SpriteAnimation.PlayMode()
            {
                Replay = SiAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrustAnimation = new SpriteAnimation(_engine, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode)
            {
                OwnerUID = UID
            };
            ThrustAnimation.Reset();
            _engine.Sprites.Animations.AddAt(ThrustAnimation, this);

            BoostAnimation = new SpriteAnimation(_engine, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode)
            {
                OwnerUID = UID
            };
            BoostAnimation.Reset();
            _engine.Sprites.Animations.AddAt(BoostAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void LocationChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointBehind = SiPoint.PointFromAngleAtDistance360(Velocity.Angle + SiPoint.DegreesToRadians(180), new SiPoint(20, 20));
                ThrustAnimation.Velocity.Angle = Velocity.Angle;
                ThrustAnimation.Location = Location + pointBehind;
            }
            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointBehind = SiPoint.PointFromAngleAtDistance360(Velocity.Angle + SiPoint.DegreesToRadians(180), new SiPoint(20, 20));
                BoostAnimation.Velocity.Angle = Velocity.Angle;
                BoostAnimation.Location = Location + pointBehind;
            }
        }

        private void EnemyBase_OnVisibilityChanged(SpriteBase sender)
        {
            if (ThrustAnimation != null)
            {
                ThrustAnimation.Visable = false;
            }
            if (BoostAnimation != null)
            {
                BoostAnimation.Visable = false;
            }
        }

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);

            if (ThrustAnimation != null)
            {
                ThrustAnimation.Visable = Velocity.ForwardMomentium > 0;
            }
            if (BoostAnimation != null)
            {
                BoostAnimation.Visable = Velocity.ForwardBoostMomentium > 0;
            }
        }

        public override void Cleanup()
        {
            ThrustAnimation?.QueueForDelete();
            BoostAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}