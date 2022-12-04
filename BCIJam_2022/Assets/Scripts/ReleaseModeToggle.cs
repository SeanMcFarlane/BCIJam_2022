using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseModeToggle : MonoBehaviour {
	[SerializeField] bool disableInEditor;
	[SerializeField] bool enableInEditor;
	[SerializeField] bool disableInRelease;
	[SerializeField] bool enableInRelease;

	private void Awake() {
		SetActiveState(); //Safeguard in case this starts later than ReleaseModeSwitch
	}

	public void SetActiveState() {
#if UNITY_EDITOR
		if(disableInEditor) {
			if(!gameObject.scene.IsValid()) return;
			DestroyImmediate(this.gameObject);
		}
		else if(enableInEditor) {
			this.gameObject.SetActive(true);
		}
#else
		if(disableInRelease) {
			if(!gameObject.scene.IsValid()) return;
			DestroyImmediate(this.gameObject);
		}
		else if(enableInRelease) {
			this.gameObject.SetActive(true);
		}
#endif
	}
}
