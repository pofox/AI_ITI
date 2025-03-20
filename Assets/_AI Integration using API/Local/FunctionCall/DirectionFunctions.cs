using UnityEngine;
// This class contains static methods that return different directions as Vector3
public class DirectionFunctions
{
    // Each method represents a movement direction using Unity's built-in Vector3 values
    public static Vector3 MoveUp()
    {
        return Vector3.up;    // Returns (0, 1, 0)
    }

    public static Vector3 MoveDown()
    {
        return Vector3.down;  // Returns (0, -1, 0)
    }

    public static Vector3 MoveLeft()
    {
        return Vector3.left;  // Returns (-1, 0, 0)
    }

    public static Vector3 MoveRight()
    {
        return Vector3.right; // Returns (1, 0, 0)
    }

    public static Vector3 NoDirectionsMentioned()
    {
        return Vector3.zero;  // Returns (0, 0, 0) when no direction is specified
    }
}
