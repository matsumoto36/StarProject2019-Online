using UnityEngine;
using System.Collections;

namespace Matsumoto.Character {

	/// <summary>
	/// プレイヤーの入力を取得する
	/// </summary>
	public abstract class PlayerController : MonoBehaviour {

		public abstract bool GetButton(string buttonName);
		public abstract bool GetButtonDown(string buttonName);

		public abstract bool GetKeyDown(KeyCode key);

		public abstract float GetAxis(string axisName);

	}

}
