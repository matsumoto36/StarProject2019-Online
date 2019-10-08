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

        public string RoomName;

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            _lobbyButtonController = FindObjectOfType<LobbyButtonController>();
            _lobbyButtonController.OnCreateRoomButtonClick += () => { _connect.CreateOrJoinRoom(RoomName); };
            _lobbyButtonController.OnInRoomButtonClick += () => { _connect.CreateOrJoinRoom(RoomName); };

            _connect = FindObjectOfType<OnlineConnect>();
            _connect.Connect("2.0");
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
                PhotonNetwork.LoadLevel("BattleScene");
            }
            
        }

        public void CreateRoom()
        {
            if (!PhotonNetwork.InLobby) return;

            _connect.CreateOrJoinRoom("Test");
        }
    }
}
