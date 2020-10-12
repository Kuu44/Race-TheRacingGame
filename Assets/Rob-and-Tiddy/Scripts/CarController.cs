using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Range(1,5)]
    public int startPositionOnGrid = 1;
    CarPhysics carPhysics;
    void Awake()
    {
        SceneObjects.current.ActiveCar.transform.SetPositionAndRotation(SceneObjects.current.gridPositions[startPositionOnGrid-1].position, SceneObjects.current.gridPositions[startPositionOnGrid].rotation);
        carPhysics = SceneObjects.current.ActiveCar.GetComponent<CarPhysics>();   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        carPhysics.AccelerateForward(inputY * Time.fixedDeltaTime);
        carPhysics.AddUpwardsTorque(inputX * Time.fixedDeltaTime);

        if(Input.GetKey("space")){
            carPhysics.hardBrake();
        }
    }
}

