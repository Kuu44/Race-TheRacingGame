using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{
    // Start is called before the first frame update

    //CarPhysics myCarPhysics;
    void Start()
    {
        //myCarPhysics = this.GetComponent<CarPhysics>();
        Collider temp = SceneObjects.current.trackCollider;
    }


    void AddForce(Vector3 force){
        transform.position += force;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posVec = SceneObjects.current.trackCollider.ClosestPoint(transform.position) - transform.position;
        Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight / Mathf.Pow(posVec.magnitude, MainController.current.gravityExponent);
        //AddForce(gravity);
    }
}
