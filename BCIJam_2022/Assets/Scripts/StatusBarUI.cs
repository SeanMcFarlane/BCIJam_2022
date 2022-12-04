using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StatusBarUI : MonoBehaviour {

	[Range(0, 1)] public float fillAmount = 1.0f;
	[SerializeField] RectTransform fillRect;

	public FillDirection fillDirection = FillDirection.Horizontal;

	public void Update() {
		UpdateUI();
	}

	[Button("Test fill scaling")]
	public void UpdateUI() {
		if(fillDirection == FillDirection.Horizontal) {
			fillRect.anchorMax = new Vector2(fillAmount, fillRect.anchorMax.y);
		}
		else if(fillDirection == FillDirection.Vertical) {
			fillRect.anchorMax = new Vector2(fillRect.anchorMax.x, fillAmount);
		}
		else if(fillDirection == FillDirection.ReverseHorizontal) {
			fillRect.anchorMin = new Vector2(1.0f-fillAmount, fillRect.anchorMin.y);
		}
	}

	public enum FillDirection {
		Horizontal,
		Vertical,
		ReverseHorizontal
	}
}
