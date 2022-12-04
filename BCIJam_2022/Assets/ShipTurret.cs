using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTurret : MonoBehaviour {
	public PlayerShip parentShip;
	public Animator animator;

	public void GunFiredSuccessfully() {
		parentShip.chargingCannon = false;
		parentShip.CannonFiredSuccessfully();
		animator.SetBool("AnimLocked", true);
	}

	public void GunFinishedFiring() {
		animator.SetBool("AnimLocked", false);
	}
}
