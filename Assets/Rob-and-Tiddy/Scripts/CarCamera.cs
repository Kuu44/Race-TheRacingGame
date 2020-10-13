using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{

    public float cameraDistanceBehind = 4.5f;
    public float cameraDistanceAbove = 2.5f;
    public float lookDistanceAboveCar = 0.5f;

    [Range(0,1)]
    public float lookSpeed = 0.1f;
    [Range(0,1)]
    public float followSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(SceneObjects.current.ActiveDriver != null){

                Vector3 carPos = SceneObjects.current.ActiveDriver.car.transform.position;
                Vector3 carBack = -SceneObjects.current.ActiveDriver.car.transform.forward;
                Vector3 carUp = SceneObjects.current.ActiveDriver.car.transform.up;
                Vector3 movetarget = carPos + carBack * cameraDistanceBehind + carUp * cameraDistanceAbove;
                Vector3 lookTarget = carPos + carUp * lookDistanceAboveCar;

                transform.LookAt(Vector3.Lerp(transform.position + transform.forward,lookTarget, lookSpeed), Vector3.Slerp(transform.up, carUp, lookSpeed));

                transform.position = Vector3.Lerp(transform.position, movetarget, followSpeed);

        }
    }
}
