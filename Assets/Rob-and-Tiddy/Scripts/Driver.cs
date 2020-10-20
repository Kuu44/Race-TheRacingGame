using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Phase { Practicing, Qualifying, Qualified, Racing, PostRace };


public class Driver : NetworkBehaviour
{

    int randomPos
    {
        get
        {
            return Random.Range(0, 100);
        }
    }

    [Range(0, 100)]
    public float startingFuel = 50;

    [SyncVar]
    public Phase phase = Phase.Practicing;

    int currentRaceLap = 0;
    int currentQualifyingLap = 0;
    bool preQualifyingLap = false;
    //bool Active = false;
    /*public bool active
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
    }*/
    public string driverName = "DriverMcDriveyFace";
    public GameObject car;
    public CarPhysics carPhysics;
    float currentLapTime = 0;
    [SyncVar]
    public int starterRank = 0;
    [SyncVar]
    public int tempCarIndex = 0;
    public List<Transform> wayPointsPassed = new List<Transform>();

    [ClientRpc]
    public void RpcSelectCar(int carIndex)
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

    [ClientRpc]
    public void RpcSwitchCar(int carIndex)
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

    [ClientRpc]
    public void RpcBackToGrid()
    {
        wayPointsPassed.Clear();
        carPhysics.stopAllMovement();
        car.transform.position = SceneObjects.current.gridPositions[starterRank].position;
        car.transform.rotation = SceneObjects.current.gridPositions[starterRank].rotation;
    }

    int TargetNumErrors()
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

    [TargetRpc]
    public void TargetOnPassFlag()
    {
        //print("The main car just passed the chequered Flag!");
        if (TargetNumErrors() <= 2)
        {
            //print("Lap completed successfully!");
            UIController.current.addLapTime(new lapTime(currentLapTime));
            UIController.current.showMessage("Your last lap time was " + new lapTime(currentLapTime).lapTimeAsString(), 3);

            if (phase == Phase.Qualifying)
            {
                //RaceManager.current.addQualifyLapTime(currentLapTime, driverName);
                if (preQualifyingLap && currentQualifyingLap < RaceManager.current.numberOfQualifyingLaps)
                {

                    currentQualifyingLap += 1;

                    UIController.current.StatusText.text = "Qualifying! (Lap " + (currentQualifyingLap + 1).ToString() + ")";
                    if (currentQualifyingLap >= RaceManager.current.numberOfQualifyingLaps)
                    {
                        phase = Phase.Qualified;

                        UIController.current.StatusText.text = "Waiting";
                    }
                    RaceManager.current.RpcAddQualifyLapTime(currentLapTime, driverName);
                }
                else
                {
                    preQualifyingLap = true;

                    UIController.current.StatusText.text = "Qualifying! (Lap 1)";
                    UIController.current.showMessage("Good luck! This lap will be counted", 5);

                }
            }
            else

            if (phase == Phase.Racing)
            {

                currentRaceLap += 1;
                if (currentRaceLap >= RaceManager.current.numberOfRaceLaps)
                {
                    phase = Phase.PostRace;
                    RaceManager.current.CmdAddRaceFinishEntry(gameObject);
                }
                else
                {
                    //RaceManager.current.progressLap(SceneObjects.current.ActiveDriver);

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

                        UIController.current.showMessage("Good luck! This lap will be counted", 5);
                    }

                }
            }

            currentLapTime = 0;
        }
        wayPointsPassed.Clear();
    }

    void OnDestroy()
    {

        SceneObjects.current.drivers.Remove(gameObject);
        if (car != null)
        {
            Destroy(car);
        }
    }

    [TargetRpc]
    public void TargetSetUI()
    {
        UIController.current.setFuelSlider(carPhysics.fuel);
        UIController.current.setTurboSlider(carPhysics.turbo);
    }


    [TargetRpc]
    public void TargetSetSpeedUI()
    {
        UIController.current.speed = carPhysics.speed;
    }

    [ClientRpc]
    public void RpcStartThrusters()
    {
        carPhysics.StartThrusters();
    }

    [ClientRpc]
    public void RpcStopThrusters()
    {
        carPhysics.StopThrusters();
    }

    void Awake()
    {
        
        //print(car.name);
    }

    [ClientRpc]
    void RpcSetDriverStartUI()
    {
        UIController.current.setDriverTags();
        UIController.current.showMessage(driverName + " just joined the game!", 3);
    }

    [ClientRpc]
    void RpcSetDriverDestroyUI()
    {
        UIController.current.showMessage(driverName + " left the game", 3);
        UIController.current.setDriverTags();
    }

    void Start()
    {
        startingFuel = 50;
        starterRank = SceneObjects.current.drivers.Count;
        RpcSelectCar(tempCarIndex);
        SceneObjects.current.drivers.Add(gameObject);
        RpcSetDriverStartUI();
        driverName = "Driverface " + randomPos.ToString();
    }

    // Update is called once per frame

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }


        Move();


        if (Time.frameCount % 10 == 0)
        {
            UIController.current.setCurrentLapTime(currentLapTime);
        }


        currentLapTime += Time.deltaTime;
        //TEMPCODE
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        FixedMove();

    }

    float cameraDistanceBehind = 2f;
    float cameraDistanceAbove = 1.5f;
    float lookDistanceAboveCar = 0.2f;

    [Range(0, 1)]
    float lookSpeed = 0.04f;
    [Range(0, 1)]
    float followSpeed = 0.1f;
    // Start is called before the first frame update

    // Update is called once per frame

    void Move()
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
            RpcSwitchCar(tempCarIndex);
        }
        if (Input.GetKeyUp("m"))
        {
            //print("N KEY PRESSED");
            RaceManager.current.startQualify();
            //UIController.current.startQualifyCountDown();
        }

    }


    void FixedMove()
    {
        if (car != null)
        {
            Vector3 carPos = car.transform.position;
            Vector3 carBack = -car.transform.forward;
            Vector3 carUp = car.transform.up;
            Vector3 movetarget = carPos + carBack * cameraDistanceBehind + carUp * cameraDistanceAbove;
            Vector3 lookTarget = carPos + carUp * lookDistanceAboveCar;

            Camera.main.transform.LookAt(Vector3.Lerp(Camera.main.transform.position + Camera.main.transform.forward, lookTarget, lookSpeed), Vector3.Slerp(Camera.main.transform.up, carUp, lookSpeed));

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, movetarget, followSpeed);

        }
        if (RaceManager.current.allowCarControl)
        {
            if (carPhysics)
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");

                carPhysics.AccelerateForward(inputY * Time.fixedDeltaTime);
                carPhysics.AddUpwardsTorque(inputX * Time.fixedDeltaTime);

                if (Input.GetKey("space") && phase != Phase.Qualifying)
                {
                    carPhysics.useTurbo();
                }

                if (Input.GetKey("b"))
                {
                    carPhysics.hardBrake();
                }
            }


        }
    }
}
