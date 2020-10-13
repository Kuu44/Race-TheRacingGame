using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjects : ControllerBase<SceneObjects>
{
    public override void OnAwake()
    {
        base.OnAwake();
        //WHAT TO DO ON AWAKE
        track = GameObject.FindGameObjectWithTag("TrackBase").transform;
        for(int i = 0; i < track.Find("GridPositions").childCount; i++){
            gridPositions.Add(track.Find("GridPositions").GetChild(i));
        }
        for(int i = 0; i < track.Find("TrackWaypoints").childCount; i++){
            trackWayPoints.Add(track.Find("TrackWaypoints").GetChild(i));
        }
        
        carCam = Camera.main;

    }
 
    
    public Camera carCam;

    Driver activeDriver;
    public Driver ActiveDriver{
        get{
            return activeDriver;
        }set{
            activeDriver = value;
        }
    }
    public List<GameObject> carPrefabs;
    public List<GameObject> cars = new List<GameObject>();

    public List<Driver> drivers;

    public GameObject driverPrefab;
    public Transform track;
    //public List<Vector3> trackEnvelops = new List<Vector3>();

    public List<Transform> trackWayPoints;
    public List<Transform> gridPositions = new List<Transform>();
    public CarController carController;

}
