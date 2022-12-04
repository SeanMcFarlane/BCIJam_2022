using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {
	public static float SHIELD_REFUEL_RATE = 0.1f;
	public static float SHIELD_DRAIN_RATE = -0.5f;
	public static float RELOAD_RATE = 0.2f;

	[ReadOnly] public float reloadProgress = 0;
	[ReadOnly] public float health = 1;
	[ReadOnly] public float shieldFuel = 1;

	[ReadOnly] public bool shieldsOnline = false;
	[ReadOnly] public bool reloading = false;

	public StatusBarUI healthBarUI;
	public StatusBarUI shieldsBarUI;
	public StatusBarUI reloadBarUI;


	public Transform[] shootPoints;
	public Transform turretPivot;

	public PlayerShip enemyShip;

	public void ShootCannonAtStern() {
		ShootCannon(ShipSection.STERN);
	}

	public void ShootCannonAtBow() {
		ShootCannon(ShipSection.BOW);
	}

	private void ShootCannon(ShipSection target) {
		if(target == ShipSection.STERN) {
			Vector3 shootVector = enemyShip.shootPoints[(int)ShipSection.STERN].position - turretPivot.position;
			turretPivot.eulerAngles = new Vector3(0, 0, Get2DAngle(shootVector));
		}
		else if(target == ShipSection.BOW) {
			Vector3 shootVector = enemyShip.shootPoints[(int)ShipSection.BOW].position - turretPivot.position;
			turretPivot.eulerAngles = new Vector3(0, 0, Get2DAngle(shootVector));
		}
		reloadProgress = 0.0f;
	}

	public void SetReloading(bool isReloading) {
		reloading = isReloading;
	}

	public void SetShieldsOnline(bool areShieldsOnline) {
		shieldsOnline = areShieldsOnline;
	}

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void FixedUpdate() {
		if(!shieldsOnline) {
			shieldFuel += SHIELD_REFUEL_RATE*Time.fixedDeltaTime;
			shieldFuel = Mathf.Clamp01(shieldFuel);
		}
		else {
			shieldFuel += SHIELD_DRAIN_RATE*Time.fixedDeltaTime;
			shieldFuel = Mathf.Clamp01(shieldFuel);
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
