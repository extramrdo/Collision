﻿using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DummyBot : Enemy {

	public Animator anim;
	public Transform left;
	public Transform right;
	public GameObject throwW;

	public float maxSpeed = 10f;
	public float waitTime = 0f;
	public float maxWaitTime = 2f;
	public float minWaitTime = 1f;
	public float cooldown = 5f;

	public bool canMove = true;
	public bool moving = true;
	public bool moveRight = true;

	public Stopwatch delay = new Stopwatch();
	private Vector3 dest;
	private float shootTime = 0f;

	private AudioSource killSound;

	// Use this for initialization
	void Start () {
		dest = transform.position;
		delay.Start ();
		killSound = gameObject.GetComponents<AudioSource> () [1];
	}


	void FixedUpdate() {
		
		//Destination condition
		if((moveRight && transform.position.x > dest.x) ||
		   (!moveRight && transform.position.x < dest.x))
		{
			//End attack animation and stop moving
			anim.SetBool("Attack", false);
			if(audio.isPlaying)
			{
				audio.Stop();
			}
			moving = false;

			//Reset waittime
			delay.Reset();
			delay.Start();
			waitTime = Random.Range(minWaitTime, maxWaitTime);

			//Set new randomized destination
			dest = new Vector3(Random.Range(left.position.x, right.position.x), dest.y);
			moveRight = dest.x > transform.position.x;
		}

		if(moving){
			if(!audio.isPlaying)
			{
				audio.Play();
			}

			//Check attack cooldown while moving only
			if(Time.time - shootTime > cooldown)
			{
				//End attack animation after cooldown
				anim.SetBool("Attack", false);

				if( isFacing("Aliver") && hasVision("Aliver", 70) ){
					
					//Shoot randomly
					if(Random.Range(0, 4) > 2)
					{
						Shoot ();
					}
				}
			}

			//Set velocity
			if(moveRight)
				rigidbody2D.velocity = new Vector2 (maxSpeed, rigidbody2D.velocity.y);
			else
				rigidbody2D.velocity = new Vector2 (-maxSpeed, rigidbody2D.velocity.y);
		}
		else if(canMove){

			//Wait condition
			if(delay.ElapsedMilliseconds > (waitTime * 1000)){
				moving = true;

				delay.Stop();
			}

		}
	
		//Neutralize velocity
		if(!moving)
		{
			rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);
		}


		//Set animation variables
		anim.SetBool("FacingRight", moveRight);
		anim.SetBool("Moving", moving);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		//Damage Aliver on collision and stop moving
		if(alive && col.tag == "Player")
		{
			AliverController aliver = col.gameObject.GetComponent<AliverController>();
			aliver.Freeze();
			moving = false;
			aliver.LoseLives(1);
			
			delay.Reset();
			delay.Start();
			
			waitTime = maxWaitTime;
			aliver.Unfreeze();
		}

		//Take damage from projectile
		else if(alive && col.tag == "Projectile")
		{
			killSound.Play();
			col.gameObject.GetComponent<Projectile>().collided = true;
			moving = false;
			canMove = false;
			delay.Stop();
			Hurt (1);
			anim.Play("Break");
		}

		else if(col.tag == "Shield")
		{
			moving = false;
			dest = transform.position;
			delay.Reset();
			delay.Start();
		}
	}


	/// <summary>
	/// Shoot while moving.
	/// </summary>
	private void Shoot(){

		float angle = 0;
		float dir = 0;

		shootTime = Time.time;
		anim.SetBool("Attack", true);

		Vector3 aim =  GameObject.Find("Aliver").transform.position - transform.position;
		aim.z = 0;
		aim.Normalize();
		angle = Vector3.Angle( new Vector3(1,0,0), aim);

		//Set arm rotation to match projectile
		if (moveRight)
		{
			aim = Vector3.right;
			dir = 1;
		}
		else
		{
			aim = Vector3.left;
			dir = -1;
		}
		
		aim.z = 0;
		aim.Normalize();
		
		GameObject proj = Instantiate (throwW, transform.position, Quaternion.Euler (new Vector3 (0, 0, angle))) as GameObject;
		
		
		proj.rigidbody2D.velocity = new Vector2(Mathf.Cos(angle * (Mathf.PI / 180)) * (180 / Mathf.PI), Mathf.Sin(angle * (Mathf.PI / 180)) * (180 / Mathf.PI) ).normalized * 24f;
		//proj.transform.Rotate(new Vector3(0, 0, 1), angle);
		//proj.rigidbody2D.velocity = new Vector2 (dir,0) * 24f;
	}

	bool hasVision(  string target, float range){
		
		GameObject targ = GameObject.Find(target);
		if( targ == null ){
			
			return false;
		}
		
		int mask = ~(1 << LayerMask.NameToLayer("Enemies"));
		//UnityEngine.Debug.DrawLine(transform.position, targ.transform.position - transform.position, range, mask);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, targ.transform.position - transform.position, range, mask);
		
		if( hit == null || hit.transform == null){
			
			return false;
		}

		if ( hit.transform.name.Equals(target) ){
			
			return true;
		}
		
		return false;
		
	}
	
	bool isFacing(string target){
		
		GameObject targ = GameObject.Find(target);
		if( targ == null ){
			
			return false;
		}
		
		float targetDir = targ.transform.position.x - transform.position.x;
		
		if( (moveRight && targetDir >= 0) || (!moveRight && targetDir <= 0)){
			
			return true;
		}
		
		return false;
	}

	/*void OnCollisionEnter2D(Collision2D col)
	{
		GameObject obj = col.gameObject;

		if(obj.tag == "Player")
		{
			obj.GetComponent<AliverController>().LoseLives(1);
			moving = false;
			
			delay.Reset();
			delay.Start();
			
			waitTime = maxWaitTime;
		}

		if(col.collider.tag != "Ground")
		{
			moving = false;
			
			delay.Reset();
			delay.Start();
			
			waitTime = Random.Range(minWaitTime, maxWaitTime);
		}
	}*/
}
