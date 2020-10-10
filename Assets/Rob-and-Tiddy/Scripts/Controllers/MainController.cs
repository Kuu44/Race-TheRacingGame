using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : ControllerBase<MainController>
{
    [Range(0,5f)]
    public float gravityConstant = 0.5f;

    [Range(0.2f,3f)]
    public float gravityExponent = 0.8f;

    [Range(0,1)]
    public float turningThrustLoss = 0.2f;
    
    public float averageCarWeight = 1;

    //[Range(0f,1f)]
    public float airResistance = 0.95f;

    public float kineticFriction = 0.1f;

    [Range(2,20)]
    public int checkWayPointsPerFrame = 5;

    public bool markClosestWaypoints = true;

    public override void OnAwake()
    {
        base.OnAwake();

        //WHAT TO DO ON AWAKE
    }


}
