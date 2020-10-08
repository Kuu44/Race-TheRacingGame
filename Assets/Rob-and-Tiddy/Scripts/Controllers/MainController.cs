using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : ControllerBase<MainController>
{
    [Range(0,5f)]
    public float gravityConstant = 0.5f;

    [Range(1,3f)]
    public float gravityExponent = 1.5f;
    
    public float averageCarWeight = 1;

    [Range(0.1f,1f)]
    public float airResistance = 0.95f;

    public override void OnAwake()
    {
        base.OnAwake();

        //WHAT TO DO ON AWAKE
    }


}
