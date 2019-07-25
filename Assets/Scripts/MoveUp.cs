using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : MonoBehaviour {

	public Quaternion originalRotationValue; // declare this as a Quaternion
	float rotationResetSpeed = 1.0f;

	// Use this for initialization
	void Start () {
		originalRotationValue = transform.rotation; // save the initial rotation

	}

	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.tag == "Respawn")
		{
			transform.position += new Vector3 (0, 8, 0);
			transform.Translate (0, 4.4f, 0);
			transform.rotation = Quaternion.Slerp (transform.rotation, originalRotationValue, Time.time * rotationResetSpeed);
		}
	}
}
