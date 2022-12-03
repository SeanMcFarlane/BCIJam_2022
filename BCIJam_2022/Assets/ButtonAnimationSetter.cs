using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationSetter : MonoBehaviour {
	[SerializeField] Animator o_Animator;
	public void SetHighlighted(bool isHighlighted) {
		o_Animator.SetBool("Highlighted", isHighlighted);
	}
	public void SetPressed(bool isPressed) {
		o_Animator.SetBool("Pressed", isPressed);
	}
}
