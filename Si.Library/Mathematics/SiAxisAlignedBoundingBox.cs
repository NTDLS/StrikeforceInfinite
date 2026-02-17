namespace Si.Library.Mathematics
{
    public static class SiAxisAlignedBoundingBox
    {
        /// <summary>
        /// Calculates the axis-aligned bounding box (AABB) that fully contains the motion of an object between two
        /// positions, expanded by the specified half size.
        /// </summary>
        /// <remarks>This method is useful for collision detection and spatial queries in 2D environments,
        /// as it provides the minimal bounding box that encompasses the object's entire path, including its
        /// size.</remarks>
        /// <param name="startPos">The initial position of the object as a vector.</param>
        /// <param name="endPos">The final position of the object as a vector.</param>
        /// <param name="halfSize">A vector representing half the width and height of the object, used to expand the bounding box to account
        /// for the object's dimensions.</param>
        /// <returns>A tuple containing the minimum and maximum corners of the calculated AABB as vectors.</returns>
        public static (SiVector min, SiVector max) SweptAabbForMotion(SiVector startPos, SiVector endPos, SiVector halfSize)
        {
            var minX = MathF.Min(startPos.X, endPos.X) - halfSize.X;
            var minY = MathF.Min(startPos.Y, endPos.Y) - halfSize.Y;
            var maxX = MathF.Max(startPos.X, endPos.X) + halfSize.X;
            var maxY = MathF.Max(startPos.Y, endPos.Y) + halfSize.Y;

            return (new SiVector(minX, minY), new SiVector(maxX, maxY));
        }

        /// <summary>
        /// Determines whether two axis-aligned bounding boxes (AABBs) overlap.
        /// </summary>
        /// <remarks>Both bounding boxes must be properly defined, with the minimum point less than or
        /// equal to the maximum point on each axis. This method compares the coordinates of the bounding boxes to
        /// determine overlap in two dimensions.</remarks>
        /// <param name="a">A tuple representing the first axis-aligned bounding box, defined by its minimum and maximum corner points.</param>
        /// <param name="b">A tuple representing the second axis-aligned bounding box, defined by its minimum and maximum corner points.</param>
        /// <returns>true if the two bounding boxes overlap; otherwise, false.</returns>
        public static bool AabbOverlaps((SiVector min, SiVector max) a, (SiVector min, SiVector max) b)
        {
            return !(a.max.X < b.min.X || a.min.X > b.max.X || a.max.Y < b.min.Y || a.min.Y > b.max.Y);
        }
    }
}
