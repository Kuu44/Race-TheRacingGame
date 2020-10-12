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

/*
    public lapTime fastestQualifyLap;
    public lapTime fastestRaceLap;*/

    public float currentLapTime = 0;

    public int starterRank = 0;
    // Start is called before the first frame update
    int tempCarIndex = 0;

    public void selectCar(int carIndex){
        if(car != null){
            Destroy(car);
        }
        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
    }

    public void switchCar(int carIndex){
        if(car != null){
            Destroy(car);
        }
        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
        if(Active){
            SceneObjects.current.ActiveCar = car;
            SceneObjects.current.carController.carPhysics = car.GetComponent<CarPhysics>();
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
        if(active){
            currentLapTime += Time.deltaTime;
            if(Time.frameCount % 10 == 0){
                UIController.current.setCurrentLapTime(currentLapTime);
            }
            if(Input.GetKeyDown("n")){
                tempCarIndex += 1;
                if(tempCarIndex >= SceneObjects.current.carPrefabs.Count){
                    tempCarIndex = 0;
                }
                switchCar(tempCarIndex);
            }
        }
    }
}
