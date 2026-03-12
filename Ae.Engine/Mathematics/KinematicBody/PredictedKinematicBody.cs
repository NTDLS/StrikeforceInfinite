using Ae.Engine.Sprite.Interactive;
using SharpDX.Mathematics.Interop;
using System.Drawing;
using System.Linq;

namespace Ae.Engine.Mathematics.KinematicBody
{
    /// <summary>
    /// Contains the prediction of sprite location, bounds, velocity and direction after the up comming call to ApplyMotion()
    /// Keep in mind that this is somewhat rudimentary in the way that it predicts the next location but decisively so.
    /// 
    /// An object that contains both the location (position) and the velocity (vector indicating both
    /// speed and direction) of another object is commonly referred to as a "Kinematic" object.
    /// </summary>
    public class PredictedKinematicBody
    {
        /// <summary>
        /// Reference to the sprite.
        /// </summary>
        public AeSpriteInteractive Sprite { get; set; }

        /// <summary>
        /// The location of the render window when the prediction was made.
        /// </summary>
        public AeVector RenderWindowPosition { get; private set; }

        /// <summary>
        /// Size of the referenced sprite.
        /// </summary>
        public Size Size => Sprite.Size;

        /// <summary>
        /// Predicted location after next call to ApplyMotion().
        /// </summary>
        public AeVector PredictedLocation { get; private set; }

        /// <summary>
        /// Predicted direction after next call to ApplyMotion().
        /// </summary>
        public AeVector PredictedDirection { get; private set; }

        /// <summary>
        /// Predicted bounds after next call to ApplyMotion().
        /// </summary>
        public RectangleF Bounds => new(
            (PredictedLocation.X - Size.Width / 2.0f) - (Sprite.Metadata.CollisionPolyAugmentation ?? 0 / 2.0f),
            (PredictedLocation.Y - Size.Height / 2.0f) - (Sprite.Metadata.CollisionPolyAugmentation ?? 0 / 2.0f),
            Size.Width + (Sprite.Metadata.CollisionPolyAugmentation ?? 0 / 2.0f),
            Size.Height + (Sprite.Metadata.CollisionPolyAugmentation ?? 0 / 2.0f));

        /// <summary>
        /// Predicted render bounds after next call to ApplyMotion().
        /// </summary>
        public virtual RawRectangleF RawRenderBounds => new(
                        (RenderLocation.X - Size.Width / 2.0f),
                        (RenderLocation.Y - Size.Height / 2.0f),
                        (RenderLocation.X - Size.Width / 2.0f) + Size.Width,
                        (RenderLocation.Y - Size.Height / 2.0f) + Size.Height);

        /// <summary>
        /// Predicted render location after next call to ApplyMotion().
        /// </summary>
        public AeVector RenderLocation => PredictedLocation - RenderWindowPosition;

