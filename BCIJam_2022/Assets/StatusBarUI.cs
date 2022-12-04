using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StatusBarUI : MonoBehaviour {

	[Range(0, 1)] public float fillAmount = 1.0f;
	[SerializeField] RectTransform fillRect;

	public FillDirection fillDirection = FillDirection.Horizontal;

	public void Update() {
		fillAmount -= 0.1f*Time.deltaTime;
		UpdateUI();
	}

	[Button("Test fill scaling")]
	public void UpdateUI() {
		if(fillDirection == FillDirection.Horizontal) {
			fillRect.anchorMax = new Vector2(fillAmount, fillRect.anchorMax.y);
		}
		else {
			fillRect.anchorMax = new Vector2(fillRect.anchorMax.x, fillAmount);
		}
	}

	public enum FillDirection {
		Horizontal,
		Vertical
	}
}
