using UnityEngine;
using System.Collections;

namespace Matsumoto.Character {

	/// <summary>
	/// プレイヤーの入力を取得する
	/// </summary>
	public interface IPlayerController{

		bool GetButton(string buttonName);
		bool GetButtonDown(string buttonName);

		bool GetKeyDown(KeyCode key);

		float GetAxis(string axisName);

	}

}