        /// <summary>
        /// Initializes a new instance of the PredictedKinematicBody class using the specified sprite, render window
        /// position, and prediction epoch. Calculates the predicted direction and location based on the sprite's
        /// current state and the given epoch.
        /// </summary>
        /// <remarks>Use this constructor to create a predicted kinematic body for visualizing or
        /// simulating future movement based on the sprite's current orientation, rotation speed, and movement vector.
        /// The predicted direction and location are calculated assuming linear motion over the specified
        /// epoch.</remarks>
        /// <param name="sprite">The interactive sprite whose kinematic state is used as the basis for prediction. Cannot be null.</param>
        /// <param name="renderWindowPosition">The position within the render window where the predicted body will be displayed.</param>
        /// <param name="epoch">The time interval, in seconds, used for prediction calculations. Must be non-negative.</param>
        public PredictedKinematicBody(AeSpriteInteractive sprite, AeVector renderWindowPosition, float epoch)
        {
            RenderWindowPosition = renderWindowPosition;

            Sprite = sprite;

            PredictedDirection = new AeVector(sprite.Orientation.RadiansSigned + sprite.RotationSpeed * epoch);
            PredictedLocation = sprite.Location + (sprite.MovementVector * epoch);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsAABB(PredictedKinematicBody otherObject) =>
            Bounds.IntersectsWith(otherObject.Bounds);

        /// <summary>
        /// Determines if two (non-axis-aligned) rectangles interest using Separating Axis Theorem (SAT).
        /// This allows us to determine if a rotated rectangle intersects another rotated rectangle.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsSAT(PredictedKinematicBody otherObject)
            => AeSeparatingAxisTheorem.IntersectsRotated(Bounds, PredictedDirection.RadiansSigned,
                otherObject.Bounds, otherObject.PredictedDirection.RadiansSigned);

        /// <summary>
        /// Calculates the overlapping rectangle between this object and another using the Separating Axis Theorem
        /// (SAT).
        /// </summary>
        /// <remarks>This method uses the predicted bounds and direction of both objects to determine the
        /// overlap. The result is based on the current predicted positions and orientations.</remarks>
        /// <param name="otherObject">The predicted kinematic body to check for overlap with this object. Must not be null.</param>
        /// <returns>A RectangleF representing the area of overlap between the two objects. If there is no overlap, the rectangle
        /// will have zero width and height.</returns>
        public RectangleF GetOverlapRectangleSAT(PredictedKinematicBody otherObject)
            => AeSeparatingAxisTheorem.GetOverlapRectangle(Bounds, PredictedDirection.RadiansSigned,
                otherObject.Bounds, otherObject.PredictedDirection.RadiansSigned);

        /// <summary>
        /// Calculates the bounding box of the intersection area between this kinematic body and another predicted
        /// kinematic body.
        /// </summary>
        /// <remarks>The intersection is computed based on the predicted positions and orientations of
        /// both kinematic bodies. This method is useful for collision detection and spatial analysis
        /// scenarios.</remarks>
        /// <param name="otherObject">The other predicted kinematic body to intersect with. Cannot be null.</param>
        /// <returns>A RectangleF representing the bounding box of the intersection area. If there is no intersection, the
        /// bounding box will be empty.</returns>
        public RectangleF GetIntersectionBoundingBox(PredictedKinematicBody otherObject)
            => AeSutherlandHodgmanPolygonIntersection.GetIntersectionBoundingBox(Bounds, PredictedDirection.RadiansSigned,
                otherObject.Bounds, otherObject.PredictedDirection.RadiansSigned);

        /// <summary>
        /// Calculates the polygon representing the intersection area between this kinematic body and another predicted
        /// kinematic body.
        /// </summary>
        /// <remarks>The intersection is computed based on the predicted bounds and movement directions of
        /// both bodies. This method is useful for collision detection and spatial analysis in kinematic
        /// simulations.</remarks>
        /// <param name="otherObject">The predicted kinematic body to intersect with. Must not be null.</param>
        /// <returns>An array of points defining the intersected polygon in 2D space. The array will be empty if there is no
        /// intersection.</returns>
        public PointF[] GetIntersectedPolygon(PredictedKinematicBody otherObject)
            => AeSutherlandHodgmanPolygonIntersection.GetIntersectedPolygon(Bounds, PredictedDirection.RadiansSigned,
            otherObject.Bounds, otherObject.PredictedDirection.RadiansSigned);

        /// <summary>
        /// Calculates the coordinates of the four corners of the rectangle after applying the predicted rotation.
        /// </summary>
        /// <remarks>The rotation is determined by the predicted direction in radians. Use this method to
        /// obtain the exact positions of the rectangle's corners for collision detection or rendering after
        /// rotation.</remarks>
        /// <returns>An array of four <see cref="PointF"/> values representing the corners of the rotated rectangle, ordered
        /// clockwise starting from the top-left corner.</returns>
        public PointF[] GetRotatedRectangleCorners()
            => AeSeparatingAxisTheorem.GetRotatedRectangleCorners(Bounds, PredictedDirection.RadiansSigned).ToArray();
    }
}
