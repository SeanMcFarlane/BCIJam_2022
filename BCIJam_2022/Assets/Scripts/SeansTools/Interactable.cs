using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class GameObjEvent : UnityEvent<GameObject> {
}

public class Interactable : MonoBehaviour {

	/* 
	Place this component on any GameObject in the scene that has a collider.
	Then, you can connect it to any other GameObject in the scene and execute public functions of them.
	This is useful for making any buttons, damaging zones, levers, doors, etc that need to interacted with.
	The trigger unityevents will only work if the collider is set to "useTrigger" mode.
	*/
	public GameObject localPlayer;

	[SerializeField] public KeyCode keyboardPressCode;

	[SerializeField] private bool useMouseEvents = false;
	[ShowIf("useMouseEvents"), FoldoutGroup("Mouse", Expanded = true)]
	[SerializeField] public GameObjEvent mouseEnterEvent;
	[ShowIf("useMouseEvents"), FoldoutGroup("Mouse", Expanded = true)]
	[SerializeField] public GameObjEvent mouseExitEvent;
	[ShowIf("useMouseEvents"), FoldoutGroup("Mouse", Expanded = true)]
	[SerializeField] public GameObjEvent mouseDownEvent;
	[ShowIf("useMouseEvents"), FoldoutGroup("Mouse", Expanded = true)]
	[SerializeField] public GameObjEvent mouseUpEvent;
	[ShowIf("useMouseEvents"), FoldoutGroup("Mouse", Expanded = true)]
	[SerializeField] public GameObjEvent rightMouseDownEvent;


	[SerializeField] private bool useTriggerEvents = false;
	[ShowIf("useTriggerEvents"), FoldoutGroup("Collision", Expanded = true)]
	[SerializeField] public GameObjEvent enterTriggerEvent;
	[ShowIf("useTriggerEvents"), FoldoutGroup("Collision", Expanded = true)]
	[SerializeField] public GameObjEvent exitTriggerEvent;

	//
	// Interact code used for triggering things near the player fighter when pressing E
	//
	[FoldoutGroup("Interact", Expanded = true)]
	[SerializeField] public GameObjEvent interactEvent;
	[FoldoutGroup("Interact", Expanded = true)]
	[SerializeField] public GameObjEvent interactSelectEvent;
	[FoldoutGroup("Interact", Expanded = true)]
	[SerializeField] public UnityEvent interactDeselectEvent;

	[FoldoutGroup("Monobehavior", Expanded = true)]
	[SerializeField] public UnityEvent onStartEvent;
	[FoldoutGroup("Monobehavior", Expanded = true)]
	[SerializeField] public UnityEvent onEnableEvent;
	[FoldoutGroup("Monobehavior", Expanded = true)]
	[SerializeField] public UnityEvent onDisableEvent;

	[SerializeField] [ReadOnly] public GameObject interactor; //This is the entity that is highlighting this entity for an interaction. Null if no active interactor present.

	[SerializeField] [ReadOnly] private bool isHighlighted = false;

	void OnMouseEnter() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		mouseEnterEvent.Invoke(localPlayer);
	}

	//Currently allowing mouse exit even when input is locked!! This is to allow animations to reset.
	//As a result don't use this for gameplay affecting logic unless necessary.
	void OnMouseExit() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		mouseExitEvent.Invoke(localPlayer);
	}

	void OnMouseDown() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		if(Input.GetMouseButton(1)) {
			rightMouseDownEvent.Invoke(localPlayer);
		}
		else {
			mouseDownEvent.Invoke(localPlayer);
		}
	}

	private void OnMouseUp() {
		if(!useMouseEvents || !isActiveAndEnabled) return;
		mouseUpEvent.Invoke(localPlayer);
	}

	void OnTriggerEnter2D(Collider2D col) {
		if(!useTriggerEvents || !isActiveAndEnabled) return;
		enterTriggerEvent.Invoke(col.gameObject);
	}

	void OnTriggerExit2D(Collider2D col) {
		if(!useTriggerEvents || !isActiveAndEnabled) return;
		exitTriggerEvent.Invoke(col.gameObject);
	}

	private void Start() {
		onStartEvent.Invoke();
	}

	public void Enable() {
		if(this.enabled) return;
		this.enabled = true;
		onEnableEvent.Invoke();
	}

	public void Disable() {
		if(!enabled) return;
		onDisableEvent.Invoke();
		this.enabled = false;
	}

	// Update is called once per frame
	void Update() {
		if(Input.GetKeyDown(keyboardPressCode)) {
			OnMouseDown();
		}
		if(Input.GetKeyUp(keyboardPressCode)) {
			OnMouseUp();
		}

		if(interactor != null && !isHighlighted) {
			isHighlighted = true;
			interactSelectEvent.Invoke(interactor);
		}
		else if(interactor == null && isHighlighted) {
			isHighlighted = false;
			interactDeselectEvent.Invoke();
		}
	}
}


