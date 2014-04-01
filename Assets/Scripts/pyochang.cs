﻿using UnityEngine;
using System.Collections;


public class pyochang : MonoBehaviour {

	public float liveTime = 5;
	public float speed = 5000f;
	void Awake()
	{
		Destroy (gameObject, liveTime);
	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Enemy")
		{
			// ... find the Enemy script and call the Hurt function.
			col.gameObject.GetComponent<Enemy>().Hurt();
			// Destroy the rocket.
			Destroy (gameObject);
		}
		
		// Otherwise if the player manages to shoot himself...
		else if(col.gameObject.tag != "Player" && col.gameObject.tag != "weapon")
		{
		Destroy (gameObject);
		}

	}

	void Update(){
		rigidbody2D.AddTorque(Random.Range(-10,10));
	}
}
