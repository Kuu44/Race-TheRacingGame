using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public CarPhysics carPhysics;


    // Update is called once per frame
    void FixedUpdate()
    {
        if(carPhysics){
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            carPhysics.AccelerateForward(inputY * Time.fixedDeltaTime);
            carPhysics.AddUpwardsTorque(inputX * Time.fixedDeltaTime);

            if(Input.GetKey("space")){
                carPhysics.hardBrake();
            }
        }
    }
}

