using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;

public class LobbyButtonController : MonoBehaviour {

	public RectTransform ParentCanvas;
	public Button[] Buttons;
	public RectTransform Cursor;

	public event UnityAction OnCreateRoomButtonClick;
	public event UnityAction OnInRoomButtonClick;
	public event UnityAction OnBackButtonClick;

	private int _selectionButton = 0;
	private float _controllerDead = 0.3f;
	private bool _canMoveCursor = true;

	private bool _isActive = true;
	public bool IsActive {
		get { return _isActive; }
		set {
			if(_isActive == value) return;
			_isActive = value;
			ParentCanvas.gameObject.SetActive(_isActive);
		}
	}

	private void Awake() {

		Buttons[0].onClick.AddListener(() => {
			if(!IsActive) return;
			OnCreateRoomButtonClick?.Invoke();
			});
		Buttons[1].onClick.AddListener(() => {
			if(!IsActive) return;
			OnInRoomButtonClick?.Invoke();
		});
		Buttons[2].onClick.AddListener(() => {
			if(!IsActive) return;
			OnBackButtonClick?.Invoke();
		});

	}

	private void Update() {
		if(!IsActive) return;

		if(Input.GetButtonDown("Attack")) {
			switch(_selectionButton) {
				case 0: OnCreateRoomButtonClick?.Invoke(); break;
				case 1: OnInRoomButtonClick?.Invoke(); break;
				case 2: OnBackButtonClick?.Invoke(); break;
			}
		}

		var input = Input.GetAxisRaw("Horizontal");

		if(Mathf.Abs(input) < _controllerDead) {
			_canMoveCursor = true;
			return;
		}

		if(!_canMoveCursor) return;

		_canMoveCursor = false;

		_selectionButton = (int)Mathf.Clamp(_selectionButton + Mathf.Sign(input), 0, 2);
		Cursor.position = Buttons[_selectionButton].transform.position;

	}
}
