using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour {
	float timeTilBootToMainMenu = 5.0f;

	public bool countdownActivated = true;

	public void TriggerReturnToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	public void FixedUpdate() {
		if(!countdownActivated) {
			return;
		}
		timeTilBootToMainMenu -= Time.fixedDeltaTime;
		if(timeTilBootToMainMenu <= 0) {
			TriggerReturnToMainMenu();
		}
	}
}
