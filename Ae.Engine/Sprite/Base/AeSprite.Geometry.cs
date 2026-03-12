using Ae.Engine.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a sprite in the scene, providing methods for spatial queries such as proximity and directional
    /// relationships with other sprites.
    /// </summary>
    /// <remarks>The AeSprite class enables detection of spatial relationships, including determining which
    /// sprites are pointing at this sprite and identifying the closest sprite among a group. Methods support
    /// tolerance-based direction checks and distance calculations, facilitating common game or simulation logic
    /// involving sprite interactions.</remarks>
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

        /// <summary>
        /// Calculates the distance from this sprite to the specified sprite.
        /// </summary>
        /// <param name="to">The sprite to which the distance is calculated. Cannot be null.</param>
        /// <returns>The distance between this sprite and the specified sprite, measured in units defined by the sprite's
        /// location.</returns>
        public float DistanceTo(AeSprite to) => Location.DistanceTo(to.Location);

        /// <summary>
        /// Calculates the squared distance from this sprite to the specified sprite.
        /// </summary>
        /// <remarks>Using the squared distance avoids the computational cost of calculating the square
        /// root, which can be beneficial for performance when only relative distances are needed.</remarks>
        /// <param name="to">The target sprite to which the squared distance is calculated. Cannot be null.</param>
        /// <returns>A single-precision floating-point value representing the squared distance between this sprite and the
        /// specified sprite.</returns>
        public float DistanceSquaredTo(AeSprite to) => Location.DistanceSquaredTo(to.Location);

        /// <summary>
        /// Calculates the Euclidean distance from the current vector to the specified vector.
        /// </summary>
        /// <param name="to">The target vector to which the distance is calculated.</param>
        /// <returns>The distance between the current vector and the specified vector, measured as a floating-point value.</returns>
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
