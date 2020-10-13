using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarPhysics : MonoBehaviour
{
    //public event Action OnExitFlag;
    public Driver driver;
    void OnTriggerExit(Collider other){
        if(other.tag == "ChequeredFlag"){
           // Debug.Log("flag collider detected");            
            /*if(OnExitFlag != null){
                OnExitFlag();
            }*/
            if(driver != null){
                //print("Car: My driver is still here");
                if(driver.active){
                  CarController.current.onPassFlag();
                }
            }
        }

        if(other.tag == "WayPoint"){
           // Debug.Log("flag collider detected");            
            /*if(OnExitFlag != null){
                OnExitFlag();
            }*/
            if(driver != null){
                //print("Car: My driver is still here");
                if(driver.active){
                  CarController.current.wayPointsPassed.Add(other.transform);
                }
            }
        }


    }

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
    public float Traction = 0.5f;

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
    float traction;
    float dragFactor;
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
                propulsion *= (1 - brakeFactor * 0.01f);
            }else{
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

    public void stopAllMovement(){
        self.velocity = Vector3.zero;
        propulsion = Vector3.zero;
        self.angularVelocity = Vector3.zero;
    }
    float tempTraction = 1;
    float tempFriction = 0;
    public void hardBrake(){
        traction = Traction/3.0f;
        propulsion *= (1 - brakeFactor * 0.005f);
    }
    bool turning;
    Vector3 posVec;
    Vector3 closestPointOnTrack;
    Vector3 normalVec;
    void Start()
    {
        traction = Traction;
        self = transform.GetComponent<Rigidbody>();
        tempFriction = MainController.current.kineticFriction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10)){
            if(hit.collider.tag == "Track"){
                RaycastHit hit2;
                if(Physics.Raycast(transform.position, -hit.normal, out hit2, 10)){
                    closestPointOnTrack = hit2.point;
                    normalVec = hit.normal;
                }else{
                    closestPointOnTrack = hit.point;
                    normalVec = hit.normal;
                }
            }
        }else{
            print("Track/Grid position not found, disabling car to avoid errors");
            this.enabled = false;
        }
        
    }
    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 10)){
            if(hit.collider.tag == "Track"){
                RaycastHit hit2;
                if(Physics.Raycast(transform.position, -hit.normal, out hit2, 10)){
                    if(hit2.collider.tag == "Track"){
                        closestPointOnTrack = hit2.point;
                        normalVec = hit.normal;
                    }
                }else{
                    closestPointOnTrack = hit.point;
                    normalVec = hit.normal;
                }
            }
        }else{
            bool found = false;
            for(int i = 5; i <=180; i =i +5){
                RaycastHit hit3;              
                if(Physics.Raycast(transform.position, Vector3.SlerpUnclamped(-transform.up, transform.right, i/90.0f), out hit3, 10)){
                    if(hit3.collider.tag == "Track"){
                        closestPointOnTrack = hit3.point;
                        normalVec = hit3.normal;
                        found = true;
                        break;
                    }
                }
                if(Physics.Raycast(transform.position, Vector3.SlerpUnclamped(-transform.up, -transform.right, i/90.0f), out hit3, 10)){
                    if(hit3.collider.tag == "Track"){
                        closestPointOnTrack = hit3.point;
                        normalVec = hit3.normal;
                        found = true;
                        break;
                    }
                }
            }
            if(!found){
                normalVec = Vector3.ProjectOnPlane((transform.position - closestPointOnTrack), transform.forward).normalized;
            }
        }
        posVec = closestPointOnTrack - transform.position;

        if(posVec.sqrMagnitude < 1){
            thrust = 1;
        }else{
            thrust = 1 / (1f * posVec.sqrMagnitude);
        }

        if(turning){
            thrust *= 1 - MainController.current.turningThrustLoss;
        }

        if(posVec.sqrMagnitude > 4){

            propulsion *= 0.95f;
        }

        if(posVec.magnitude > 10){
            propulsion *= (1 - 5 * 0.01f);
        }

        Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight;

        self.AddForce(gravity*10f);

        Vector3 orientVec = Vector3.Cross(transform.up, normalVec);
        self.AddTorque(orientVec*orientStrength);

        dragFactor = 1 - propulsion.magnitude * propulsion.magnitude * acceleration / (aerodynamic * 6000000.0f);
        if(dragFactor < 0) dragFactor = 0;
        propulsion *= dragFactor;

        if(propulsion.magnitude > 1){
            propulsion -= propulsion.normalized * tempFriction * acceleration * 0.01f;
        }else{
            propulsion *= 0.99f;
        }

        tempTraction = traction / 10f;
        if(tempTraction >= 0.09f){
            tempTraction = 1.0f;
        }
        propulsion = Vector3.Lerp(propulsion, transform.forward * propulsion.magnitude, tempTraction);

        if(tyres.Count > 0){
            foreach(Transform tyre in tyres){
                tyre.Rotate(new Vector3(propulsion.magnitude * 0.1f,0,0), Space.Self);
            }
        }

        UIController.current.speed = (int)(propulsion.magnitude);
        
        deltaPosition = propulsion * (Time.fixedDeltaTime) * 0.1f;
        if(Vector3.Dot(deltaPosition, posVec)>0){
            deltaPosition = Vector3.ProjectOnPlane(deltaPosition, posVec);
        }

        turning = false;
        traction = Traction;

        transform.position += deltaPosition;


    }
}
