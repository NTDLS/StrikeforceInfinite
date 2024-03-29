﻿using Si.Engine.Sprite._Superclass;
using Si.GameEngine.Sprite.SupportingClasses;
using System.Collections.Generic;
using System.Drawing;

namespace Si.Engine.Manager
{
    public class CollisionManager
    {
        private readonly EngineCore _engine;
        public Dictionary<string, Collision> Detected { get; private set; } = new();

        public PredictedSpriteRegion[] Colliadbles { get; private set; }

        public struct Collision
        {
            public string Key { get; private set; }
            public PredictedSpriteRegion Predicted1 { get; private set; }
            public PredictedSpriteRegion Predicted2 { get; private set; }

            public RectangleF OverlapRectangle { get; set; }
            public PointF[] OverlapPolygon { get; set; }

            public Collision(string key, PredictedSpriteRegion sprite1, PredictedSpriteRegion sprite2)
            {
                Key = key;
                Predicted1 = sprite1;
                Predicted2 = sprite2;
            }
        }

        public CollisionManager(EngineCore engine)
        {
            _engine = engine;
        }

        public static string MakeCollisionKey(uint uid1, uint uid2)
            => uid1 > uid2 ? $"{uid1}:{uid2}" : $"{uid2}:{uid1}";

        public Collision Add(PredictedSpriteRegion predicted1, PredictedSpriteRegion predicted2)
        {
            var key = MakeCollisionKey(predicted1.Sprite.UID, predicted1.Sprite.UID);

            var collision = new Collision(key, predicted1, predicted2)
            {
                OverlapRectangle = predicted1.GetIntersectionBoundingBox(predicted2),
                OverlapPolygon = predicted1.GetIntersectedPolygon(predicted2)
            };

            Detected.Add(key, collision);
            return collision;
        }

        public void Reset(float epoch)
        {
            Colliadbles = _engine.Sprites.VisibleColliadblePredictiveMove(epoch);
            Detected.Clear();
        }

        public bool IsAlreadyHandled(SpriteInteractiveBase sprite1, SpriteInteractiveBase sprite2)
            => Detected.ContainsKey(MakeCollisionKey(sprite1.UID, sprite2.UID));

        public bool IsAlreadyHandled(PredictedSpriteRegion predicted1, PredictedSpriteRegion predicted2)
            => Detected.ContainsKey(MakeCollisionKey(predicted1.Sprite.UID, predicted2.Sprite.UID));
    }
}
