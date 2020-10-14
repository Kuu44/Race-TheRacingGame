using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : ControllerBase<RaceManager>
{
    // Start is called before the first frame update

    //Race settings
    [Range(2,10)]
    public int maxNumberOfDrivers = 10;
    [Range(0,5)]
    public int numberOfQualifyingLaps = 2;
    [Range(1,50)]
    public int numberOfRaceLaps = 15;
    public bool allowFuel = true;

    [HideInInspector]
    public bool allowCarControl = true;

    [HideInInspector]
    public enum GameStatus {Practice, Qualify, Race, Victory};
    [HideInInspector]

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
    [HideInInspector]
    public List<Driver> raceFinishers = new List<Driver>();

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
        for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
            if(SceneObjects.current.drivers[i].phase == Driver.Phase.Qualifying){
                QualifyingDone = false;
                break;
            }
        }
        if(QualifyingDone){
            for(int j = 0; j < SceneObjects.current.drivers.Count; j++){
                SceneObjects.current.drivers[j].phase = Driver.Phase.Racing;
                for(int i = 0; i < rankedQualifyLapTimeDrivers.Count; i++){
                    if(rankedQualifyLapTimeDrivers[i] == SceneObjects.current.drivers[j].driverName){
                        SceneObjects.current.drivers[j].starterRank = i;
                        break;
                    }
                }
            }

            UIController.current.StatusText.text = "Qualifying over!";
            startRace();
        }
    }

    
    public void startQualify(){

        if(numberOfQualifyingLaps > 0){
            UIController.current.startQualifyCountDown();
            for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
                SceneObjects.current.drivers[i].phase = Driver.Phase.Qualifying;
                SceneObjects.current.drivers[i].carPhysics.fuel = SceneObjects.current.drivers[i].startingFuel;
                SceneObjects.current.drivers[i].carPhysics.turbo = 100;
                UIController.current.setFuelSliderAuto();
                UIController.current.setTurboSliderAuto();
            }
        }else{
            List<int> nums = new List<int>();
            for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
                nums.Add(i);
            }
            for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
                int n = Random.Range(0, nums.Count);
                SceneObjects.current.drivers[i].starterRank = nums[n]+1;
                SceneObjects.current.drivers[i].phase = Driver.Phase.Racing;
                SceneObjects.current.drivers[i].carPhysics.fuel = SceneObjects.current.drivers[i].startingFuel;
                SceneObjects.current.drivers[i].carPhysics.turbo = 100;
                UIController.current.setFuelSliderAuto();
                UIController.current.setTurboSliderAuto();
                nums.RemoveAt(n);
            }
            UIController.current.StatusText.text = "Preparing Race";
            startRace();
        }
    }

    public void startRace(){
      
        UIController.current.showMessage("The race is about to begin! Good luck!", 5);
        
        UIController.current.startRaceCountDown();
    }

    public void addRaceFinishEntry(Driver driver){
        raceFinishers.Add(driver);
        UIController.current.setRaceTimes();
        if(raceFinishers.Count == SceneObjects.current.drivers.Count){
            finishRace();
        }
    }
    

    public void finishRace(){
        gameStatus = GameStatus.Victory;
        UIController.current.SetPostRaceUI();
    }

    public Driver joinGame(string driverName, float startingFuelAmount){
        if(SceneObjects.current.drivers.Count >= maxNumberOfDrivers){
            return null;
        }else{
            GameObject driverPrefab = Instantiate(SceneObjects.current.driverPrefab, Vector3.zero, Quaternion.identity);
            Driver driverScript = driverPrefab.GetComponent<Driver>();
            driverScript.driverName = driverName;
            driverScript.startingFuel = startingFuelAmount;
            driverScript.starterRank = SceneObjects.current.drivers.Count;
            SceneObjects.current.drivers.Add(driverScript);
            UIController.current.setDriverTags();
            return driverScript;
        }
    }

    public void leaveGame(Driver driver){
        SceneObjects.current.drivers.Remove(driver);
        SceneObjects.current.cars.Remove(driver.car);
        Destroy(driver.car);
        Destroy(driver.gameObject);
        UIController.current.setDriverTags();
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
            joinGame("DriveyMcDriverFace", 50).active = true;
        }
    }
}
