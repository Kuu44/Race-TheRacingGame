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
    void Start()
    {
        if(SceneObjects.current.cars.Count > 0){
            for(int i = 0; i < SceneObjects.current.cars.Count; i++){
                carPhysics = SceneObjects.current.cars[i].transform.GetComponent<CarPhysics>();
                if(carPhysics.mainCar){
                    SceneObjects.current.ActiveCar = SceneObjects.current.cars[i];
                    break;
                }

            }
            SceneObjects.current.ActiveCar.transform.SetPositionAndRotation(SceneObjects.current.gridPositions[startPositionOnGrid-1].position, SceneObjects.current.gridPositions[startPositionOnGrid].rotation);

        }else{
            GameObject car = Instantiate(SceneObjects.current.defaultCar, SceneObjects.current.gridPositions[startPositionOnGrid-1].position, SceneObjects.current.gridPositions[startPositionOnGrid].rotation);
            SceneObjects.current.cars.Add(car);
            SceneObjects.current.ActiveCar = car;
            carPhysics = car.GetComponent<CarPhysics>();
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        carPhysics.AccelerateForward(inputY * Time.fixedDeltaTime);
        carPhysics.AddUpwardsTorque(inputX * Time.fixedDeltaTime);

        if(Input.GetKeyDown("space")){
            carPhysics.hardBrake();
        }

        if(Input.GetKeyUp("space")){
            carPhysics.releaseHardBrake();
        }
    }
}

