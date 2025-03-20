using UnityEngine;
// This class contains static methods that return different colors
public class ColorFunctions
{
    // Each method returns a Unity Color value
    public static Color BlueColor()
    {
        return Color.blue;
    }

    public static Color RedColor()
    {
        return Color.red;
    }

    public static Color NoColorMentioned()
    {
        return Color.white;  // Returns white when no color is specified
    }
}
