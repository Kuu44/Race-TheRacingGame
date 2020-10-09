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

        cars.AddRange(GameObject.FindGameObjectsWithTag("Car"));
        if(cars.Count > 0){
            ActiveCar = cars[0];
        }


        for(int i = 0; i < track.Find("GridPositions").childCount; i++){
            gridPositions.Add(track.Find("GridPositions").GetChild(i));
        }

        carCam = Camera.main;

    }
 
    
    public Camera carCam;

    public GameObject ActiveCar;
    public GameObject defaultCar;
    public List<GameObject> cars = new List<GameObject>();
    public Transform track;

    [HideInInspector]
    public GameObject trackDirector;

    public List<Vector3> trackWayPoints = new List<Vector3>();
    public List<Transform> gridPositions = new List<Transform>();


    //Testing
    public GameObject wayPointMarker;
    public GameObject wayPointMarkerBehind;

}
