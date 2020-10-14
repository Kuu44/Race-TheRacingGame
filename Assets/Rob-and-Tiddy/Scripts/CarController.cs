//using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : ControllerBase<CarController>
{
    // Update is called once per frame
    void FixedUpdate()
    {
        if(SceneObjects.current.ActiveDriver && RaceManager.current.allowCarControl){
            if(SceneObjects.current.ActiveDriver.carPhysics){
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");

                SceneObjects.current.ActiveDriver.carPhysics.AccelerateForward(inputY * Time.fixedDeltaTime);
                SceneObjects.current.ActiveDriver.carPhysics.AddUpwardsTorque(inputX * Time.fixedDeltaTime);

                if(Input.GetKey("space")){
                    SceneObjects.current.ActiveDriver.carPhysics.hardBrake();
                }
            }

        
        }else{
            //print("ACTIVE DRIVER ABSENT!");
        }
    }


    void Update(){
         if(SceneObjects.current.ActiveDriver){
            
            
            if(Input.GetKeyUp("n")){
                //print("N KEY PRESSED");
                SceneObjects.current.ActiveDriver.tempCarIndex += 1;
                if(SceneObjects.current.ActiveDriver.tempCarIndex >= SceneObjects.current.carPrefabs.Count){
                    SceneObjects.current.ActiveDriver.tempCarIndex = 0;
                }
                SceneObjects.current.ActiveDriver.switchCar(SceneObjects.current.ActiveDriver.tempCarIndex);
            }
            if(Input.GetKeyUp("m")){
                //print("N KEY PRESSED");
                RaceManager.current.startQualify();
                //UIController.current.startQualifyCountDown();
            }
        }
    }
}

