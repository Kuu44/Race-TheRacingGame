using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : ControllerBase<UIController>
{
    public override void OnAwake()
    {
        base.OnAwake();

        //WHAT TO DO ON AWAKE

    }

    //ADD OBJECTS LIKE THIS
    int Speed;
    public int speed{
        get{
            return Speed;
        }
        set{
            Speed = value;
            SpeedText.text = "SPEED: " + value.ToString() + " km/hr";
        }
    }

    public void showMessage(string theMessage, float time){
        StopCoroutine("messageCoroutine");
        message.gameObject.SetActive(true);
        message.text = theMessage;
        StartCoroutine(messageCoroutine(time));

    }

    IEnumerator messageCoroutine(float time){
        yield return new WaitForSeconds(time);
        message.gameObject.SetActive(false);
    }

    public void SetPracticeUI(){
        StatusText.text = "Practice";
        qualificationRankings.SetActive(false);
        clearLapTimes();
        RacePosition.gameObject.SetActive(false);
           
    }

    public void SetQualifyUI(){
        StatusText.text = "Qualifying!";
        qualificationRankings.SetActive(true);
        clearLapTimes();
        clearQualifyTimes();
        RacePosition.gameObject.SetActive(false);
    }
    public void SetRaceUI(){
        StatusText.text = "Race! Lap 1";
        qualificationRankings.SetActive(false);
        clearLapTimes();
        clearRaceTimes();
        RacePosition.gameObject.SetActive(true);
    }

    public void SetPostRaceUI(){
        StatusText.text = "Race is over!";
        qualificationRankings.SetActive(false);
        clearLapTimes();
        RacePosition.gameObject.SetActive(true);
        currentLapTime.gameObject.SetActive(false);
        
    }



    public void clearRelevantUI(){

    }

    public void setCurrentLapTime(float timeInSeconds){
        currentLapTime.text = new lapTime(timeInSeconds).lapTimeAsString();
    }

    public void addLapTime(lapTime newLapTime){
        for(int i = lapTimes.Length-1; i > 0; i--){
            lapTimes[i].text = lapTimes[i-1].text;
        }
        lapTimes[0].text = newLapTime.lapTimeAsString();
    }

    public void clearLapTimes(){
        for(int i = 0; i < lapTimes.Length; i++){
            lapTimes[i].text = "";
        }
    }

    public void clearQualifyTimes(){
        for(int i = 0; i < qualificationRanks.Length; i++){
            qualificationRanks[i].text = "";
        }
    }

    public void setQualifyTimes(){
        for(int i = 0; i < Mathf.Min(RaceManager.current.rankedQualifyLapTimeDrivers.Count, qualificationRanks.Length); i++){
            qualificationRanks[i].text = RaceManager.current.rankedQualifyLapTimeDrivers[i] + "   " + new lapTime(RaceManager.current.rankedQualifyLapTimes[i]).lapTimeAsString();
        }
    }

    public void clearRaceTimes(){
        for(int i = 0; i < raceRanks.Length; i++){
            raceRanks[i].text = "";
        }
    }

    public void setRaceTimes(){
        if(raceRankings.activeSelf == false){
            raceRankings.SetActive(true);
        }
        for(int i = 0; i < Mathf.Min(RaceManager.current.rankedQualifyLapTimeDrivers.Count, qualificationRanks.Length); i++){
            raceRanks[i].text = (i+1).ToString() + ". "+ RaceManager.current.raceFinishers[i].GetComponent<Driver>().driverName;
        }
    }


    public Text SpeedText;
    public Text StatusText;
    
    public Text RacePosition;

    public Text message;
    public Text[] lapTimes;

    public GameObject qualificationRankings;
    public Text[] qualificationRanks;

    public GameObject slipstreamIndicator;
    public GameObject turboIndicator;

    public GameObject raceRankings;
    public Text[] raceRanks;
    public Text currentLapTime;

    public Text[] driverNameTags;

    public InputField driverNameField;

    public GameObject spectatorModeIndicator;

    public void closeNameWindow(){
        driverNameField.gameObject.SetActive(false);
    }

    public Slider fuel;
    public Slider turbo;

    public void setFuelSlider(float amount){
        fuel.value = amount;
    }

    public void setTurboSlider(float amount){
        turbo.value = amount;
    }

    /*public void setFuelSliderAuto(){
        fuel.value = SceneObjects.current.ActiveDriver.carPhysics.fuel;
    }*/

    /*public void setTurboSliderAuto(){
        fuel.value = SceneObjects.current.ActiveDriver.carPhysics.turbo;
    }*/

    public void setDriverTags(){
        for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
            driverNameTags[i].text = RaceManager.current.driver(i).driverName;
        }
    }
    void Update(){
        for(int i = 0; i < SceneObjects.current.drivers.Count; i++){
            GameObject tempCar = SceneObjects.current.drivers[i];
            if (tempCar != null)
            {
                Transform carTransform = tempCar.transform;
                Vector3 pos = Camera.main.WorldToScreenPoint(carTransform.position + carTransform.up);
                if(pos.z > 0){
                    driverNameTags[i].rectTransform.anchoredPosition = pos;
                }
            }
        }
    }

}

public struct lapTime{
    int minutes;
    float seconds;
    public lapTime(int mins, float secs){
        minutes = mins + (int)(secs/60);
        seconds = secs % 60;
    }

    public lapTime(float secs){
        minutes = (int)(secs/60);
        seconds = secs % 60;
    }

    public string lapTimeAsString(){
        string minString = minutes.ToString();
        string secString = ((int)(seconds * 1000) / 1000.0f).ToString();
        if(minutes < 10){
            minString = "0" + minString;
        }
        if(seconds < 10){
            secString = "0" + secString;
        }
        return minString + ":" + secString;
    }

    public float toSeconds(){
        return minutes*60 + seconds;
    }

    public static lapTime operator + (lapTime a, lapTime b){     
        return new lapTime(a.minutes + b.minutes, a.seconds + b.seconds);
    }

    public static lapTime operator - (lapTime a, lapTime b){     
        return new lapTime(a.minutes - b.minutes, a.seconds - b.seconds);
    }

    public static bool operator > (lapTime a, lapTime b){     
        return a.toSeconds() > b.toSeconds();
    }

    public static bool operator < (lapTime a, lapTime b){     
        return a.toSeconds() < b.toSeconds();
    }

    public static bool operator >= (lapTime a, lapTime b){     
        return a.toSeconds() >= b.toSeconds();
    }

    public static bool operator <= (lapTime a, lapTime b){     
        return a.toSeconds() <= b.toSeconds();
    }

    /*public static bool operator == (lapTime a, lapTime b){     
        return a.toSeconds() == b.toSeconds();
    }

    public static bool operator != (lapTime a, lapTime b){     
        return a.toSeconds() != b.toSeconds();
    }*/
}
