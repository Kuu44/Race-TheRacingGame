using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    
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
        if(car != null){
            Destroy(car);
        }
        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
        carPhysics = car.GetComponent<CarPhysics>();
        carPhysics.driver = this;
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
