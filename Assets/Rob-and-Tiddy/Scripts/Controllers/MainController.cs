using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : ControllerBase<MainController>
{
    [Range(0,5f)]
    public float gravityConstant;

    public float averageCarWeight;

    [Range(1,3f)]
    public float gravityExponent;


    public override void OnAwake()
    {
        base.OnAwake();

        //WHAT TO DO ON AWAKE
    }


}
