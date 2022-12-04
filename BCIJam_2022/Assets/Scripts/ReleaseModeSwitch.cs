using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class ReleaseModeSwitch : MonoBehaviour {

	private void Awake() {
		UpdateReleaseEnabledObjects();
		SceneManager.sceneLoaded += UpdateOnSceneLoad;
	}

	void UpdateOnSceneLoad(Scene scene, LoadSceneMode mode) {
		UpdateReleaseEnabledObjects();
	}

	void UpdateReleaseEnabledObjects() {
		ReleaseModeToggle[] sceneObjects = Resources.FindObjectsOfTypeAll<ReleaseModeToggle>();
		foreach(ReleaseModeToggle r in sceneObjects) {
			r.SetActiveState();
		}
	}
}

