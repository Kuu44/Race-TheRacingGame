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
        }else{
            GameObject car = Instantiate(SceneObjects.current.defaultCar, SceneObjects.current.gridPositions[startPositionOnGrid].position, SceneObjects.current.gridPositions[startPositionOnGrid].rotation);
            SceneObjects.current.cars.Add(car);
            SceneObjects.current.ActiveCar = car;
            carPhysics = car.GetComponent<CarPhysics>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        carPhysics.AccelerateForward(inputY);
        carPhysics.AddUpwardsTorque(inputX);
    }
}

