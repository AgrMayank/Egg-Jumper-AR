using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGenerate : MonoBehaviour {

	public float minWait = 5;
	public float maxWait = 6;
	public GameObject spawn;
	private bool isSpawning;

	void Awake ()
	{
		isSpawning = false;
	}

	void Update ()
	{
		if (!isSpawning)
		{
			float timer = Random.Range (minWait, maxWait);
			Invoke ("SpawnObject", timer);
			isSpawning = true;
		}
	}

	void SpawnObject ()
	{
		Instantiate (spawn);
		isSpawning = false;
	}
}
