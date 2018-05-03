using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var dir = getPlayerInput();
		GetComponent<Rigidbody>().velocity = dir * speed;
	}

	Vector3 getPlayerInput() {
		Vector3 input = new Vector3
        {
            x = Input.GetAxis("Horizontal"),
			y = 0,
            z = Input.GetAxis("Vertical")
        };
        return input;
	}
}
