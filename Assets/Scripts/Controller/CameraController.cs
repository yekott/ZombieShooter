using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    private Vector3 velocity;

    private void LateUpdate()
    {
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothSpeed);
        transform.position = smoothPosition;
    }
}
