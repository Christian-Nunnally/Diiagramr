using System;

namespace DiiagramrAPI.Diagram
{
    [Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public static class DirectionHelpers
    {
        public static Direction OppositeDirection(Direction direction)
        {
            if (direction == Direction.North)
            {
                return Direction.South;
            }

            if (direction == Direction.South)
            {
                return Direction.North;
            }

            if (direction == Direction.East)
            {
                return Direction.West;
            }

            return Direction.East;
        }
    }
}
