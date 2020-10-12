using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    // Start is called before the first frame update

    //Race settings
    public int maxNumberOfDrivers = 10;
    public int numberOfQualifyingLaps = 2;
    public int numberOfRaceLaps = 15;
    public bool allowFuel = true;



    [HideInInspector]
    public enum GameStatus {Practice, Qualify, Race, Victory};
    GameStatus gameStatus = GameStatus.Practice;
    [HideInInspector]
    public List<lapTime> qualifyLapTimes = new List<lapTime>();
    [HideInInspector]
    public List<lapTime> raceLapTimes = new List<lapTime>();
    [HideInInspector]
    public lapTime currentLapTime;
    [HideInInspector]
    public float currentLapTimeInSeconds;

    public void startQualify(){
        gameStatus = GameStatus.Qualify;
    }

    public void startRace(){
        gameStatus = GameStatus.Race;
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
            print("DriveyMcDriverFace joined the game");
            joinGame("DriveyMcDriverFace").active = true;
        }
    }
}
