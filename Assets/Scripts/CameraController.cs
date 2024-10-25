using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Target to follow
    public float fixedY = 0.7f; // Fixed height of the camera

    void Update()
    {
        if (target != null)
        {
            // Set the camera's position to follow the target's X and Z, but keep Y fixed
            transform.position = new Vector3(target.position.x, fixedY, target.position.z);
        }
    }
}
