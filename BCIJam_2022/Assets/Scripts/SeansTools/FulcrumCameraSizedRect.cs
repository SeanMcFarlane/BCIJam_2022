using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

#endif


//Currently the correct way to configure root level RectTransforms.
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class FulcrumCameraSizedRect : MonoBehaviour {
	[SerializeField] bool updateInEditor = false;

	RectTransform o_Rect;
	Camera mainCamera;
	[SerializeField] Camera overrideCamera;


	private void OnDisable() {
		if(Application.isPlaying) {
			FulcrumResolutionRectifier.OnResolutionChange -= UpdateScaling;
		}
	}

	private void OnEnable() {
		if(Application.isPlaying) {
			FulcrumResolutionRectifier.OnResolutionChange += UpdateScaling;
		}
	}

	void Start() {
		UpdateScaling();
	}

	void OnValidate() {
		UpdateScaling();
	}

	public void UpdateScaling() {
#if UNITY_EDITOR
		if(!updateInEditor) return;
		if(UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null) {
			return;
		}
#endif

		if(!gameObject.scene.IsValid()) return;

		mainCamera = Camera.main;
		if(overrideCamera != null) {
			mainCamera = overrideCamera;
		}

		o_Rect = GetComponent<RectTransform>();

		if(mainCamera == null) {
			return;
		}

		Vector2 desiredScaling = new Vector2(mainCamera.aspect*mainCamera.orthographicSize*2, mainCamera.orthographicSize*2);

		if(transform.parent == mainCamera.transform && transform.localPosition == Vector3.zero && o_Rect.sizeDelta == desiredScaling) {
			return;
		}

		o_Rect.SetParent(mainCamera.transform);
		transform.localPosition = new Vector3(0.0f, 0.0f, transform.localPosition.z);
		o_Rect.sizeDelta = desiredScaling;
	}
}
