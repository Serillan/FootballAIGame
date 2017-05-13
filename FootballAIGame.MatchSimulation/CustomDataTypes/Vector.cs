using System;

namespace FootballAIGame.MatchSimulation.CustomDataTypes
{
    /// <summary>
    /// Represents the two-dimensional vectors or points.
    /// </summary>
    public class Vector
    {
        /// <summary>
        /// Gets or sets the x-coordinate value.
        /// </summary>
        /// <value>
        /// The x-coordinate.
        /// </value>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate value.
        /// </summary>
        /// <value>
        /// The y-coordinate.
        /// </value>
        public double Y { get; set; }

        /// <summary>
        /// Gets the vector's length.
        /// </summary>
        /// <value>
        /// The vector's length.
        /// </value>
        public double Length =>
            Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// </summary>
        public Vector() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the distances between the specified vectors.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <returns>The distance between the specified vectors.</returns>
        public static double DistanceBetween(Vector firstVector, Vector secondVector)
        {
            return Math.Sqrt(Math.Pow(firstVector.X - secondVector.X, 2) + Math.Pow(firstVector.Y - secondVector.Y, 2));
        }

        /// <summary>
        /// Gets the dot product of the specified vectors.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <returns>The dot product of the specified vectors.</returns>
        public static double DotProduct(Vector firstVector, Vector secondVector)
        {
            return firstVector.X*secondVector.X + firstVector.Y*secondVector.Y;
        }
    }
}
