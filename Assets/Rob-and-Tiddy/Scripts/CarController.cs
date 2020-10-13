using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : ControllerBase<CarController>
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

    public void onPassFlag(){
        //print("The main car just passed the chequered Flag!");
        if(numErrors() <= 2){
            //print("Lap completed successfully!");
            UIController.current.addLapTime(new lapTime(currentLapTime));
            currentLapTime = 0;
        }else{
            print("Invalid lab, you probably cut corners or something");
            currentLapTime = 0;
        }
        wayPointsPassed.Clear();
    }


    int numErrors(){
        int result = SceneObjects.current.trackWayPoints.Count - wayPointsPassed.Count;
        if(result < 0) result = 0;

        for(int i = 0; i < wayPointsPassed.Count; i++){
            if(wayPointsPassed[i] != SceneObjects.current.trackWayPoints[i]){
                result++;
            }
        }

        return result;
    }


    public List<Transform> wayPointsPassed = new List<Transform>();

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
            if(Input.GetKeyUp("m")){
                //print("N KEY PRESSED");
                RaceManager.current.startQualify();
            }
        }
    }
}

