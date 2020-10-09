using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{

    public bool mainCar;


    [SerializeField]
    [Range(0, 400f)]
    private float topSpeed = 200f;

    [SerializeField]
    [Range(0, 10f)]
    private float turnStrength = 5;

    [Range(0f,1f)]
    public float traction = 0.5f;

    [Range(1f,8f)]
    public float orientStrength = 3;

    float thrust = 1;
    public List<ParticleSystem> thrusters;
    Rigidbody self;
    float tractionSpeed;
    Vector3 deltaPosition;
    Vector3 propulsion;
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

    public void StartThrusters(){
            foreach(ParticleSystem thruster in thrusters){
                if(thruster.isStopped){
                    thruster.Play();
                }
            }
    }

    public void StopThrusters(){
            foreach(ParticleSystem thruster in thrusters){
                if(thruster.isPlaying){
                    thruster.Stop();
                }
            }
    }



    public void AccelerateForward(float strength){
        if(Mathf.Abs(strength) > 0.01f){
            tractionSpeed += strength * thrust * topSpeed * 0.01f;
            propulsion += strength * transform.forward * thrust;
            if(thrust > 0.5f){
                StartThrusters();
            }else{
                StopThrusters();
            }
        }else{

            StopThrusters();
        }
    }   


    public void AddUpwardsTorque(float strength){
        if(Mathf.Abs(strength) > 0.01f){
            self.AddTorque(transform.up * strength * turnStrength * 0.04f);
            turning = true;
        }
    }

    bool turning;

    void Start()
    {
        self = transform.GetComponent<Rigidbody>();
        wayPointAheadI = 1;
        wayPointBehindI = 0;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        while(Vector3.Dot(transform.position - wayPointAhead, wayPointBehind-wayPointAhead) < 0){
            wayPointBehindI = indexIncrement(wayPointBehindI, 1, SceneObjects.current.trackWayPoints.Count);
        }
        while(Vector3.Dot(transform.position - wayPointBehind, wayPointAhead-wayPointBehind) < 0){
            wayPointBehindI = indexIncrement(wayPointBehindI, -1, SceneObjects.current.trackWayPoints.Count);
        }

        if(SceneObjects.current.wayPointMarker != null && MainController.current.markClosestWaypoints){
            SceneObjects.current.wayPointMarker.transform.position = wayPointAhead;
            SceneObjects.current.wayPointMarkerBehind.transform.position = wayPointBehind;
        }
        
        
        Vector3 lineDirection = (wayPointAhead - wayPointBehind).normalized;
        
        
        Vector3 closestPointOnTrack = wayPointBehind + lineDirection * Vector3.Dot(lineDirection, transform.position - wayPointBehind);
        Vector3 posVec = closestPointOnTrack - transform.position;
        Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight / Mathf.Pow(posVec.magnitude, MainController.current.gravityExponent);
        
        self.AddForce(gravity*10f);

        Vector3 normalVec = -posVec.normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, posVec, out hit, 100)){
            if(hit.collider.tag == "Track"){
                normalVec = hit.normal;
                if(hit.distance < 1){
                    thrust = 1;
                }else{
                    thrust = 1 / (1f * hit.distance * hit.distance);
                }
                if(turning){
                    thrust *= 1 - MainController.current.turningThrustLoss;
                }
            }
        }
        

        Vector3 orientVec = Vector3.Cross(transform.up, normalVec);
        self.AddTorque(orientVec*orientStrength);
        Debug.DrawLine(transform.position, closestPointOnTrack, Color.black);

        if(Time.frameCount % 10 == 0){
            UIController.current.speed = (int)(tractionSpeed);
            
            tractionSpeed *= MainController.current.airResistance;
            propulsion *= MainController.current.airResistance;
        }


        deltaPosition = Vector3.Slerp(propulsion, transform.forward * tractionSpeed, traction) * (Time.deltaTime) * 0.1f;
        turning = false;
        transform.position += deltaPosition;

    }
}
