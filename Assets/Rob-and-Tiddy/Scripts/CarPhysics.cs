using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{
    // Start is called before the first frame update

    //CarPhysics myCarPhysics;
    public Rigidbody self;
    public GameObject wayPointMarker;
    public GameObject wayPointMarkerBehind;
    float deltaForwards;
    //float deltaUpwardsRotation;
    int wayPointAheadIndex;
    int wayPointBehindIndex;

    int wayPointAheadI{
        get{
            return wayPointAheadIndex;
        }
        set{
            wayPointAheadIndex = value;
            wayPointAhead = SceneObjects.current.trackWayPoints[value];
        }
    }

    int wayPointBehindI{
        get{
            return wayPointBehindIndex;
        }
        set{
            int tempvalue = value;
            wayPointBehindIndex = tempvalue;
            wayPointBehind = SceneObjects.current.trackWayPoints[tempvalue];
            tempvalue = indexIncrement(tempvalue, 1, SceneObjects.current.trackWayPoints.Count);
            wayPointAheadIndex = tempvalue;
            wayPointAhead = SceneObjects.current.trackWayPoints[tempvalue];
        }
    }


    int indexIncrement(int index, int delta, int limit){
        index = index + delta;
        //Debug.Log("First step " + index);
        if(index >= limit){
            index = index % limit;
        }
        if(index < 0){
            while(index < 0){
                index = limit + index;
            }
        }
        return index;
    }

    Vector3 wayPointAhead;
    Vector3 wayPointBehind;
    int numWayPoints;
    public void AddForce(Vector3 force){
       // deltaMovement += force * Time.deltaTime;
        self.AddForce(force);
        //transform.position += force * Time.deltaTime;
    }

    public void AccelerateForward(float strength){
        deltaForwards += strength;
    }


    public void AddUpwardsTorque(float strength){
        self.AddTorque(transform.up * strength * 0.1f);
    }

    void Start()
    {
        StartCoroutine(physicsLoop());
        wayPointAheadI = 1;
        wayPointBehindI = 0;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector3.Dot(transform.position - wayPointAhead, wayPointBehind-wayPointAhead));
        while(Vector3.Dot(transform.position - wayPointAhead, wayPointBehind-wayPointAhead) < 0){
            wayPointBehindI = indexIncrement(wayPointBehindI, 1, SceneObjects.current.trackWayPoints.Count);
        }
        while(Vector3.Dot(transform.position - wayPointBehind, wayPointAhead-wayPointBehind) < 0){
            wayPointBehindI = indexIncrement(wayPointBehindI, -1, SceneObjects.current.trackWayPoints.Count);
        }
        wayPointMarker.transform.position = wayPointAhead;
        wayPointMarkerBehind.transform.position = wayPointBehind;Vector3 lineDirection = (wayPointAhead - wayPointBehind).normalized;
        
        
        Vector3 closestPointOnTrack = wayPointBehind + lineDirection * Vector3.Dot(lineDirection, transform.position - wayPointBehind);
        Vector3 posVec = closestPointOnTrack - transform.position;
        Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight / Mathf.Pow(posVec.magnitude, MainController.current.gravityExponent);
        
        self.AddForce(gravity*10f);


        Vector3 orientVec = Vector3.Cross(transform.up, -posVec.normalized);
        self.AddTorque(orientVec*2f);
        Debug.DrawLine(transform.position, closestPointOnTrack, Color.black);

        transform.position += transform.forward * deltaForwards * Time.deltaTime * 0.1f;
        //AddForce(gravity);

    }

    IEnumerator physicsLoop(){
        for(;;){


            deltaForwards *= MainController.current.airResistance;

            
            yield return new WaitForSeconds(0.05f);
        }
    }
}
