using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRotate : MonoBehaviour
{
    [Tooltip("Set to any Transform, or null to look forward relative to the camera.")]
    public Transform BodyFollow;

    public Transform lookAt;
    public float rotationSpeed = 0.25f;
    public Vector4 limitLeftRightUpDown = new Vector4(-80, 80, -45, 45);
    private float rxVel = 0;
    private float ryVel = 0;

    void Update()
    {
        if (BodyFollow != null)
            transform.position = Vector3.Lerp(transform.position, BodyFollow.position, 2 * Time.maximumDeltaTime);

        Vector3 forward = (lookAt == null) ? Camera.main.transform.forward : (lookAt.position - transform.position).normalized;
        Vector3 prevRotation = transform.localRotation.eulerAngles;

        transform.forward = forward;
        Vector3 newRotation = transform.localRotation.eulerAngles;

        if (newRotation.x > 180) newRotation.x -= 360;
        if (newRotation.y > 180) newRotation.y -= 360;
        newRotation.x = Mathf.Clamp(newRotation.x, limitLeftRightUpDown.z, limitLeftRightUpDown.w);
        newRotation.y = Mathf.Clamp(newRotation.y, limitLeftRightUpDown.x, limitLeftRightUpDown.y);
        transform.localRotation = Quaternion.Euler(new Vector3(Mathf.SmoothDampAngle(prevRotation.x, newRotation.x, ref rxVel, rotationSpeed), Mathf.SmoothDampAngle(prevRotation.y, newRotation.y, ref ryVel, rotationSpeed)));
    }
}
