//using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : ControllerBase<CarController>
{
    // Update is called once per frame
    float currentLapTime = 0;
    int numQualifyingLaps = 0;
    bool preQualifyingLap = false;
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

    public void onPassFlag(){
        //print("The main car just passed the chequered Flag!");
        if(numErrors() <= 2){
            //print("Lap completed successfully!");
            UIController.current.addLapTime(new lapTime(currentLapTime));
            UIController.current.showMessage("Your last lap time was " + new lapTime(currentLapTime).lapTimeAsString(), 3);
            if(RaceManager.current.gameStatus == RaceManager.GameStatus.Qualify && !SceneObjects.current.ActiveDriver.qualified){
                //RaceManager.current.addQualifyLapTime(currentLapTime, SceneObjects.current.ActiveDriver.driverName);
                if(preQualifyingLap){
                    //FOR TESTING
     
                    numQualifyingLaps+=1;
                    UIController.current.StatusText.text = "Qualifying! (Lap "+ numQualifyingLaps.ToString()+")";
                    if(numQualifyingLaps > RaceManager.current.numberOfQualifyingLaps){
                        SceneObjects.current.ActiveDriver.qualified = true;
                        UIController.current.StatusText.text = "Waiting";
                    }
                    RaceManager.current.addQualifyLapTime(currentLapTime, Random.Range(1,4).ToString() + " Driver");
                }else{
                    preQualifyingLap = true;
                    UIController.current.StatusText.text = "Qualifying! (Lap 1)";
                    UIController.current.showMessage("Good luck! The next lap will be counted", 5);
                }
            
            }
            
            
            currentLapTime = 0;
        }else{
            //print("Invalid lab, you probably cut corners or something");
            if(wayPointsPassed.Count > 0){
                UIController.current.showMessage("Invalid lap, you might have missed a corner or something", 5);
                if(RaceManager.current.gameStatus == RaceManager.GameStatus.Qualify){;
                    if(preQualifyingLap){
                    }else{
                        preQualifyingLap = true;
                        UIController.current.showMessage("Good luck! The next lap will be counted", 5);
                    }
                
                }
            }
            
            currentLapTime = 0;
        }
        wayPointsPassed.Clear();
    }


    int numErrors(){
        int result = SceneObjects.current.trackWayPoints.Count - wayPointsPassed.Count;
        if(result < 0) result = 0;

        for(int i = 0; i < Mathf.Min(wayPointsPassed.Count, SceneObjects.current.trackWayPoints.Count); i++){
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
                //RaceManager.current.startQualify();
                UIController.current.startQualifyCountDown();
            }
        }
    }
}

