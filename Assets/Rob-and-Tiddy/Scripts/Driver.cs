using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [Range(0, 100)]
    public float startingFuel = 50;

    public enum Phase { Practicing, Qualifying, Qualified, Racing, PostRace };

    public Phase phase = Phase.Practicing;
    int currentRaceLap = 0;
    int currentQualifyingLap = 0;
    bool preQualifyingLap = false;
    bool Active = false;
    public bool active
    {
        get
        {
            return Active;
        }
        set
        {
            Active = value;
            if (value == true)
            {
                SceneObjects.current.ActiveDriver = this;
            }
        }
    }
    public string driverName = "DriverMcDriveyFace";
    public GameObject car;
    public CarPhysics carPhysics;
    float currentLapTime = 0;
    public int starterRank = 0;
    public int tempCarIndex = 0;
    public List<Transform> wayPointsPassed = new List<Transform>();

    public void selectCar(int carIndex)
    {
        if (car != null)
        {
            Destroy(car);
        }

        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
        carPhysics = car.GetComponent<CarPhysics>();
        carPhysics.driver = this;
        carPhysics.fuel = startingFuel;
    }

    public void switchCar(int carIndex)
    {
        UIController.current.showMessage("You have switched to a different car", 1);
        if (car != null)
        {
            Destroy(car);
        }
        car = Instantiate(SceneObjects.current.carPrefabs[carIndex], SceneObjects.current.gridPositions[starterRank].position, SceneObjects.current.gridPositions[starterRank].rotation);
        carPhysics = car.GetComponent<CarPhysics>();
        carPhysics.driver = this;
        carPhysics.fuel = startingFuel;
    }

    public void backToGrid()
    {
        wayPointsPassed.Clear();
        carPhysics.stopAllMovement();
        car.transform.position = SceneObjects.current.gridPositions[starterRank].position;
        car.transform.rotation = SceneObjects.current.gridPositions[starterRank].rotation;
    }

    int numErrors()
    {
        int result = SceneObjects.current.trackWayPoints.Count - wayPointsPassed.Count;
        if (result < 0) result = 0;

        for (int i = 0; i < Mathf.Min(wayPointsPassed.Count, SceneObjects.current.trackWayPoints.Count); i++)
        {
            if (wayPointsPassed[i] != SceneObjects.current.trackWayPoints[i])
            {
                result++;
            }
        }
        return result;
    }

    public void onPassFlag()
    {
        //print("The main car just passed the chequered Flag!");
        if (numErrors() <= 2)
        {
            //print("Lap completed successfully!");
            if (active)
            {
                UIController.current.addLapTime(new lapTime(currentLapTime));
                UIController.current.showMessage("Your last lap time was " + new lapTime(currentLapTime).lapTimeAsString(), 3);
            }
            if (phase == Phase.Qualifying)
            {
                //RaceManager.current.addQualifyLapTime(currentLapTime, driverName);
                if (preQualifyingLap && currentQualifyingLap < RaceManager.current.numberOfQualifyingLaps)
                {

                    currentQualifyingLap += 1;
                    if (active)
                        UIController.current.StatusText.text = "Qualifying! (Lap " + (currentQualifyingLap + 1).ToString() + ")";
                    if (currentQualifyingLap >= RaceManager.current.numberOfQualifyingLaps)
                    {
                        phase = Phase.Qualified;
                        if (active)
                            UIController.current.StatusText.text = "Waiting";
                    }
                    RaceManager.current.addQualifyLapTime(currentLapTime, driverName);
                }
                else
                {
                    preQualifyingLap = true;
                    if (active)
                    {
                        UIController.current.StatusText.text = "Qualifying! (Lap 1)";
                        UIController.current.showMessage("Good luck! This lap will be counted", 5);
                    }
                }

            }
            else

            if (phase == Phase.Racing)
            {

                currentRaceLap += 1;
                if (currentRaceLap >= RaceManager.current.numberOfRaceLaps)
                {
                    phase = Phase.PostRace;
                    RaceManager.current.addRaceFinishEntry(SceneObjects.current.ActiveDriver);
                }
                else
                {
                    //RaceManager.current.progressLap(SceneObjects.current.ActiveDriver);
                    if (active)
                        UIController.current.StatusText.text = "Race! - Lap " + (currentRaceLap + 1).ToString();
                }
            }


            currentLapTime = 0;
        }
        else
        {
            //print("Invalid lab, you probably cut corners or something");
            if (wayPointsPassed.Count > 0)
            {
                if (active)
                    UIController.current.showMessage("Invalid lap, you might have missed a corner or something", 5);
                if (RaceManager.current.gameStatus == RaceManager.GameStatus.Qualify)
                {
                    ;
                    if (preQualifyingLap)
                    {
                    }
                    else
                    {
                        preQualifyingLap = true;
                        if (active)
                            UIController.current.showMessage("Good luck! This lap will be counted", 5);
                    }

                }
            }

            currentLapTime = 0;
        }
        wayPointsPassed.Clear();
    }

    void onDestroy()
    {
        if (car != null)
        {
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
        if (Input.GetKeyUp("space"))
        {
            carPhysics.stopTurbo();
        }

        if (Input.GetKeyUp("n"))
        {
            //print("N KEY PRESSED");
            tempCarIndex += 1;
            if (tempCarIndex >= SceneObjects.current.carPrefabs.Count)
            {
                tempCarIndex = 0;
            }
            switchCar(tempCarIndex);
        }
        if (Input.GetKeyUp("m"))
        {
            //print("N KEY PRESSED");
            RaceManager.current.startQualify();
            //UIController.current.startQualifyCountDown();
        }
        if (active)
        {
            if (Time.frameCount % 10 == 0)
            {
                UIController.current.setCurrentLapTime(currentLapTime);
            }
        }

        currentLapTime += Time.deltaTime;
        //TEMPCODE
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (SceneObjects.current.ActiveDriver && RaceManager.current.allowCarControl)
        {
            if (carPhysics)
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");

                carPhysics.AccelerateForward(inputY * Time.fixedDeltaTime);
                carPhysics.AddUpwardsTorque(inputX * Time.fixedDeltaTime);

                if (Input.GetKey("space") && phase != Driver.Phase.Qualifying)
                {
                    carPhysics.useTurbo();
                }

                if (Input.GetKey("b"))
                {
                    carPhysics.hardBrake();
                }
            }


        }
        else
        {
            //print("ACTIVE DRIVER ABSENT!");
        }
    }
}
