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
        LobbyButtonController _lobbyButtonController;

        string _roomName;
        public string RoomName { get { return _roomName; } }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            _lobbyButtonController = FindObjectOfType<LobbyButtonController>();
            
            // 部屋を作成
            _lobbyButtonController.OnCreateRoomButtonClick += () => 
            {
                _roomName = _lobbyButtonController.SetedLobbyName;
                _connect.CreateOrJoinRoom(_roomName);
                Debug.Log(_roomName);
            };
            // 部屋を作成
            _lobbyButtonController.OnInRoomButtonClick += () => 
            {
                _roomName = _lobbyButtonController.SetedLobbyName;
                _connect.CreateOrJoinRoom(_roomName);
            };

            _lobbyButtonController.OnBackButtonClick += () => 
            {
                _connect.LeaveRoom();
            };

            _connect = FindObjectOfType<OnlineConnect>();
            _connect.Connect("2.0");

            _connect.OnDisconnect = () => 
            {
                Destroy(gameObject);
            };
        }

        /// <summary>
        /// 部屋に入室したとき
        /// </summary>
        public override void OnJoinedRoom()
        {
            OnlineData.PlayerID = PhotonNetwork.PlayerList.Length;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!PhotonNetwork.IsMasterClient) return;
            
            if(PhotonNetwork.PlayerList.Length >= 2)
            {
                SceneChanger.Instance.MoveScene("BattleScene", 1.0f, 1.0f, SceneChangeType.StarBlackFade, true);
            }
            
        }

        public void CreateRoom()
        {
            if (!PhotonNetwork.InLobby) return;

            _connect.CreateOrJoinRoom("Test");
        }
    }
}
