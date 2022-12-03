using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FulcrumResolutionRectifier : MonoBehaviour {
	bool wasFullscreen;
	Resolution lastResolution;
	bool lateStartComplete = false;

	public delegate void ResolutionChangeDelegate();
	public static ResolutionChangeDelegate OnResolutionChange;

	// Start is called before the first frame update
	void Start() {
		wasFullscreen = Screen.fullScreen;
		lastResolution = Screen.currentResolution;
	}

	// Update is called once per frame
	void Update() {
		if(lastResolution.height != Screen.currentResolution.height || lastResolution.width != Screen.currentResolution.width) {
			ResolutionChanged();
			lastResolution = Screen.currentResolution;
		}
		if(wasFullscreen != Screen.fullScreen) {
			OnFullscreenStateChange();
			wasFullscreen = Screen.fullScreen;
		}
	}

	private void LateUpdate() {
		if(lateStartComplete) return;

		ResolutionChanged();

		lateStartComplete = true;
	}

	void OnFullscreenStateChange() {
		ResolutionChanged();
		if(Screen.fullScreen) {
			Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Screen.fullScreen);
		}
	}

	void ResolutionChanged() {
		//Iterate through all UI objects and recompute scaling
		Debug.Log("Detected resolution change, recomputing UI scaling...");
		if(OnResolutionChange != null) OnResolutionChange.Invoke();
	}
}
