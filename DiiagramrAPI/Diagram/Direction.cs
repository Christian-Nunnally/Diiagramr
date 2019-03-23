using System;

namespace DiiagramrAPI.Diagram
{
    [Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
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
            if (direction == Direction.West)
            {
                return Direction.East;
            }
            return Direction.None;
        }
    }
}
