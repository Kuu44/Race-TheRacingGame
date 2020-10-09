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


    public Text SpeedText;
    public Text StatusText;
    public GameObject UIelement3;

}
