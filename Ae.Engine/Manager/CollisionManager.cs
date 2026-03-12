using Ae.Engine.Mathematics.KinematicBody;
using Ae.Engine.Sprite.Interactive;
using System.Collections.Generic;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// This class is used to keep track of which sprites are collidable and which collisions have been handled.
    /// I do no yet know how to really "handle" collisions, but I do know that if we detect a collision with
    ///     sprite "A" then we will also detect a separate collision with sprite "B" and I wanted a way to determine
    ///     if that "collision" interaction had already been addressed with the first objects detection.
    ///     
    /// Each time the game loop begins, CollisionManager.Reset() is called to (1) clear all the detected collisions,
    ///     (2) create a list of collidable sprites, and (3) calculate the predicted location and rotation of those sprites.
    /// </summary>
    public class CollisionManager
    {
        private readonly AeEngine _engine;
        internal Dictionary<string, OverlappingKinematicBodyPair> Detected { get; private set; } = new();

        internal PredictedKinematicBody[] Collidables { get; private set; } = new PredictedKinematicBody[0];

        /// <summary>
        /// Initializes a new instance of the CollisionManager class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance that provides context and services for collision management. Cannot be null.</param>
        public CollisionManager(AeEngine engine)
        {
            _engine = engine;
        }

        internal OverlappingKinematicBodyPair Create(PredictedKinematicBody body1, PredictedKinematicBody body2)
        {
            var key = OverlappingKinematicBodyPair.MakeKey(body1.Sprite.UID, body2.Sprite.UID);

            var collisionPair = new OverlappingKinematicBodyPair(key, body1, body2)
            {
                //We are just adding these here for demonstration purposes. This is probably over the top
                // and we DEFINITELY do not need GetIntersectionBoundingBox() AND GetIntersectedPolygon().
                //
                // Q: Also note that this is just the collision for predicted1→predicted2, which I am thinking might be different??
                // A: I tested it, they are definitely different.
                //https://github.com/NTDLS/StrikeforceInfinite/wiki/Collision-Detection-Issues
                OverlapRectangle = body1.GetIntersectionBoundingBox(body2),
                OverlapPolygon = body1.GetIntersectedPolygon(body2)
            };

            return collisionPair;
        }

        internal void Record(OverlappingKinematicBodyPair pair)
        {
            Detected.Add(pair.Key, pair);
        }

        internal OverlappingKinematicBodyPair CreateAndRecord(PredictedKinematicBody body1, PredictedKinematicBody body2)
        {
            var collisionPair = Create(body1, body2);
            Record(collisionPair);
            return collisionPair;
        }

        /// <summary>
        /// This is where we snapshot all of the collidable sprites before the tick.
        /// It is important to remeber that we will later need to verify the visibility of sprites that are colliding
        ///     since that state can change between this recording and the collision calculation.
        /// </summary>
        /// <param name="epoch"></param>
        internal void SnapshotCollidables(float epoch)
        {
            Collidables = _engine.Sprites.VisibleCollidablePredictiveMove(epoch);
            Detected.Clear();
        }

        internal bool IsAlreadyHandled(AeSpriteInteractive sprite1, AeSpriteInteractive sprite2)
            => Detected.ContainsKey(OverlappingKinematicBodyPair.MakeKey(sprite1.UID, sprite2.UID));

        internal bool IsAlreadyHandled(PredictedKinematicBody body1, PredictedKinematicBody body2)
            => Detected.ContainsKey(OverlappingKinematicBodyPair.MakeKey(body1.Sprite.UID, body2.Sprite.UID));
    }
}
