using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Update is called once per frame
    float currentLapTime = 0;
    void FixedUpdate()
    {
        if(SceneObjects.current.ActiveDriver){
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
            print("ACTIVE DRIVER ABSENT!");
        }
    }

    void Update(){
         if(SceneObjects.current.ActiveDriver){
           
            currentLapTime += Time.deltaTime;
            if(Time.frameCount % 10 == 0){
                UIController.current.setCurrentLapTime(currentLapTime);
            }
            if(Input.GetKeyUp("n")){
                //print("N KEY PRESSED");
                SceneObjects.current.ActiveDriver.tempCarIndex += 1;
                if(SceneObjects.current.ActiveDriver.tempCarIndex >= SceneObjects.current.carPrefabs.Count){
                    SceneObjects.current.ActiveDriver.tempCarIndex = 0;
                }
                SceneObjects.current.ActiveDriver.switchCar(SceneObjects.current.ActiveDriver.tempCarIndex);
            }
        }
    }
}

