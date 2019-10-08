using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Saitou.Online
{

    public class LobbyConnect : MonoBehaviour
    {
        OnlineConnect _connect;

        public Button[] JoinButton;

        void Start()
        {
            _connect = FindObjectOfType<OnlineConnect>();
            _connect.Connect("2.0");

            for(int i = 0; i < JoinButton.Length; i++)
            {
                // ロビー接続までボタンが押せないようにする
                JoinButton[i].enabled = false;
            }

            // 接続されたらボタンを有効化
            _connect.OnJoinLobbySuccess = () => 
            {
                for (int i = 0; i < JoinButton.Length; i++)
                {
                    // ロビー接続までボタンが押せないようにする
                    JoinButton[i].enabled = true;
                }
            };
        }

        public void CreateRoom()
        {
            if (!PhotonNetwork.InLobby) return;

            _connect.CreateOrJoinRoom("Test");

            StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(1.5f);

            SetOnlineData();

            yield return new WaitForSeconds(0.5f);

            SceneManager.LoadScene("BattleScene");
            Debug.Log("遷移");
        }

        public void SetOnlineData()
        {
            Debug.Log(PhotonNetwork.PlayerList.Length);
            OnlineData.Instance.SetPlayerID(PhotonNetwork.PlayerList.Length);
        }


    }
}
