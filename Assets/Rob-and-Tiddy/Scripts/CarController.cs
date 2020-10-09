using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    [Range(0, 20f)]
    private float accConstant = 0.1f;

    [SerializeField]
    [Range(0, 10f)]
    private float turnStrength = 4;

    public CarPhysics carPhysics;
    //float velocityX =0,velocityY =0;
    void Start()
    {
        
        carPhysics = transform.GetComponent<CarPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        //carPhysics.AddForce(transform.forward * accConstant * inputY);
        carPhysics.AccelerateForward(accConstant * inputY);
        carPhysics.AddUpwardsTorque(turnStrength * inputX);

        //velocityY += inputY * speed * Time.deltaTime;
        //print(velocityY);
        //carPhysics.AddForce(velocityY);
        // transform.Rotate(Vector3.up * inputX * Time.deltaTime * 10.0f);
        // transform.Translate(Vector3.forward * velocityY * Time.deltaTime);
        // velocityY -= velocityY * .2f * Time.deltaTime;
    }
}

