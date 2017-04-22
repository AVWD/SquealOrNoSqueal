using UnityEngine;
using System.Collections;

public static class TransformExtensions
{
    public static void SetX(this Transform transform, float x)
    {
        Vector3 newPos = new Vector3(x, transform.position.y, transform.position.z);
        transform.position = newPos;
    }
    public static void SetY(this Transform transform, float y)
    {
        Vector3 newPos = new Vector3(transform.position.x, y, transform.position.z);
        transform.position = newPos;
    }
}
