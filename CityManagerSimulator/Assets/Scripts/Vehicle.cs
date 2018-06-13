using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SensorsInfoStruct
{
    public RaycastHit frontCenterSensor;
    public RaycastHit frontLeftSensor;
    public RaycastHit frontLeftAngleSensor;
    public RaycastHit frontRightSensor;
    public RaycastHit frontRightAngleSensor;

    public bool frontCenterSensorFired;
    public bool frontLeftSensorFired;
    public bool frontLeftAngleSensorFired;
    public bool frontRightSensorFired;
    public bool frontRightAngleSensorFired;
}

[RequireComponent(typeof(Driver))]
public class Vehicle : MonoBehaviour
{
    private Rigidbody rb;
    private float speed;
    private float accelerator;
    private float brake;
    private float turnAngle;

    [Header("Sensors")]
    public float sensorLength = 10f;

    public Vector3 frontSensorPosition = new Vector3(0f, 0f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

    public SensorsInfoStruct SensorsInfo;

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

        // Ensure car is stationary
        speed = 0;
        accelerator = 0;
        brake = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ReadSensors();

        speed = rb.velocity.magnitude;
        Debug.Log(rb.velocity.magnitude * 2.237);  // 2.237 is from wolframalpha.com found on a unity webpage
    }

    private void ReadSensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + frontSensorPosition;

        SensorsInfo.frontCenterSensorFired = false;
        SensorsInfo.frontLeftAngleSensorFired = false;
        SensorsInfo.frontLeftSensorFired = false;
        SensorsInfo.frontRightSensorFired = false;
        SensorsInfo.frontRightAngleSensorFired = false;

        // Front center Sensor
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            // Update the sensor with new data
            SensorsInfo.frontCenterSensor = hit;
            SensorsInfo.frontCenterSensorFired = true;
            // Fire an event
        }

        // Front right sensor
        sensorStartPos.x += frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            // Update the sensor with new data
            SensorsInfo.frontRightSensor = hit;
            SensorsInfo.frontRightSensorFired = true;
            // Fire an event
        }

        // Front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            // Update the sensor with new data
            SensorsInfo.frontRightAngleSensor = hit;
            SensorsInfo.frontRightAngleSensorFired = true;

            // Fire an event
        }

        // Front left sensor
        sensorStartPos.x -= 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            // Update the sensor with new data
            SensorsInfo.frontLeftSensor = hit;
            SensorsInfo.frontLeftSensorFired = true;
            // Fire an event
        }

        // Front left angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            // Update the sensor with new data
            SensorsInfo.frontLeftAngleSensor = hit;
            SensorsInfo.frontLeftAngleSensorFired = true;

            // Fire an event
        }
    }
}