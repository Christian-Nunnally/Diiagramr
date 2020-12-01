using DiiagramrModel;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Helpful extension methods for <see cref="Direction"/>.
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Gets the <see cref="Direction"/> opposite of this <see cref="Direction"/>.
        /// </summary>
        /// <param name="direction">The original direction.</param>
        /// <returns>the opposite <see cref="Direction"/> </returns>
        public static Direction Opposite(this Direction direction) => direction switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => Direction.None,
        };
    }
}