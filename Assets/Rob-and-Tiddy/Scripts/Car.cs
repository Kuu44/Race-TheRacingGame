using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    [Range(10, 100f)]
    private float speed = 50f;

    [SerializeField]
    [Range(10, 100f)]
    private float acceleration = 50f;

    [SerializeField]
    [Range(0, 10f)]
    private float turnStrength = 5;

    [Range(0f, 1f)]
    public float Traction = 0.5f;

    [Range(1f, 8f)]
    public float orientStrength = 3;

    [Range(1, 10)]
    public float brakeFactor = 5;

    public List<ParticleSystem> thrusters;
    public List<Transform> tyres;

    public void setPropertiesToCar()
    {
        CarPhysics p = transform.parent.GetComponent<CarPhysics>();
        p.aerodynamic = speed;
        p.acceleration = acceleration;
        p.turnStrength = turnStrength;
        p.Traction = Traction;
        p.orientStrength = orientStrength;
        p.brakeFactor = brakeFactor;
        p.thrusters = thrusters;
        p.tyres = tyres;

        p.setTempAerodynamic();
    }

}
