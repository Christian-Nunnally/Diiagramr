using DiiagramrModel;

namespace DiiagramrAPI.Editor
{
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;

                case Direction.South:
                    return Direction.North;

                case Direction.East:
                    return Direction.West;

                case Direction.West:
                    return Direction.East;

                default:
                    return Direction.None;
            }
        }
    }
}