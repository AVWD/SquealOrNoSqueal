using UnityEngine;
using System.Collections;

public class SmoothCamFollow : MonoBehaviour
{
    public float DampTime = 0.15f;

    public Vector3 CameraSpeedModifier = Vector2.one;
    public Transform Target;
    private Vector3 CameraVelocity = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (Target)
        {
            Vector3 point = Camera.main.WorldToViewportPoint(Target.position);

            Vector3 delta = Target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            delta.x *= CameraSpeedModifier.x;
            delta.y *= CameraSpeedModifier.y;
            delta.z *= CameraSpeedModifier.z;

            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref CameraVelocity, DampTime);
        }

    }
}
