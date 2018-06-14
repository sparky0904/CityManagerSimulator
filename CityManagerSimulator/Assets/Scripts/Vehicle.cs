using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public enum theDriveType
{
    RearWheelDrive,
    FrontWheelDrive,
    AllWheelDrive
}

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

public class Vehicle : MonoBehaviour
{
    [Tooltip("Maximum steering angle of the wheels")]
    public float maxAngle = 30f;

    [Tooltip("Maximum torque applied to the driving wheels")]
    public float maxTorque = 300f;

    [Tooltip("Maximum brake torque applied to the driving wheels")]
    public float brakeTorque = 30000f;

    [Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
    public GameObject wheelShape;

    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
    public float criticalSpeed = 5f;

    [Tooltip("Simulation sub-steps when the speed is above critical.")]
    public int stepsBelow = 5;

    [Tooltip("Simulation sub-steps when the speed is below critical.")]
    public int stepsAbove = 1;

    [Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
    public theDriveType driveType;

    private WheelCollider[] m_Wheels;
    private Vehicle vehicle;

    private Rigidbody rb;
    private float speed;
    private int speedMPH;
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

    public int SpeedMPH
    {
        get
        {
            return speedMPH;
        }

        set
        {
            speedMPH = value;
        }
    }

    [Header("Vehicle Specs")]
    public int MaxSpeedMPH;

    // Use this for initialization
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find all the WheelColliders down in the hierarchy.
        vehicle = GetComponent<Vehicle>();
        m_Wheels = GetComponentsInChildren<WheelCollider>();

        for (int i = 0; i < m_Wheels.Length; ++i)
        {
            var wheel = m_Wheels[i];

            // Create wheel shapes only when needed.
            if (wheelShape != null)
            {
                var ws = Instantiate(wheelShape);
                ws.transform.parent = wheel.transform;
            }
        }

        // Ensure car is stationary
        speed = 0;
        accelerator = 0;
        brake = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ReadSensors();
        Drive();

        speed = rb.velocity.magnitude;
        speedMPH = (int)(rb.velocity.magnitude * 2.23694); // Figure is in m/s so to convert to mph we multiply by 2.23694, MPH per M/s
        Debug.Log(speedMPH);
    }

    private void Drive()
    {
        // This is a really simple approach to updating wheels.
        // We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
        // This helps us to figure our which wheels are front ones and which are rear.

        m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

        // float angle = maxAngle * Input.GetAxis("Horizontal");
        // float torque = maxTorque * Input.GetAxis("Vertical");
        float angle = maxAngle * turnAngle;

        // Check if speed is above requested speed, if so set Torque to 0...we dont need to go any faster!
        float torque = maxTorque * accelerator;

        // float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;
        float handBrake = vehicle.Brake * brakeTorque;

        foreach (WheelCollider wheel in m_Wheels)
        {
            // A simple car where front wheels steer while rear ones drive.
            if (wheel.transform.localPosition.z > 0)
                wheel.steerAngle = angle;

            if (wheel.transform.localPosition.z < 0)
            {
                wheel.brakeTorque = handBrake;
            }
            wheel.brakeTorque = handBrake;

            if (wheel.transform.localPosition.z < 0 && driveType != theDriveType.FrontWheelDrive)
            {
                wheel.motorTorque = torque;
            }

            if (wheel.transform.localPosition.z >= 0 && driveType != theDriveType.RearWheelDrive)
            {
                wheel.motorTorque = torque;
            }

            // Update visual wheels if any.
            if (wheelShape)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                // Assume that the only child of the wheelcollider is the wheel shape.
                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }
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