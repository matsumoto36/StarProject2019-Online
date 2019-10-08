using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace Saitou.Online
{

    public class LobbyConnect : MonoBehaviourPunCallbacks,IInRoomCallbacks
    {
        OnlineConnect _connect;

        public Button[] JoinButton;

        void Start()
        {
            DontDestroyOnLoad(gameObject);

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

        /// <summary>
        /// 部屋に入室したとき
        /// </summary>
        public override void OnJoinedRoom()
        {
            //Debug.Log("入ったよ");
            Debug.Log("PlayerID" + PhotonNetwork.PlayerList.Length);
            OnlineData.PlayerID = PhotonNetwork.PlayerList.Length;
            //PhotonNetwork.AutomaticallySyncScene = false;
            ////PhotonNetwork.LoadLevel("BattleScene");
            //SceneManager.LoadScene("BattleScene");
            ////SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!PhotonNetwork.IsMasterClient) return;
            
            if(PhotonNetwork.PlayerList.Length >= 2)
            {
                PhotonNetwork.LoadLevel("BattleScene");
            }
            
        }

        void Update()
        {
            //if(Input.GetMouseButtonDown(0))
            //{
            //    Debug.Log(OnlineData.Instance.PlayerID);
            //}
        }

        public void CreateRoom()
        {
            if (!PhotonNetwork.InLobby) return;

            _connect.CreateOrJoinRoom("Test");
        }
    }
}
