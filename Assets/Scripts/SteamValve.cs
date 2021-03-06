﻿using UnityEngine;
using System.Collections;

public class SteamValve : PowerSwitch {

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Projectile"){
			if(!anim.GetBool("Power")){
				anim.SetBool ("Power", true);
				Destroy(transform.parent.FindChild ("Steam").gameObject);
				audio.Play();
			}
		}
	}

	void Update()
	{
		if(activated)
		{
			anim.SetBool ("Power", true);
			Destroy(transform.parent.FindChild ("Steam").gameObject);
			FindObjectOfType<AliverController>().Unfreeze();
			activated = false;
		}
	}
}
