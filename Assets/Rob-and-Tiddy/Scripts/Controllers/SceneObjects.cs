using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SceneObjects : NetworkBehaviour
{
    static SceneObjects _current;
    public static SceneObjects current
    {
        get
        {
            if (_current == null)
                Debug.Log(typeof(SceneObjects) + " NOT FOUND");

            return _current;
        }
    }


    void Awake()
    {
        _current = this as SceneObjects;

        track = GameObject.FindGameObjectWithTag("TrackBase").transform;

        for (int i = 0; i < track.Find("TrackWaypoints").childCount; i++){
            trackWayPoints.Add(track.Find("TrackWaypoints").GetChild(i));
        }
        track = GameObject.FindGameObjectWithTag("TrackBase").transform;
        for (int i = 0; i < track.Find("GridPositions").childCount; i++)
        {
            gridPositions.Add(track.Find("GridPositions").GetChild(i));
        }
        //carCam = Camera.main;

    }
 
    [System.Obsolete]
    public Camera carCam;

    //Driver activeDriver;
    /*public Driver ActiveDriver{
        get{
            return activeDriver;
        }set{
            activeDriver = value;
        }
    }*/
    public List<GameObject> carPrefabs;
    //public List<GameObject> cars = new List<GameObject>();
    [SyncVar]
    public List<GameObject> drivers;

    public GameObject driverPrefab;


    public Transform track;
    //public List<Vector3> trackEnvelops = new List<Vector3>();

    public List<Transform> trackWayPoints;
    public List<Transform> gridPositions = new List<Transform>();


}
