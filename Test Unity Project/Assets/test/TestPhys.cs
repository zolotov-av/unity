using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhys : MonoBehaviour {

	private Rigidbody rb;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
	{
		if ( Input.GetKeyDown("w") )
		{
			rb.AddForce(1f, 0f, 0f, ForceMode.Impulse);
			//rb.AddForce(1f, 0f, 0f, ForceMode.VelocityChange);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
