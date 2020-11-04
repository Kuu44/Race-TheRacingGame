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

        if (track.Find("TrackNodes") != null)
        {
            nodeCount =  track.Find("TrackNodes").childCount;
            nodesExist = true;
            for (int i = 0; i < nodeCount; i++)
            {
                trackNodes.Add(track.Find("TrackNodes").GetChild(i).position);
                nodes.Add(track.Find("TrackNodes").GetChild(i));
                nodes[i].name = i.ToString();
 
                int ii = i+1;
                if(ii == nodeCount)
                {
                    ii = 0;
                }
                trackConnections.Add(track.Find("TrackNodes").GetChild(ii).position - trackNodes[i]);
                nodePercentages.Add(trackLength);
                trackLength += trackConnections[i].magnitude;
                
            }

            for (int i = 0; i < nodeCount; i++)
            {
                nodePercentages[i] = nodePercentages[i] * 100 / (trackLength * 1.0f);

                int ii = i + 1;
                if (ii == nodeCount)
                {
                    ii = 0;
                }
                nodes[i].rotation = Quaternion.identity;
                CapsuleCollider cap = nodes[i].GetComponent<CapsuleCollider>();
                cap.height = (nodes[ii].localPosition - nodes[i].localPosition).magnitude + 3f;
                cap.radius = 1.5f;
                cap.center = new Vector3(0,0,(nodes[ii].localPosition - nodes[i].localPosition).magnitude * 0.5f);
                nodes[i].rotation = Quaternion.FromToRotation(nodes[i].forward, nodes[ii].position - nodes[i].position);


                trackConnections.Add(track.Find("TrackNodes").GetChild(ii).position - trackNodes[i]);
            }
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

    public bool nodesExist = false;
    public int nodeCount = 0;

    public List<Transform> nodes;
    public List<Vector3> trackNodes;
    public List<Vector3> trackConnections;
    public List<float> nodePercentages;
    public float trackLength;

    public List<GameObject> carPrefabs;
    //public List<GameObject> cars = new List<GameObject>();
    [SyncVar]
    public List<GameObject> drivers;

    public GameObject driverPrefab;


    public Transform track;
    //public List<Vector3> trackEnvelops = new List<Vector3>();

    public List<Transform> trackWayPoints;
    public List<Transform> gridPositions = new List<Transform>();

    public List<Transform> nodeMarkers;
}
