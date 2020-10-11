using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{

    public bool mainCar;


    [SerializeField]
    [Range(10, 100f)]
    private float aerodynamic = 50f;

    [SerializeField]
    [Range(10,100f)]
    private float acceleration = 50f;

    [SerializeField]
    [Range(0, 10f)]
    private float turnStrength = 5;

    [Range(0f,1f)]
    public float traction = 0.5f;

    [Range(1f,8f)]
    public float orientStrength = 3;

    [Range(1,10)]
    public float brakeFactor = 5;

    float thrust = 1;
    public List<ParticleSystem> thrusters;
    public List<Transform> tyres;
    public List<Transform> tyreSuspensions;
    Rigidbody self;
    float tractionSpeed;
    Vector3 deltaPosition = Vector3.zero;
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
            //Debug.Log(tempvalue + " =1= " + SceneObjects.current.trackWayPoints.Count);
            wayPointBehindIndex = tempvalue;
            wayPointBehind = SceneObjects.current.trackWayPoints[tempvalue];
            tempvalue = indexIncrement(tempvalue, 1, SceneObjects.current.trackWayPoints.Count);
            //Debug.Log(tempvalue + " =2= " + SceneObjects.current.trackWayPoints.Count);
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

    float dragFactor;

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
        if(strength > 0.01f){
            //tractionSpeed += strength * thrust * 2 * acceleration;
            propulsion = Vector3.ProjectOnPlane(propulsion, transform.up);
            propulsion += strength * transform.forward * thrust * 2 * acceleration;
            if(thrust > 0.5f){
                StartThrusters();
            }else{
                StopThrusters();
            }
        }else{

            StopThrusters();
        }

        if(strength < -0.01f){
            if(tractionSpeed > 1f){
                //tractionSpeed *= (1 - brakeFactor * 0.01f);
                propulsion *= (1 - brakeFactor * 0.01f);
            }else{
                //tractionSpeed += strength * thrust  * (acceleration);
                propulsion += strength * transform.forward * thrust * (acceleration);
            }
        }
    }   


    public void AddUpwardsTorque(float strength){
        if(Mathf.Abs(strength) > 0.01f){
            self.AddTorque(transform.up * strength * turnStrength * 3);
            turning = true;
        }
    }

    float tempTraction = 1;
    float tempFriction = 0;
    public void hardBrake(){
        tempTraction = 0;
        //tractionSpeed *= (1 - brakeFactor * 0.005f);
        propulsion *= (1 - brakeFactor * 0.005f);
    }
    bool turning;
    float tractionAccountedAngleOffset = 0;

    float currentYangle;
    float prevYangle;

    void Start()
    {
        self = transform.GetComponent<Rigidbody>();
        wayPointBehindI = 0;
        tempFriction = MainController.current.kineticFriction;
        currentYangle = transform.localEulerAngles.y;
        prevYangle = transform.localEulerAngles.y;
    }
    // Update is called once per frame
    void FixedUpdate()
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


        Vector3 normalVec = -posVec.normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, posVec, out hit, 10)){
            if(hit.collider.tag == "Track"){
                if(hit.distance < 2){
                    normalVec = hit.normal;
                    posVec = -hit.normal *(hit.distance + 1f);
                }else{
                    self.velocity = Vector3.Lerp(self.velocity, Vector3.Project(self.velocity, posVec), 0.1f);
                    propulsion *= 0.95f;
                    posVec = -hit.normal *(hit.distance + 1f);
                }

                
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

        if(posVec.magnitude > 10){
            //tractionSpeed *= (1 - 5 * 0.01f);
            propulsion *= (1 - 5 * 0.01f);
            //self.velocity = Vector3.Lerp(self.velocity, Vector3.Project(self.velocity, posVec), 0.1f);
        }

        Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight / Mathf.Pow(posVec.magnitude, MainController.current.gravityExponent);
        
        self.AddForce(gravity*10f);

        Vector3 orientVec = Vector3.Cross(transform.up, normalVec);
        self.AddTorque(orientVec*orientStrength);

        //if(Time.frameCount % 10 == 0){

        //}
            
        //tractionSpeed *= MainController.current.airResistance;
        //propulsion *= MainController.current.airResistance;

        /*dragFactor = 1 - tractionSpeed * tractionSpeed * acceleration / (aerodynamic * 6000000.0f);
        //dragFactor = 1;
        if(dragFactor < 0) dragFactor = 0;
        tractionSpeed *= dragFactor;*/

        dragFactor = 1 - propulsion.magnitude * propulsion.magnitude * acceleration / (aerodynamic * 6000000.0f);
        if(dragFactor < 0) dragFactor = 0;
        propulsion *= dragFactor;

        //kinetic friction
        /*if(tractionSpeed > 1){
            tractionSpeed -= Mathf.Sign(tractionSpeed) * tempFriction * acceleration * 0.01f;
        }else{
            tractionSpeed *= 0.99f;
        }*/

        if(propulsion.magnitude > 1){
            propulsion -= propulsion.normalized * tempFriction * acceleration * 0.01f;
        }else{
            propulsion *= 0.99f;
        }




        
        //vector = Quaternion.AngleAxis(-45, Vector3.up) * vector;

        //deltaPosition = tractionSpeed * Vector3.Slerp(Quaternion.AngleAxis(tractionAccountedAngleOffset, transform.up) * transform.forward, transform.forward, traction * tempTraction) * (Time.fixedDeltaTime) * 0.1f;
        //Debug.Log(propulsion.magnitude + " " + deltaPosition);
        tempTraction = traction / 10f;
        if(tempTraction >= 0.09f){
            tempTraction = 1.0f;
        }
        propulsion = Vector3.Lerp(propulsion, transform.forward * propulsion.magnitude, tempTraction);

        if(tyres.Count > 0){
            foreach(Transform tyre in tyres){
                tyre.Rotate(new Vector3(propulsion.magnitude * 0.1f,0,0), Space.Self);
                /*if(tyre.localEulerAngles.x > 360){
                    tyre.localEulerAngles += new Vector3(-360, 0, 0);
                }*/
            }
        }

        UIController.current.speed = (int)(propulsion.magnitude);
        
        deltaPosition = propulsion * (Time.fixedDeltaTime) * 0.1f;
        if(Vector3.Dot(deltaPosition, posVec)>0){
            deltaPosition = Vector3.ProjectOnPlane(deltaPosition, posVec);
        }

        turning = false;
        tempTraction = 1;

        transform.position += deltaPosition;


    }
}
