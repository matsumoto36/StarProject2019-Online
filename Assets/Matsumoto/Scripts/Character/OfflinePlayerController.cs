using UnityEngine;
using System.Collections;

namespace Matsumoto.Character {

	/// <summary>
	/// 通常のプレイヤーの入力
	/// </summary>
	public class OfflinePlayerController : MonoBehaviour , IPlayerController {

		public float GetAxis(string axisName) {
			return Input.GetAxisRaw(axisName);
		}

		public bool GetButton(string buttonName) {
			return Input.GetButton(buttonName);
		}

		public bool GetButtonDown(string buttonName) {
			return Input.GetButtonDown(buttonName);
		}

		public bool GetKeyDown(KeyCode key) {
			return Input.GetKeyDown(key);
		}
	}
}

