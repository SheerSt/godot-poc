using Godot;
using System;

/**
 * 
 */
public enum Direction
{
    UP = 0,
    RIGHT = 2,
    DOWN = 4,
    LEFT = 6,
    NE = 1,
    SE = 3,
    SW = 5,
    NW = 7,
    NONE = -1,
}

/**
 * 
 */
namespace ExtensionMethods
{
    public static class DirectionExtensions
    {

        public static Direction[] all = {
            Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT, 
            Direction.NE, Direction.SE, Direction.SW, Direction.NW
        };

        public static Direction opposite(this Direction direction)
        {

            return (Direction)((int)direction + 4 % 8);

        }

        /**
         * Returns Vector2 that represents direction.
         */
        public static Vector2 getVector(this Direction direction)
        {

            if (direction == Direction.UP) return new Vector2(0, 1);
            else if (direction == Direction.DOWN) return new Vector2(0, -1);
            else if (direction == Direction.LEFT) return new Vector2(-1, 0);
            else if (direction == Direction.RIGHT) return new Vector2(1, 0);
            else if (direction == Direction.NE) return new Vector2(1, 1);
            else if (direction == Direction.SE) return new Vector2(1, -1);
            else if (direction == Direction.SW) return new Vector2(-1, -1);
            else if (direction == Direction.NW) return new Vector2(-1, 1);

            return Vector2.Zero;

        }

    }

}

