using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Driver))]
public class Vehicle : MonoBehaviour
{
    private Rigidbody rb;
    private float speed;    
    private float accelerator;
    private float brake;
    private float turnAngle;

    public float Accelerator
    {
        get { return accelerator; }
        set { accelerator = Mathf.Clamp(value, -1, 1); } // A negative indicates reversing
    }

    public float TurnAngle
    {
        get { return turnAngle; }
        set { turnAngle = value; }
    }

    public float Brake
    {
        get { return brake; }
        set { brake = Mathf.Clamp01(value); } // Brake can only be between 0 and 1
    }

    // Use this for initialization
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure care is stationary
        speed = 0;
        accelerator = 0;
        brake = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        speed = rb.velocity.magnitude;
        Debug.Log(rb.velocity.magnitude * 2.237);  // 2.237 is from wolframalpha.com found on a unity webpage
    }

}