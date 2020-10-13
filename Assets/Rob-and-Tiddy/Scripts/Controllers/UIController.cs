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

    public void showMessage(string message, float time){

    }

    public void SetPracticeUI(){
        StatusText.text = "Practice";
        qualificationRankings.SetActive(false);
        clearLapTimes();
        RacePosition.gameObject.SetActive(false);
           
    }

    public void SetQualifyUI(){

    }

    public void SetRaceUI(){

    }

    public void SetPostRaceUI(){

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


    public Text SpeedText;
    public Text StatusText;
    
    public Text RacePosition;

    public Text message;
    public Text[] lapTimes;

    public GameObject qualificationRankings;
    public Text[] qualificationRanks;
    public Text currentLapTime;

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
