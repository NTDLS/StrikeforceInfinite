using Ae.Engine.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.Sprite.Base
{
    public partial class AeSprite
    {
        /// <summary>
        /// Returns true if any of the given sprites are pointing at this one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="atObjects"></param>
        /// <param name="toleranceDegrees"></param>
        /// <returns></returns>
        public bool IsPointingAtAny<T>(List<T> atObjects, float toleranceDegrees) where T : AeSprite
        {
            foreach (var atObj in atObjects)
            {
                if (this.IsPointingAt(atObj, toleranceDegrees))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// From the list of given sprites, returns the list of sprites that are pointing at us.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="atObjects"></param>
        /// <param name="toleranceDegrees"></param>
        /// <returns></returns>
        public List<T> GetPointingAtOf<T>(List<T> atObjects, float toleranceDegrees) where T : AeSprite
        {
            var results = new List<T>();

            foreach (var atObj in atObjects)
            {
                if (this.IsPointingAt(atObj, toleranceDegrees))
                {
                    results.Add(atObj);
                }
            }
            return results;
        }

        public float DistanceTo(AeSprite to) => Location.DistanceTo(to.Location);

        public float DistanceSquaredTo(AeSprite to) => Location.DistanceSquaredTo(to.Location);

        public float DistanceTo(AeVector to) => Location.DistanceTo(to);

        /// <summary>
        /// Of the given sprites, returns the sprite that is the closest.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tos"></param>
        /// <returns></returns>
        public T ClosestOf<T>(List<T> tos) where T : AeSprite
        {
            float closestDistance = float.MaxValue;
            T closestSprite = tos.First();

            foreach (var to in tos)
            {
                var distance = Location.DistanceTo(to.Location);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSprite = to;
                }
                ;
            }

            return closestSprite;
        }

        /// <summary>
        /// Of the given sprites, returns the distance of the closest one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tos"></param>
        /// <returns></returns>
        public float ClosestDistanceOf<T>(List<T> tos) where T : AeSprite
        {
            float closestDistance = float.MaxValue;

            foreach (var to in tos)
            {
                var distance = Location.DistanceTo(to.Location);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
                ;
            }

            return closestDistance;
        }
    }
}
