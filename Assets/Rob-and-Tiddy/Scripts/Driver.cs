using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    public bool qualified = false;
    bool Active = false;
    public bool active{
        get{
            return Active;
        }
        set{
            Active = value;
            if(value == true){
                SceneObjects.current.ActiveDriver = this;
            }
        }
    }
    
    public string driverName = "DriverMcDriveFace";
    public GameObject car;
    public CarPhysics carPhysics;

/*
    public lapTime fastestQualifyLap;
    public lapTime fastestRaceLap;*/

    public float currentLapTime = 0;

    public int starterRank = 0;
    // Start is called before the first frame update
    public int tempCarIndex = 0;

    public void selectCar(int carIndex){
        if(car != null){
            Destroy(car);
        }

        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
        carPhysics = car.GetComponent<CarPhysics>();
        carPhysics.driver = this;
    }

    public void switchCar(int carIndex){
        UIController.current.showMessage("You have switched to a different car", 1);
        if(car != null){
            Destroy(car);
        }
        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
        carPhysics = car.GetComponent<CarPhysics>();
        carPhysics.driver = this;
    }

    public void backToGrid(){
        carPhysics.stopAllMovement();
        car.transform.position = SceneObjects.current.gridPositions[starterRank].position;
        car.transform.rotation = SceneObjects.current.gridPositions[starterRank].rotation;
    }

    void onDestroy(){
        if(car != null){
            Destroy(car);
        }
    }

    void Awake()
    {
        selectCar(tempCarIndex);
        //print(car.name);
    }

    // Update is called once per frame
    void Update()
    {
        //TEMPCODE
    }
}
