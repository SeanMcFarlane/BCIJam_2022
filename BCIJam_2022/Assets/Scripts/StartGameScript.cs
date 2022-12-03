using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour {
	[SerializeField] string gameplaySceneName;

	public void StartGameplay() {
		SceneManager.LoadScene(gameplaySceneName);
	}
}
