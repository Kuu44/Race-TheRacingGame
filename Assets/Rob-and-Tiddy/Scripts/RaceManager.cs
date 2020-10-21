using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RaceManager : NetworkBehaviour
{
    static RaceManager _current;
    public static RaceManager current
    {
        get
        {
            if (_current == null)
                Debug.Log(typeof(RaceManager) + " NOT FOUND");

            return _current;
        }
    }

    void Awake()
    {
        _current = this as RaceManager;
    }



    // Start is called before the first frame update

    //Race settings
    [Range(2,10)]
    public int maxNumberOfDrivers = 10;
    [Range(0,5)]
    public int numberOfQualifyingLaps = 2;
    [Range(1,50)]
    public int numberOfRaceLaps = 15;
    public bool allowFuel = true;

    public Driver driver(int index)
    {
        return SceneObjects.current.drivers[index].GetComponent<Driver>();
    }

    [SyncVar]
    [HideInInspector]
    public bool allowCarControl = true;

    
    [HideInInspector]
    public enum GameStatus {Practice, Qualify, Race, Victory};
    [HideInInspector]

    [SyncVar]
    public GameStatus gameStatus = GameStatus.Practice;
    [HideInInspector]
    [SyncVar]
    public List<float> allQualifyLapTimes = new List<float>();
    [HideInInspector]
    [SyncVar]
    public List<string> allQualifyLapTimeDrivers = new List<string>();
    [HideInInspector]
    [SyncVar]
    public List<float> rankedQualifyLapTimes = new List<float>();
    [HideInInspector]
    [SyncVar]
    public List<string> rankedQualifyLapTimeDrivers = new List<string>();
    [HideInInspector]
    [SyncVar]
    public List<lapTime> raceLapTimes = new List<lapTime>();
    [SyncVar]
    [HideInInspector]
    public List<GameObject> raceFinishers = new List<GameObject>();

    [Header("Local Variables")]
    [HideInInspector]
    public lapTime currentLapTime;
    [HideInInspector]
    public float currentLapTimeInSeconds;


    [Command]
    public void CmdAddQualifyLapTime(float timeInSeconds, string driverName){
        RpcAddQualifyLapTime(timeInSeconds, driverName);
    }

    [ClientRpc]
    public void RpcAddQualifyLapTime(float timeInSeconds, string driverName)
    {
        allQualifyLapTimeDrivers.Add(driverName);
        allQualifyLapTimes.Add(timeInSeconds);
        bool inserted = false;
        for (int i = 0; i < rankedQualifyLapTimes.Count; i++)
        {
            if (!inserted)
            {
                if (timeInSeconds < rankedQualifyLapTimes[i])
                {
                    rankedQualifyLapTimes.Insert(i, timeInSeconds);
                    rankedQualifyLapTimeDrivers.Insert(i, driverName);
                    inserted = true;
                }
                else
                {
                    if (rankedQualifyLapTimeDrivers[i] == driverName)
                    {
                        inserted = true;
                        break;
                    }
                }
            }
            else
            {
                if (rankedQualifyLapTimeDrivers[i] == driverName)
                {
                    rankedQualifyLapTimeDrivers.RemoveAt(i);
                    rankedQualifyLapTimes.RemoveAt(i);
                }
            }
        }

        if (!inserted && rankedQualifyLapTimes.Count < 10)
        {
            rankedQualifyLapTimes.Add(timeInSeconds);
            rankedQualifyLapTimeDrivers.Add(driverName);
        }

        UIController.current.setQualifyTimes();
        CmdCheckQualified();
    }

    [Command]
    void CmdCheckQualified(){
        RpcCheckQualified();
    }

    [ClientRpc]
    void RpcCheckQualified()
    {
        bool QualifyingDone = true;
        for (int i = 0; i < SceneObjects.current.drivers.Count; i++)
        {
            if (driver(i).phase == Phase.Qualifying)
            {
                QualifyingDone = false;
                break;
            }
        }
        if (QualifyingDone)
        {
            for (int j = 0; j < SceneObjects.current.drivers.Count; j++)
            {
                driver(j).phase = Phase.Racing;
                for (int i = 0; i < rankedQualifyLapTimeDrivers.Count; i++)
                {
                    if (rankedQualifyLapTimeDrivers[i] == driver(j).driverName)
                    {
                        driver(j).starterRank = i;
                        break;
                    }
                }
            }

            UIController.current.StatusText.text = "Qualifying over!";
            CmdStartRace();
        }
    }


    [Command]
    public void CmdStartQualify(){

        RpcStartQualify();
    }

    [ClientRpc]
    public void RpcStartQualify()
    {
        if (numberOfQualifyingLaps > 0)
        {
            startQualifyCountDown();
            for (int i = 0; i < SceneObjects.current.drivers.Count; i++)
            {
                driver(i).phase = Phase.Qualifying;
                driver(i).carPhysics.fuel = driver(i).startingFuel;
                driver(i).carPhysics.turbo = 100;
                /*UIController.current.setFuelSliderAuto();
                UIController.current.setTurboSliderAuto();*/
            }
        }
        else
        {
            List<int> nums = new List<int>();
            for (int i = 0; i < SceneObjects.current.drivers.Count; i++)
            {
                nums.Add(i);
            }
            for (int i = 0; i < SceneObjects.current.drivers.Count; i++)
            {
                int n = Random.Range(0, nums.Count);
                driver(i).starterRank = nums[n] + 1;
                driver(i).phase = Phase.Racing;
                driver(i).carPhysics.fuel = driver(i).startingFuel;
                driver(i).carPhysics.turbo = 100;
                /*UIController.current.setFuelSliderAuto();
                UIController.current.setTurboSliderAuto();*/
                nums.RemoveAt(n);
            }
            UIController.current.StatusText.text = "Preparing Race";
            CmdStartRace();
        }
    }


    [Command]
    public void CmdStartRace(){

        RpcStartRace();
    }

    [ClientRpc]
    public void RpcStartRace()
    {

        UIController.current.showMessage("The race is about to begin! Good luck!", 5);

        startRaceCountDown();
    }


    IEnumerator qualifyCountDown()
    {
        string phaseString = "Qualifying";
        if (RaceManager.current.numberOfQualifyingLaps == 0)
        {
            phaseString = "Race";
        }
        UIController.current.RacePosition.gameObject.SetActive(true);
        UIController.current.StatusText.text = phaseString + " starts in ";
        UIController.current.RacePosition.text = "6";
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            UIController.current.StatusText.text = phaseString + " starts in ";
            UIController.current.RacePosition.text = (5 - i).ToString();
        }
        yield return new WaitForSeconds(1);
        gameStatus = GameStatus.Qualify;
        UIController.current.showMessage("Qualifying has begun! Try for the fastest time after this lap to be ahead at the start!", 10);
        UIController.current.SetQualifyUI();
    }

    IEnumerator raceCountDown()
    {
        UIController.current.RacePosition.gameObject.SetActive(true);
        UIController.current.RacePosition.text = "READY!";
        yield return new WaitForSeconds(5);
        RaceManager.current.gameStatus = RaceManager.GameStatus.Race;
        RaceManager.current.allowCarControl = false;
        for (int i = 0; i < SceneObjects.current.drivers.Count; i++)
        {
            driver(i).CmdBackToGrid();
        }
        UIController.current.SetRaceUI();
        UIController.current.StatusText.text = "THE RACE!";
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            UIController.current.RacePosition.text = (5 - i).ToString();
        }
        yield return new WaitForSeconds(1);
        UIController.current.StatusText.text = "Race! - Lap 1";
        RaceManager.current.allowCarControl = true;
        UIController.current.RacePosition.text = "GO!";
        yield return new WaitForSeconds(3);
        UIController.current.RacePosition.text = "";
    }

    public void startQualifyCountDown()
    {
        StartCoroutine(qualifyCountDown());
    }

    public void startRaceCountDown()
    {
        StartCoroutine(raceCountDown());
    }

    [Command]
    public void CmdAddRaceFinishEntry(GameObject driver){
        raceFinishers.Add(driver);
        UIController.current.setRaceTimes();
        if(raceFinishers.Count == SceneObjects.current.drivers.Count){
            finishRace();
        }
    }
    
    [Server]
    public void finishRace(){
        gameStatus = GameStatus.Victory;
        UIController.current.SetPostRaceUI();
    }

    /*[ClientRpc]
    public void RpcJoinGame(string driverName, float startingFuelAmount){
        if(SceneObjects.current.drivers.Count >= maxNumberOfDrivers){
            return;
        }else{
            GameObject driverPrefab = Instantiate(SceneObjects.current.driverPrefab, Vector3.zero, Quaternion.identity);
            Driver driverScript = driverPrefab.GetComponent<Driver>();
            driverScript.driverName = driverName;
            driverScript.startingFuel = startingFuelAmount;
            driverScript.starterRank = SceneObjects.current.drivers.Count;
            SceneObjects.current.drivers.Add(driverScript);
            UIController.current.setDriverTags();
            UIController.current.showMessage(driverName + " just joined the game!", 3);
            //return driverScript;
        }
    }*/
    /*
    [ClientRpc]
    public void RpcLeaveGame(Driver driver){

    }*/

    [Command]
    public void CmdRefreshAllCarModels()
    {
        RpcRefreshAllCarModels();
    }

    [ClientRpc]
    void RpcRefreshAllCarModels()
    {
        for(int i = 0; i < SceneObjects.current.drivers.Count; i++)
        {
            driver(i).CmdSelectCar(driver(i).tempCarIndex);
        }
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
    }
}
