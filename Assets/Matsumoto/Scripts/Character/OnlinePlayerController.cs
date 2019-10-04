using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Matsumoto.Character {

	/// <summary>
	/// 通常のプレイヤーの入力
	/// </summary>
	public class OnlinePlayerController : PlayerController {

		private PhotonView _photonView;

		private void Awake() {
			_photonView = GetComponent<PhotonView>();
		}

		public override float GetAxis(string axisName) {
			if(_photonView.IsMine) return 0.0f;
			return Input.GetAxisRaw(axisName);
		}

		public override bool GetButton(string buttonName) {
			if(_photonView.IsMine) return false;
			return Input.GetButton(buttonName);
		}

		public override bool GetButtonDown(string buttonName) {
			if(_photonView.IsMine) return false;
			return Input.GetButtonDown(buttonName);
		}

		public override bool GetKeyDown(KeyCode key) {
			if(_photonView.IsMine) return false;
			return Input.GetKeyDown(key);
		}
	}
}

