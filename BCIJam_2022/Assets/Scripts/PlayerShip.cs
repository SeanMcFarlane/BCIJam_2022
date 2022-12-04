using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {
	public static float SHIELD_REFUEL_RATE = 0.1f;
	public static float SHIELD_DRAIN_RATE = -0.5f;
	public static float RELOAD_RATE = 0.4f;
	public static float SHOOT_CHARGE_RATE = 0.33f;

	public static float DAMAGE_PER_SHOT = 0.1f;
	public static float SHIELD_DAMAGE_REDUCTION = 0.75f;

	[ReadOnly] public Animator o_Animator;
	[ReadOnly] public Animator o_TurretAnimator;

	[ReadOnly] public float reloadProgress = 0;
	[ReadOnly] public float health = 1;
	[ReadOnly] public float shieldFuel = 1;

	[ReadOnly] public bool chargingCannon = false;
	[ReadOnly] public bool shieldsOnline = false;
	[ReadOnly] public bool reloading = false;
	[ReadOnly] public bool shipDestroyed = false;

	public StatusBarUI healthBarUI;
	public StatusBarUI shieldsBarUI;
	public StatusBarUI reloadBarUI;


	public Transform[] shootPoints;
	public Transform turretPivot;

	public PlayerShip enemyShip;

	public void ShootCannonAtStern() {
		StartShootingCannon(ShipSection.STERN);
	}

	public void ShootCannonAtBow() {
		StartShootingCannon(ShipSection.BOW);
	}

	private void StartShootingCannon(ShipSection target) {
		if(reloadProgress < 1.0f) {
			return;
		}
		chargingCannon = true;
		if(target == ShipSection.STERN) {
			Vector3 shootVector = enemyShip.shootPoints[(int)ShipSection.STERN].position - turretPivot.position;
			turretPivot.eulerAngles = new Vector3(0, 0, Get2DAngle(shootVector));
		}
		else if(target == ShipSection.BOW) {
			Vector3 shootVector = enemyShip.shootPoints[(int)ShipSection.BOW].position - turretPivot.position;
			turretPivot.eulerAngles = new Vector3(0, 0, Get2DAngle(shootVector));
		}
	}

	public void StopShootingCannon() {
		turretPivot.localEulerAngles = new Vector3(0, 0, 90);
		chargingCannon = false;
	}

	public void CannonFiredSuccessfully() {
		reloadProgress = 0.0f;
		float damage = DAMAGE_PER_SHOT;
		if(enemyShip.shieldsOnline) {
			damage *= 1.0f-SHIELD_DAMAGE_REDUCTION;
		}
		enemyShip.GotHit(damage);
	}

	public void GotHit(float damage) {
		health -= damage;
		if(!shieldsOnline) {
			o_Animator.Play("ShipHit");
		}
		if(health <= 0.0f) {
			o_Animator.Play("ShipExplode");
		}
	}

	public void SetReloading(bool isReloading) {
		reloading = isReloading;
	}

	public void SetShieldsOnline(bool areShieldsOnline) {
		shieldsOnline = areShieldsOnline;
	}

	// Start is called before the first frame update
	void Start() {
		o_Animator = GetComponent<Animator>();
		o_TurretAnimator = turretPivot.GetComponent<Animator>();
	}

	// Update is called once per frame
	void FixedUpdate() {
		o_Animator.SetBool("ShieldsOnline", shieldsOnline);
		o_Animator.SetBool("Reloading", reloading);

		o_TurretAnimator.SetBool("ChargingCannon", chargingCannon);

		if(!shieldsOnline) {
			shieldFuel += SHIELD_REFUEL_RATE*Time.fixedDeltaTime;
			shieldFuel = Mathf.Clamp01(shieldFuel);
		}
		else {
			shieldFuel += SHIELD_DRAIN_RATE*Time.fixedDeltaTime;
			shieldFuel = Mathf.Clamp01(shieldFuel);
			if(shieldFuel <= 0.0f) {
				shieldsOnline = false;
			}
		}

		if(reloading) {
			reloadProgress += RELOAD_RATE*Time.fixedDeltaTime;
			reloadProgress = Mathf.Clamp01(reloadProgress);
		}

		healthBarUI.fillAmount = health;
		shieldsBarUI.fillAmount = shieldFuel;
		reloadBarUI.fillAmount = reloadProgress;
	}

	[SerializeField]
	public enum ShipSection {
		STERN, BOW
	}

	public static float Get2DAngle(Vector2 vector2) {// Get angle, from -180 to +180 degrees. Degree offset to horizontal right.
		float angle = Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
		angle = 90 - angle;
		if(angle > 180)
			angle = -360 + angle;
		return angle;
	}

	public static float Get2DAngle(Vector3 vector3, float degOffset) {
		float angle = Mathf.Atan2(vector3.x, vector3.y) * Mathf.Rad2Deg;
		angle = degOffset - angle;
		if(angle > 180)
			angle = -360 + angle;
		return angle;
	}

	public static float Get2DAngle(Vector3 vector3) {
		float angle = Mathf.Atan2(vector3.x, vector3.y) * Mathf.Rad2Deg;
		angle = 90 - angle;
		if(angle > 180)
			angle = -360 + angle;
		return angle;
	}
}
