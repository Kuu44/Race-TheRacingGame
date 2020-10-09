using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjects : ControllerBase<SceneObjects>
{
    public override void OnAwake()
    {
        base.OnAwake();
        //WHAT TO DO ON AWAKE
        

    }
 
    public Camera carCam;
    public Transform track;

    public GameObject trackDirector;

    public List<Vector3> trackWayPoints = new List<Vector3>();

}
