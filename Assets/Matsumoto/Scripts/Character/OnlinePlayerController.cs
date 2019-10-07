using UnityEngine;
using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using Saitou.Online;

namespace Matsumoto.Character
{
    /// <summary>
	/// 通常のプレイヤーの入力
	/// </summary>
	public class OnlinePlayerController : PlayerController
    {
        
        private PhotonView _photonView;
        
        OnlineState state;
        OnlineManager manager;
        
		Dictionary<string, float> axisCache = new Dictionary<string, float>();
        Dictionary<string, bool> buttonCache = new Dictionary<string, bool>();
        Dictionary<KeyCode, bool> keyCache = new Dictionary<KeyCode, bool>();
        
		private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            state = GetComponent<OnlineState>();
            manager = FindObjectOfType<OnlineManager>();
            
			// 使うキー情報登録
			axisCache.Add("Horizontal", 0.0f);
            axisCache.Add("Vertical", 0.0f);
            buttonCache.Add("Morph", false);
            buttonCache.Add("Attack", false);
            keyCache.Add(KeyCode.P, false);
        }
        
		public override float GetAxis(string axisName)
        {
            if (!PhotonNetwork.InRoom) return 0.0f;

            if ((int)state._PlayerList != manager.PlayerID)
            {
                // 自分以外は送られたキャッシュを使う
                return axisCache[axisName];
            }
            
			var input = Input.GetAxisRaw(axisName);
            // 他プレイヤーに入力を送る
            _photonView.RPC(nameof(SetAxisCache), RpcTarget.All, axisName, input);
            return input;
        }
        
		public override bool GetButton(string buttonName)
        {
            if (!PhotonNetwork.InRoom) return false;

            if ((int)state._PlayerList != manager.PlayerID)
            {
                return buttonCache[buttonName];
            }
            
			var input = Input.GetButton(buttonName);
            _photonView.RPC(nameof(SetButtonCache), RpcTarget.Others, buttonName, input);
            return input;
        }
        
		public override bool GetButtonDown(string buttonName)
        {
            if (!PhotonNetwork.InRoom) return false;

            if ((int)state._PlayerList != manager.PlayerID)
            {
                var cache = buttonCache[buttonName];
                // ButtonDownなので一回判定をとったらfalseにする
                buttonCache[buttonName] = false;
                return cache;
            }
            
			var input = Input.GetButton(buttonName);
            _photonView.RPC(nameof(SetButtonCache), RpcTarget.Others, buttonName, input);
            return input;
        }

		public override bool GetKeyDown(KeyCode key)
        {
            if (!PhotonNetwork.InRoom) return false;

            if ((int)state._PlayerList != manager.PlayerID)
            {
                var cache = keyCache[key];
                keyCache[key] = false;
                return cache;
            }
            
			var input = Input.GetKeyDown(key);
            _photonView.RPC(nameof(SetKeyCache), RpcTarget.Others, key, input);
            return input;
        }
        
		[PunRPC]
        private void SetAxisCache(string key, float value)
        {
            axisCache[key] = value;
        }
        
		[PunRPC]
        private void SetButtonCache(string button, bool value)
        {
            buttonCache[button] = value;
        }
        
		[PunRPC]
        private void SetKeyCache(KeyCode key, bool value)
        {
            keyCache[key] = value;
        }
    }
}