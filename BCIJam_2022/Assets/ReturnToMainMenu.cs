using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour {
	static float timeTilBootToMainMenu = 5.0f;

	public void FixedUpdate() {
		timeTilBootToMainMenu -= Time.fixedDeltaTime;
		if(timeTilBootToMainMenu <= 0) {
			SceneManager.LoadScene("MainMenu");
		}
	}
}
