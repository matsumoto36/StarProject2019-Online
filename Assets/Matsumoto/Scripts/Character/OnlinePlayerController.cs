using UnityEngine;
using System.Collections;
using Photon.Pun;
using Saitou.Online;

namespace Matsumoto.Character {

	/// <summary>
	/// 通常のプレイヤーの入力
	/// </summary>
	public class OnlinePlayerController : PlayerController {

        //private PhotonView _photonView;

        OnlineState state;
        OnlineManager manager;

		private void Awake() {
            //_photonView = GetComponent<PhotonView>();
            state = GetComponent<OnlineState>();
            manager = FindObjectOfType<OnlineManager>();
		}

		public override float GetAxis(string axisName) {
			if((int)state._PlayerList != manager.PlayerID) return 0.0f;
			return Input.GetAxisRaw(axisName);
		}

		public override bool GetButton(string buttonName) {
			if((int)state._PlayerList != manager.PlayerID) return false;
			return Input.GetButton(buttonName);
		}

		public override bool GetButtonDown(string buttonName) {
			if((int)state._PlayerList != manager.PlayerID) return false;
			return Input.GetButtonDown(buttonName);
		}

		public override bool GetKeyDown(KeyCode key) {
			if((int)state._PlayerList != manager.PlayerID) return false;
			return Input.GetKeyDown(key);
		}
	}
}

