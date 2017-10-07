//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class TestAnim1 : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "Player" )
        {
            obj.GetComponent<Animator>().SetInteger("state", 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            obj.GetComponent<Animator>().SetInteger("state", 0);
        }
    }
}
