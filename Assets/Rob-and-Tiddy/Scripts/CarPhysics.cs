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
        StartCoroutine(physicsLoop());
    }
    Vector3 deltaMovement = Vector3.zero;

    public void AddForce(Vector3 force){
        deltaMovement += force;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 posVec = SceneObjects.current.trackCollider.ClosestPoint(transform.position) - transform.position;
        //Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight / Mathf.Pow(posVec.magnitude, MainController.current.gravityExponent);
        


         
        //AddForce(gravity);
    }

    IEnumerator physicsLoop(){
        for(;;){

            deltaMovement *= MainController.current.airResistance;

            transform.position += deltaMovement;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
