using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBase<C> : MonoBehaviour where C : ControllerBase<C>
{

    
    static C _current;
    public static C current
    {
        get
        {
            if (_current == null)
                Debug.Log(typeof(C) + " NOT FOUND");

            return _current;
        }
    }

    private void Awake()
    {
        _current = this as C;

        OnAwake();
    }

    public virtual void OnAwake()
    {

    }
}
