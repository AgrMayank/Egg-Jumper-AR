using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotate : MonoBehaviour {

	public float jumpSpeed = 500f;
	public bool grounded = false;
	public bool onBasket = false;

	public Quaternion originalRotationValue; // declare this as a Quaternion
	float rotationResetSpeed = 1.0f;

	// Use this for initialization
	void Start ()
	{
		originalRotationValue = transform.rotation; // save the initial rotation
	}

	// Update is called once per frame
	void Update ()
	{
		if (grounded)
		{
			if (Input.GetMouseButtonDown (0))
			{
				GetComponent<Rigidbody> ().AddForce (Vector2.up * jumpSpeed);
				grounded = false;
				onBasket = false;
			}
			
			transform.position = new Vector3 (FindClosestEnemy ().transform.position.x, FindClosestEnemy ().transform.position.y + 0.335f , 0);
			transform.rotation = Quaternion.Slerp (transform.rotation, originalRotationValue, Time.time * rotationResetSpeed);
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.transform.tag == "Basket")
		{
			grounded = true;
			onBasket = true;
		}
	}

	// Find the closest basket if collided
	public GameObject FindClosestEnemy ()
	{
		GameObject [] gos;
		gos = GameObject.FindGameObjectsWithTag ("Basket");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos)
		{
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

}
