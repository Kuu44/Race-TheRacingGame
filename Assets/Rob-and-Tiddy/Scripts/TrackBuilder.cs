using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
    public Transform trackWayPoints;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < trackWayPoints.childCount; i++){
            SceneObjects.current.trackWayPoints.Add(trackWayPoints.GetChild(i).position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
