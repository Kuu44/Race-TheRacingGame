using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : ControllerBase<RaceManager>
{
    // Start is called before the first frame update

    //Race settings
    public int maxNumberOfDrivers = 10;
    public int numberOfQualifyingLaps = 2;
    public int numberOfRaceLaps = 15;
    public bool allowFuel = true;

    [HideInInspector]
    public bool allowCarControl = true;

    [HideInInspector]
    public enum GameStatus {Practice, Qualify, Race, Victory};
    public GameStatus gameStatus = GameStatus.Practice;
    [HideInInspector]
    public List<float> allQualifyLapTimes = new List<float>();
    [HideInInspector]
    public List<string> allQualifyLapTimeDrivers = new List<string>();
    [HideInInspector]
    public List<float> rankedQualifyLapTimes = new List<float>();
    [HideInInspector]
    public List<string> rankedQualifyLapTimeDrivers = new List<string>();
    [HideInInspector]
    public List<lapTime> raceLapTimes = new List<lapTime>();
    [HideInInspector]
    public lapTime currentLapTime;
    [HideInInspector]
    public float currentLapTimeInSeconds;

    public void addQualifyLapTime(float timeInSeconds, string driverName){
        allQualifyLapTimeDrivers.Add(driverName);
        allQualifyLapTimes.Add(timeInSeconds);
        bool inserted = false;
        for(int i = 0; i < rankedQualifyLapTimes.Count; i++){
            if(!inserted){
                if(timeInSeconds < rankedQualifyLapTimes[i]){
                    rankedQualifyLapTimes.Insert(i, timeInSeconds);
                    rankedQualifyLapTimeDrivers.Insert(i, driverName);
                    inserted = true;
                }else{
                    if(RaceManager.current.rankedQualifyLapTimeDrivers[i] == driverName){
                        inserted = true;
                        break;
                    }
                }
            }else{
                if(rankedQualifyLapTimeDrivers[i] == driverName){
                    rankedQualifyLapTimeDrivers.RemoveAt(i);
                    rankedQualifyLapTimes.RemoveAt(i);
                }
            }
        }

        if(!inserted && rankedQualifyLapTimes.Count < 10){
            rankedQualifyLapTimes.Add(timeInSeconds);
            rankedQualifyLapTimeDrivers.Add(driverName);
        }

        UIController.current.setQualifyTimes();
        checkQualified();
    }

    void checkQualified(){
        bool QualifyingDone = true;
        for(int i = 1; i < SceneObjects.current.drivers.Count; i++){
            if(SceneObjects.current.drivers[i].qualified == false){
                QualifyingDone = false;
            }
        }
        if(QualifyingDone){
            startRace();
        }
    }

    
    public void startQualify(){
        if(numberOfQualifyingLaps > 0){
            gameStatus = GameStatus.Qualify;
            UIController.current.showMessage("Qualifying has begun! Try for the fastest time after this lap to be ahead at the start!", 10);
            UIController.current.SetQualifyUI();
        }else{
            List<int> nums = new List<int>();
            for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
                nums.Add(i+1);
            }
            for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
                int n = Random.Range(1, nums.Count + 1);
                SceneObjects.current.drivers[i].starterRank = nums[n];
                nums.RemoveAt(n);
            }
            startRace();
        }
    }

    public void startRace(){
        allowCarControl = false;
        gameStatus = GameStatus.Race;
        UIController.current.showMessage("The race is about to begin! Good luck!", 5);
        for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
            SceneObjects.current.drivers[i].backToGrid();
        }
        UIController.current.SetRaceUI();
        UIController.current.startRaceCountDown();
    }

    

    public void finishRace(){
        gameStatus = GameStatus.Victory;
    }

    public Driver joinGame(string driverName){
        if(SceneObjects.current.drivers.Count >= maxNumberOfDrivers){
            return null;
        }else{
            GameObject driverPrefab = Instantiate(SceneObjects.current.driverPrefab, Vector3.zero, Quaternion.identity);
            Driver driverScript = driverPrefab.GetComponent<Driver>();
            driverScript.driverName = driverName;
            driverScript.starterRank = SceneObjects.current.drivers.Count;
            SceneObjects.current.drivers.Add(driverScript);
            return driverScript;
        }
    }

    public void leaveGame(Driver driver){
        SceneObjects.current.drivers.Remove(driver);
        SceneObjects.current.cars.Remove(driver.car);
        Destroy(driver.car);
        Destroy(driver.gameObject);
    }


    void Start()
    {
        gameStatus = GameStatus.Practice;
        UIController.current.SetPracticeUI();

        //TEMPCODE
        
        //END OF TEMPCODE
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount == 30){
            //print("DriveyMcDriverFace joined the game");
            UIController.current.showMessage("DriveyMcDriverFace joined the game", 5);
            joinGame("DriveyMcDriverFace").active = true;
        }
    }
}
