using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShip : MonoBehaviour {
	public static float SHIELD_REFUEL_RATE = 0.1f;
	public static float SHIELD_DRAIN_RATE = -0.5f;
	public static float RELOAD_RATE = 0.4f;
	public static float SHOOT_CHARGE_RATE = 0.33f;

	public static float TURRET_ROTATION_RATE = 5f;

	public static float DAMAGE_PER_SHOT = 0.21f;
	public static float SHIELD_DAMAGE_REDUCTION = 0.75f;

	public static float MODULE_DAMAGE_DISABLE_TIME = 2.5f;

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

	public TextMeshPro o_ReloadButtonText;
	public TextMeshPro o_ShieldButtonText;

	public float gunDisabledTime = 0.0f;
	public float shieldDisabledTime = 0.0f;
	[ReadOnly] public string defaultReloadText;
	[ReadOnly] public string defaultShieldText;


	public float turretRotation = 90;
	public float turretRotationGoal = 90;

	public Transform[] shootPoints;
	public Transform turretPivot;

	[ReadOnly] [SerializeField] ShipSection aimedSection = ShipSection.NONE;

	public PlayerShip enemyShip;

	public GameObject o_VictoryText;

	public void ShootCannonAtStern() {
		StartShootingCannon(ShipSection.STERN);
	}

	public void ShootCannonAtBow() {
		StartShootingCannon(ShipSection.BOW);
	}

	private void StartShootingCannon(ShipSection target) {
		if(shipDestroyed || gunDisabledTime > 0 || shieldsOnline) return;
		if(reloadProgress < 1.0f) {
			return;
		}
		chargingCannon = true;
		aimedSection = target;
		if(target == ShipSection.STERN) {
			Vector3 shootVector = enemyShip.shootPoints[(int)ShipSection.STERN].position - turretPivot.position;
			turretPivot.eulerAngles = new Vector3(0, 0, Get2DAngle(shootVector));
			turretRotationGoal = turretPivot.localEulerAngles.z;
			turretPivot.localEulerAngles = new Vector3(0, 0, turretRotation);
		}
		else if(target == ShipSection.BOW) {
			Vector3 shootVector = enemyShip.shootPoints[(int)ShipSection.BOW].position - turretPivot.position;
			turretPivot.eulerAngles = new Vector3(0, 0, Get2DAngle(shootVector));
			turretRotationGoal = turretPivot.localEulerAngles.z;
			turretPivot.localEulerAngles = new Vector3(0, 0, turretRotation);
		}
	}

	public void StopShootingCannon() {
		aimedSection = ShipSection.NONE;
		turretRotationGoal = 90.0f;
		chargingCannon = false;
	}

	public void CannonFiredSuccessfully() {
		reloadProgress = 0.0f;
		enemyShip.GotHit(DAMAGE_PER_SHOT, aimedSection);
	}

	public void GotHit(float damage, ShipSection section) {
		if(shieldsOnline) {
			damage *= 1.0f-SHIELD_DAMAGE_REDUCTION;
		}
		else {
			o_Animator.Play("ShipHit");
		}

		if(section == ShipSection.BOW) {
			gunDisabledTime = MODULE_DAMAGE_DISABLE_TIME;
		}
		else if(section == ShipSection.STERN) {
			shieldDisabledTime = MODULE_DAMAGE_DISABLE_TIME;
		}

		health -= damage;

		if(health <= 0.0f) {
			o_Animator.Play("ShipExplode");
			shipDestroyed = true;
			enemyShip.o_VictoryText.SetActive(true);
		}
	}

	public void SetReloading(bool isReloading) {
		if(shipDestroyed || gunDisabledTime > 0 || shieldsOnline) {
			reloading = false;
			return;
		}
		reloading = isReloading;
	}

	public void SetShieldsOnline(bool areShieldsOnline) {
		if(shipDestroyed || shieldDisabledTime > 0 || chargingCannon || reloading) {
			shieldsOnline = false;
			return;
		}
		shieldsOnline = areShieldsOnline;
	}

	// Start is called before the first frame update
	void Start() {
		defaultReloadText = o_ReloadButtonText.text;
		defaultShieldText = o_ShieldButtonText.text;

		o_Animator = GetComponent<Animator>();
		o_TurretAnimator = turretPivot.GetComponent<Animator>();
	}

	// Update is called once per frame
	void FixedUpdate() {

		gunDisabledTime -= Time.fixedDeltaTime;
		gunDisabledTime = Mathf.Clamp(gunDisabledTime, 0, float.MaxValue);

		shieldDisabledTime -= Time.fixedDeltaTime;
		shieldDisabledTime = Mathf.Clamp(shieldDisabledTime, 0, float.MaxValue);


		if(shieldDisabledTime > 0) {
			shieldsOnline = false;
			o_ShieldButtonText.text = "X\n\nDISABLED";
		}
		else {
			o_ShieldButtonText.text = defaultShieldText;
		}

		if(gunDisabledTime > 0) {
			chargingCannon = false;
			reloading = false;
			o_ReloadButtonText.text = "X\n\nDISABLED";
		}
		else {
			o_ReloadButtonText.text = defaultReloadText;
		}

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

		turretRotationGoal = Mathf.Clamp(turretRotationGoal, 0, 360);
		turretRotation = turretRotation % 360;
		float turretRotationDelta1 = turretRotationGoal-turretRotation;
		float turretRotationDelta2 = turretRotation-(360-turretRotationGoal);

		float turretRotationDelta = Mathf.Abs(turretRotationDelta1) < Mathf.Abs(turretRotationDelta2) ? turretRotationDelta1 : turretRotationDelta2;

		if(Mathf.Abs(turretRotationDelta) > TURRET_ROTATION_RATE) {
			turretRotation += (turretRotationDelta > 0) ? TURRET_ROTATION_RATE : -TURRET_ROTATION_RATE;
		}
		else {
			turretRotation = turretRotationGoal;
		}

		turretPivot.localEulerAngles = new Vector3(0, 0, turretRotation);

		healthBarUI.fillAmount = health;
		shieldsBarUI.fillAmount = shieldFuel;
		reloadBarUI.fillAmount = reloadProgress;
	}

	public void LateUpdate() {
		if(!shipDestroyed) {
			turretPivot.localEulerAngles = new Vector3(0, 0, turretRotation);
		}
	}

	[SerializeField]
	public enum ShipSection {
		STERN, BOW, NONE
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
