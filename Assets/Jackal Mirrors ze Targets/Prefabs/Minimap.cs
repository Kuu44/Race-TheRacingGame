using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    float cameraDistanceAbove = 100f;
    [Range(0, 1)]
    float followSpeed = .2f;

    Camera minimapCamera;

    void Start()
    {
        minimapCamera = GameObject.FindGameObjectWithTag("Minimap Camera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        Vector3 movetarget = transform.position + Vector3.up * cameraDistanceAbove;

        minimapCamera.transform.position = Vector3.Lerp(minimapCamera.transform.position, movetarget, followSpeed);
    }
}
