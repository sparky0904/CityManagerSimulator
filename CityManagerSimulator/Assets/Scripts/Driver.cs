﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(myWheelDrive))]
public class Driver : MonoBehaviour
{
    private Vehicle vehicle;
    private myWheelDrive wheelDrive;

    private float newBrakeForceAmount;
    private float newAcceleratorForceAmount;
    private float newTurnAngleAmount;

    // Use this for initialization
    private void Start()
    {
        vehicle = GetComponent<Vehicle>();
        wheelDrive = GetComponent<myWheelDrive>();
    }

    // Update is called once per frame
    private void Update()
    {
        vehicle.Accelerator = Input.GetAxis("Vertical");
        vehicle.TurnAngle = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        CheckSensors();
        Drive();
    }

    private void CheckSensors()
    {
        SensorsInfoStruct sensorData = vehicle.SensorsInfo;

        // check if something is in front of us and depending on the distance apply the brakes
        if (sensorData.frontCenterSensorFired)
        {
            newBrakeForceAmount = 1; // Just slow down ASAP
            newAcceleratorForceAmount = 0;
        }
        else
        {
            newAcceleratorForceAmount = 1;
            newBrakeForceAmount = 0;
        }
    }

    private void Drive()
    {
        vehicle.Accelerator = newAcceleratorForceAmount;
        vehicle.Brake = newBrakeForceAmount;
    }
}