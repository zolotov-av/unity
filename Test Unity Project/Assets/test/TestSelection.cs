using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSelection : MonoBehaviour {
	
	public GameObject obj;

	// Use this for initialization
	void Start () {
		Renderer r = obj.GetComponent<Renderer>();
		Bounds bounds = r.bounds;
		transform.position = bounds.center;
		transform.localScale = bounds.size + new Vector3(0.001f, 0.001f, 0.001f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
