using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {

    Vehicle vehicle;

	// Use this for initialization
	void Start () {
        vehicle = GetComponent<Vehicle>();
	}
	
	// Update is called once per frame
	void Update () {
        vehicle.Accelerator = Input.GetAxis("Vertical");
        vehicle.TurnAngle = Input.GetAxis("Horizontal");
	}
}
