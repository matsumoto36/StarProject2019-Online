using UnityEngine;
using System.Collections;

namespace Matsumoto.Character {

	/// <summary>
	/// 通常のプレイヤーの入力
	/// </summary>
	public class OfflinePlayerController : PlayerController {

		public override float GetAxis(string axisName) {
			return Input.GetAxisRaw(axisName);
		}

		public override bool GetButton(string buttonName) {
			return Input.GetButton(buttonName);
		}

		public override bool GetButtonDown(string buttonName) {
			return Input.GetButtonDown(buttonName);
		}

		public override bool GetKeyDown(KeyCode key) {
			return Input.GetKeyDown(key);
		}
	}
}

