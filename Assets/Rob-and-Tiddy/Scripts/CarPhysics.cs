using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



public class CarPhysics : MonoBehaviour
{
    //public event Action OnExitFlag;
    [HideInInspector]
    [Range(0,100)]
    public float fuel = 50;
    [Range(0,100)]
    public float turbo = 100;
    [HideInInspector]
    public Driver driver;

    public int speed
    {
        get
        {
            return (int)propulsion.magnitude;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.tag == "ChequeredFlag"){

            if(driver != null){
                driver.TargetOnPassFlag();
            }
        }

        if(other.tag == "WayPoint"){
            if(driver != null){
                driver.wayPointsPassed.Add(other.transform);                
            }
        }
        if(other.tag == "PitLane"){
            tempAerodynamic = aerodynamic;
            inPit = false;
        }
    }

    void OnTriggerStay(Collider other){
        if(other.tag == "PitLane"){
            tempAerodynamic = 10;
            inPit = true;
        }

        if(RaceManager.current.allowFuel){
            if(propulsion.sqrMagnitude < 16){
                if(other.tag == "Pit"){
                    if(fuel < 100){
                        fuel += 0.5f;
                        if(fuel > 100){
                            fuel = 100;
                        }
                    }
                    if(turbo < 100){
                        turbo += 2f;
                        if(turbo > 100){
                            turbo = 100;
                        }
                    }


                    if (driver != null)
                    {
                        driver.TargetSetUI();
                    }
                }
            }
        }
    }


    public void setTempAerodynamic()
    {
        if(tempAerodynamic != 10)
        {
            tempAerodynamic = aerodynamic;
        }
    }

    bool thrustersOn;

    
    public bool ThrustersOn
    {
        get
        {
            return thrustersOn;
        }
        set
        {
            thrustersOn = value;
            if (thrustersOn)
            {
                if (driver != null)
                    driver.CmdStartThrusters();
            }
            else
            {
                if (driver != null)
                    driver.CmdStopThrusters();
            }
        }
    }



    public bool inPit;
    public bool mainCar;

    [HideInInspector]
    public float aerodynamic = 50f;

    [HideInInspector]
    public float acceleration = 50f;

    [HideInInspector]
    public float turnStrength = 5;

    [HideInInspector]
    public float Traction = 0.5f;

    [HideInInspector]
    public float orientStrength = 3;

    [HideInInspector]
    public float brakeFactor = 5;

    float tempAerodynamic;
    float thrust = 1;
    [HideInInspector]
    public List<ParticleSystem> thrusters;
    [HideInInspector]
    public List<Transform> tyres;
    [HideInInspector]
    public List<Transform> tyreSuspensions;
    Rigidbody self;
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
                ThrustersOn = true;
            }else{
                ThrustersOn = false;
            }
        }else{

            ThrustersOn = false;
        }

        if(strength < -0.01f){
            if(propulsion.sqrMagnitude > 1f){
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
        thrustersOn = false;
    }

    public void useTurbo(){
        if(!turboOn){
            turboOn = true;
        }

    }

    public void stopTurbo(){
        turboOn = false;

    }

    bool turboOn = false;
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

        if (driver != null)
        {
            driver.TargetSetUI();
        }


        tempAerodynamic = aerodynamic;
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

    float speedTerm = 0;
    void FixedUpdate()
    {
        if(RaceManager.current.allowFuel){
            if(thrustersOn && fuel > 0){
                fuel -= 0.005f;
                if(fuel < 0){
                    fuel = 0;
                }


                if (driver != null)
                {
                    driver.TargetSetUI();
                }
            }
            if(fuel <= 0  && tempAerodynamic > 10){
                tempAerodynamic = 10;
            }
        }

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
            if (!found)
            {
                for (int i = 0; i < 1; i++)
                {
                    RaycastHit hit3;
                    if (Physics.Raycast(transform.position, transform.forward - transform.up, out hit3, 20))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                    if (Physics.Raycast(transform.position, -transform.forward - transform.up, out hit3, 20))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                    if (Physics.Raycast(transform.position, transform.forward, out hit3, 20))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                    if (Physics.Raycast(transform.position, -transform.forward, out hit3, 20))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found){

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

        if(!inPit && fuel > 0 && RaceManager.current.allowFuel){
            speedTerm = 20f - fuel * 0.4f;
        }

        //slipstream
        RaycastHit slipStreamRay;
        if(Physics.SphereCast(transform.position, 1, transform.forward, out slipStreamRay, 40, LayerMask.GetMask("Car"))){
            speedTerm += 40 - hit.distance;
        }





        if(turboOn && turbo > 0){
            speedTerm += 50;

            turbo -= 0.2f;
            if(turbo < 0){
                turbo = 0;
                
            }

            if (driver != null)
            {
                driver.TargetSetUI();
            }
        }
        dragFactor = 1 - propulsion.magnitude * acceleration / (((tempAerodynamic + speedTerm) * 35000.0f));
        if(dragFactor < 0) dragFactor = 0;
        propulsion *= dragFactor;
        speedTerm = 0;

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
        if(driver != null)
        if(driver.isLocalPlayer)
        if(driver != null)
                {
                    driver.TargetSetSpeedUI();
                }
        
        deltaPosition = propulsion * (Time.fixedDeltaTime) * 0.1f;
        if(Vector3.Dot(deltaPosition, posVec)>0){
            deltaPosition = Vector3.ProjectOnPlane(deltaPosition, posVec);
        }

        turning = false;
        traction = Traction;
        

        transform.position += deltaPosition;


    }
}
