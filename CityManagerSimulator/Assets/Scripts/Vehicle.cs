using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private Rigidbody rb;
    private float Speed;

    public float Accelerator = 0;

    // Use this for initialization
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Speed = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        // zrb.velocity = Speed;
    }
}