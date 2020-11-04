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

    bool speedDecay = true;

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

        if (other.tag == "NodeLink")
        {
            nodeColliders.Remove(other.transform);
            trackFront = calculateTrackFront();
        }


        if (other.tag == "NoDecayZone"){
            speedDecay = true;
        }

        if(other.tag == "WayPoint"){
            if(driver != null){
                bool newPoint = true;
                for(int i = 0; i< driver.wayPointsPassed.Count; i++)
                {
                    if(driver.wayPointsPassed[i] == other.transform)
                    {
                        newPoint = false;
                    }
                }
                if (newPoint)
                {
                    driver.wayPointsPassed.Add(other.transform);
                }
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

        if (nodeColliders.Count == 0)
        {
            if (other.tag == "NodeLink")
            {
                nodeColliders.Add(other.transform);
                trackFront = calculateTrackFront();
                latestNode = int.Parse(other.name);
            }
        }



        if (other.tag == "NoDecayZone"){
            speedDecay = false;
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
                        driver.SetFuelTurboUI();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "NodeLink")
        {
            nodeColliders.Add(other.transform);
            trackFront = calculateTrackFront();
            latestNode = int.Parse(other.name);
        }
    }

    List<Transform> nodeColliders = new List<Transform>();

    int latestNode = 0;

    Vector3 trackFront;
    Vector3 calculateTrackFront()
    {
        int n = nodeColliders.Count;
        if(n > 0)
        {
            Vector3 result = Vector3.zero;
            for(int i = 0; i < n; i++)
            {
                result += nodeColliders[i].forward;
            }

            return result.normalized;
        }
        else
        {
            return transform.forward;
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

    public int trackCovered = 0;


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
            driver.SetFuelTurboUI();
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


        if (SceneObjects.current.nodesExist)
        {
            /*int n = SceneObjects.current.nodeCount;
            float minDist = float.MaxValue;
            for(int i = 0; i < n; i++)
            {
                int ii = i + 1;
                if (ii == n)
                {
                    ii = 0;
                }
                Vector3 myPosToNode = transform.position - SceneObjects.current.trackNodes[i];
                Vector3 currentConnection = SceneObjects.current.trackConnections[i];
                Vector3 myPosToNextNode = transform.position - SceneObjects.current.trackNodes[ii];
                if (Vector3.Dot(myPosToNode, currentConnection) >= -50 && Vector3.Dot(myPosToNextNode, -currentConnection) >= -50)
                {
                    float dist = Vector3.Cross(myPosToNode, currentConnection).sqrMagnitude / (currentConnection.sqrMagnitude * 1.0f);
                    Debug.Log("Testing node: " + i.ToString() + " distance = " + dist.ToString());
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nodeBehind = i;
                    }
                }
            }
            SceneObjects.current.nodeMarkers[0].position = SceneObjects.current.trackNodes[nodeBehind];
            SceneObjects.current.nodeMarkers[1].position = SceneObjects.current.trackNodes[nodeAhead];*/
            trackFront = calculateTrackFront();

        }
        
    }

    int nodeBehind = 0;

    int nodeAhead
    {
        get
        {
            if(latestNode >= SceneObjects.current.nodeCount - 1)
            {
                return 0;
            }
            else
            {
                return latestNode + 1;
            }
        }
    }

    private void Update()
    {
        if (SceneObjects.current.nodeCount > 0)
        {
            if (Time.frameCount % 50 == 0)
                trackCovered = (int)(SceneObjects.current.nodePercentages[latestNode] + ((transform.position - SceneObjects.current.trackNodes[latestNode]).magnitude * (SceneObjects.current.nodePercentages[nodeAhead] - SceneObjects.current.nodePercentages[latestNode])) / SceneObjects.current.trackConnections[latestNode].magnitude);
        }
    }

    float speedTerm = 0;
    void FixedUpdate()
    {
        if(RaceManager.current.allowFuel){
            if(thrustersOn && fuel > 0){
                fuel -= 0.003f;
                if(fuel < 0){
                    fuel = 0;
                }


                if (driver != null)
                {
                    driver.SetFuelTurboUI();
                }
            }
            if(fuel <= 0  && tempAerodynamic > 10){
                tempAerodynamic = 10;
            }
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 10, LayerMask.GetMask("Track"))){
            if(hit.collider.tag == "Track"){
                RaycastHit hit2;
                if(Physics.Raycast(transform.position, -hit.normal, out hit2, 10, LayerMask.GetMask("Track"))){
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
                if(Physics.Raycast(transform.position, Vector3.SlerpUnclamped(-transform.up, transform.right, i/90.0f), out hit3, 10, LayerMask.GetMask("Track"))){
                    if(hit3.collider.tag == "Track"){
                        closestPointOnTrack = hit3.point;
                        normalVec = hit3.normal;
                        found = true;
                        break;
                    }
                }
                if(Physics.Raycast(transform.position, Vector3.SlerpUnclamped(-transform.up, -transform.right, i/90.0f), out hit3, 10, LayerMask.GetMask("Track"))){
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
                    if (Physics.Raycast(transform.position, transform.forward - transform.up, out hit3, 20, LayerMask.GetMask("Track")))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                    if (Physics.Raycast(transform.position, -transform.forward - transform.up, out hit3, 20, LayerMask.GetMask("Track")))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                    if (Physics.Raycast(transform.position, transform.forward, out hit3, 20, LayerMask.GetMask("Track")))
                    {
                        if (hit3.collider.tag == "Track")
                        {
                            closestPointOnTrack = hit3.point;
                            normalVec = hit3.normal;
                            found = true;
                            break;
                        }
                    }
                    if (Physics.Raycast(transform.position, -transform.forward, out hit3, 20, LayerMask.GetMask("Track")))
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
        }
        /*else
        if(posVec.sqrMagnitude < 4){
            thrust = 0.6f;
        }*/
        else
        {
            thrust = 1 / (1f * posVec.sqrMagnitude);
        }

        if(turning){
            thrust *= 1 - MainController.current.turningThrustLoss;
        }

        if(speedDecay){
        if(posVec.sqrMagnitude > 9){

            propulsion *= 0.95f;
        }}


        /*if(posVec.sqrMagnitude > 10){
            propulsion *= (1 - 5 * 0.01f);
        }*/

        Vector3 gravity = posVec.normalized * MainController.current.gravityConstant * MainController.current.averageCarWeight;
        self.AddForce(gravity*15f);

        Vector3 orientVec = Vector3.zero;

        //if (posVec.sqrMagnitude < 64)
        {
            orientVec = Vector3.Cross(transform.up, normalVec);
        }
       /* else
        {
            orientVec = Vector3.Cross(transform.up, -posVec);
        }*/


        self.AddTorque(orientVec*orientStrength*2);
        //transform.Rotate(orientVec, orientStrength);

        //Align towards track forward
        if (SceneObjects.current.nodesExist)
        {
            /* Vector3 myPosToNode = transform.position - SceneObjects.current.trackNodes[nodeBehind];
              Vector3 currentConnection = SceneObjects.current.trackConnections[nodeBehind];
              Vector3 myPosToNextNode = transform.position - SceneObjects.current.trackNodes[nodeAhead];*/
            /* int resetPoint = nodeBehind;
             //Debug.Log(" Frame change");
             //Debug.Log(" Testing node " + nodeBehind.ToString() + " dot1 = " + Vector3.Dot(myPosToNode, currentConnection).ToString() + " dot2 = " + Vector3.Dot(myPosToNextNode, -currentConnection).ToString());
             while (!(Vector3.Dot(myPosToNode, currentConnection) >= -20 && Vector3.Dot(myPosToNextNode, -currentConnection) >= -20))
             {
                 nodeBehind += 1;

                 if(nodeBehind == SceneObjects.current.nodeCount)
                 {
                     nodeBehind = 0;
                 }
                 if (nodeBehind == resetPoint)
                 {
                     break;
                 }

                 myPosToNode = transform.position - SceneObjects.current.trackNodes[nodeBehind];
                 currentConnection = SceneObjects.current.trackConnections[nodeBehind];
                 myPosToNextNode = transform.position - SceneObjects.current.trackNodes[nodeAhead];

                 //Debug.Log(" Testing node " + nodeBehind.ToString() + " dot1 = " + Vector3.Dot(myPosToNode, currentConnection).ToString() + " dot2 = " + Vector3.Dot(myPosToNextNode, -currentConnection).ToString());
             }*/

            /*int n = SceneObjects.current.nodeCount;
            float minDist = float.MaxValue;
            for (int i = 0; i < n; i++)
            {
                int ii = i + 1;
                if (ii == n)
                {
                    ii = 0;
                }
                 myPosToNode = transform.position - SceneObjects.current.trackNodes[i];
                 currentConnection = SceneObjects.current.trackConnections[i];
                 myPosToNextNode = transform.position - SceneObjects.current.trackNodes[ii];
                if (Vector3.Dot(myPosToNode, currentConnection) >= -50 && Vector3.Dot(myPosToNextNode, -currentConnection) >= -50)
                {
                    float dist = Vector3.Cross(myPosToNode, currentConnection).sqrMagnitude / (currentConnection.sqrMagnitude * 1.0f);
                    //Debug.Log("Testing node: " + i.ToString() + " distance = " + dist.ToString());
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nodeBehind = i;
                    }
                }
            }
            SceneObjects.current.nodeMarkers[0].position = SceneObjects.current.trackNodes[nodeBehind];
            SceneObjects.current.nodeMarkers[1].position = SceneObjects.current.trackNodes[nodeAhead];
            

            //Debug.Log(currentConnection.magnitude);
            Debug.DrawRay(SceneObjects.current.trackNodes[nodeBehind], myPosToNode, Color.yellow);
            Debug.DrawRay(SceneObjects.current.trackNodes[nodeAhead], myPosToNextNode, Color.green);
            Debug.DrawRay(SceneObjects.current.trackNodes[nodeBehind] + Vector3.up * 10, currentConnection, Color.red);


            SceneObjects.current.nodeMarkers[0].position = SceneObjects.current.trackNodes[nodeBehind];
            SceneObjects.current.nodeMarkers[1].position = SceneObjects.current.trackNodes[nodeAhead];*/
            if (!turning)
            {
                orientVec = Vector3.Cross(transform.forward, trackFront);
                self.AddTorque(orientVec * orientStrength * 0.04f);
            }
        }

    


        if (!inPit && fuel > 0 && RaceManager.current.allowFuel){
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
                driver.SetFuelTurboUI();
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
                    driver.SetSpeedUI();
                }
        
        deltaPosition = propulsion * (Time.fixedDeltaTime) * 0.1f;
        if(Vector3.Dot(deltaPosition, posVec)>0){
            deltaPosition = Vector3.ProjectOnPlane(deltaPosition, posVec);
        }
        
        {
            RaycastHit Hit;
            if(Physics.Raycast(transform.position, deltaPosition, out Hit, deltaPosition.magnitude, LayerMask.GetMask("Track"))){
                deltaPosition = deltaPosition.normalized * (hit.distance * 0.4f);
            }
        }


        turning = false;
        traction = Traction;
        

        transform.position += deltaPosition;


    }
}
