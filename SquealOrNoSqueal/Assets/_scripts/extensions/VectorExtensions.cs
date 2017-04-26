using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
    public static Vector3 PerpendicularClockwise(this Vector3 vector)
    {
        return new Vector3(-vector.y, vector.x, vector.z);
    }

    public static Vector3 PerpendicularCounterClockwise(this Vector3 vector)
    {
        return new Vector3(vector.y, -vector.x, vector.z);
    }

    public static Vector2 PerpendicularClockwise(this Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }
}
