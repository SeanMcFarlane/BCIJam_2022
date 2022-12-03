using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Controls the margins of a recttransform so that they are evenly sized even with different shaped parent rects. 
//Necessary for normalized scaling but not for absolute pixel values.
//Also allows you to lock a RectTransform to a specific aspect ratio, and it will scale to fit one axis to match another depending on aspect mode.
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class FulcrumScalableUIMargins : MonoBehaviour {

	public bool propagateToChildren = false;
	public bool lockAspect = false;
	[ShowIf("lockAspect")]
	public AspectMode aspectMode = AspectMode.Width;
	[ShowIf("lockAspect")]
	public float aspectRatio = 1.0f;

	[SerializeField, FoldoutGroup("Affected sides")] bool top = false;
	[SerializeField, FoldoutGroup("Affected sides")] bool bottom = false;
	[SerializeField, FoldoutGroup("Affected sides")] bool left = false;
	[SerializeField, FoldoutGroup("Affected sides")] bool right = false;

	public float normalizedMarginSize;
	public bool liveUpdate = true;
	public bool liveUpdateEditor = false;

	float prevNormalizedMarginSize;
	Rect lastConfiguration;
	RectTransform o_ParentRect;

	public bool ShouldUpdate() {
#if UNITY_EDITOR
		if(!Application.isPlaying && !liveUpdateEditor) return false;
#endif
		return liveUpdate;
	}

	public void Start() {
		if(ShouldUpdate()) RecalculateMargins(false);
	}

	public void OnResolutionChange() {
		if(ShouldUpdate()) RecalculateMargins(false);
	}

	//private void OnValidate() {
	//	//	if(ShouldUpdate()) RecalculateMargins(true);
	//}

	[Button("Recalculate margins")]
	public void RecalculateButtonPressed() {
		RecalculateMargins(false);
	}

	private void Update() {
		if(ShouldUpdate()) {
			o_ParentRect = this.transform.parent.GetComponent<RectTransform>();
			if(!o_ParentRect) return;
			if(o_ParentRect.rect != lastConfiguration || prevNormalizedMarginSize != normalizedMarginSize) {
				RecalculateMargins(false);
				lastConfiguration = o_ParentRect.rect;
				prevNormalizedMarginSize = normalizedMarginSize;
			}
		}
	}

	public void RecalculateMargins(bool calledOnValidate) {
		if(this.transform.parent == null || this.transform.parent.GetComponent<RectTransform>() == null) {
			Debug.LogError("Scalable margins script must be used on an entity with a RectTransform parent to contain it! Used on: "+gameObject.name, gameObject);
			return;
		}

		RectTransform parent = this.transform.parent.GetComponent<RectTransform>();

		if(parent.rect.height == 0) return;

		float ratio = parent.rect.width/parent.rect.height;
		if(ratio <= float.Epsilon) return;

		RectTransform myRect = GetComponent<RectTransform>();
		float topMargin = top ? 1f-(normalizedMarginSize*ratio) : myRect.anchorMax.y;
		float bottomMargin = bottom ? (normalizedMarginSize*ratio) : myRect.anchorMin.y;
		float leftMargin = left ? normalizedMarginSize : myRect.anchorMin.x;
		float rightMargin = right ? 1f-normalizedMarginSize : myRect.anchorMax.x;

		myRect.anchorMin = new Vector2(leftMargin, bottomMargin);
		myRect.anchorMax = new Vector2(rightMargin, topMargin);

		if(lockAspect) {
			Vector2 proportions = myRect.anchorMax-myRect.anchorMin;

			switch(aspectMode) {
				case AspectMode.Width:
					if(proportions.x != 0) {
						myRect.anchorMax = new Vector2(rightMargin, myRect.anchorMin.y+aspectRatio*proportions.x*ratio);
					}
					break;
				case AspectMode.Height:
					if(proportions.y != 0) myRect.anchorMax = new Vector2(myRect.anchorMin.x+(aspectRatio*proportions.y)/ratio, topMargin);
					break;
				case AspectMode.MatchLesser:
					if(proportions.x > proportions.y*ratio && proportions.x != 0) {
						myRect.anchorMax = new Vector2(rightMargin, myRect.anchorMin.y+aspectRatio*proportions.x*ratio);
					}
					else if(proportions.y*ratio > proportions.x && proportions.y != 0) {
						myRect.anchorMax = new Vector2(myRect.anchorMin.x+(aspectRatio*proportions.y)/ratio, topMargin);
					}
					break;
				case AspectMode.MatchGreater:
					if(proportions.x < proportions.y*ratio && proportions.x != 0) {
						myRect.anchorMax = new Vector2(rightMargin, myRect.anchorMin.y+aspectRatio*proportions.x*ratio);
					}
					else if(proportions.y*ratio < proportions.x && proportions.y != 0) {
						myRect.anchorMax = new Vector2(myRect.anchorMin.x+(aspectRatio*proportions.y)/ratio, topMargin);
					}
					break;
			}
		}

		if(propagateToChildren) {
			FulcrumScalableUIMargins[] children = GetComponentsInChildren<FulcrumScalableUIMargins>();
			foreach(FulcrumScalableUIMargins child in children) {
				if(child != this && child.ShouldUpdate()) child.RecalculateMargins(calledOnValidate);
			}
		}

		//Rect changes should prompt the universal conformer to react as well.
		RectTransformUniversalConformer conformer = GetComponent<RectTransformUniversalConformer>();
		if(conformer) conformer.ConformAll(calledOnValidate);
#if UNITY_EDITOR
		EditorUtility.SetDirty(myRect);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public enum AspectMode {
		Width,
		Height,
		MatchLesser,
		MatchGreater
	}
}
