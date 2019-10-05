﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Saitou.Online
{
    public class OnlineConnect : MonoBehaviourPunCallbacks
    {
        [Header("DefaultRoomSettings")]

        // 最大人数
        [SerializeField] int maxPlayers = 4;

        // 公開・非公開
        [SerializeField] bool isVisible = true;

        // 入室の可否
        [SerializeField] bool isOpen = true;

        // 部屋名
        [SerializeField] string roomName = "hoshimaru";

        public Action OnJoinRoomFiled { get; set; }
        public Action OnJoinLobbySuccess { get; set; }
        public Action OnJoinRoomSuccess { get; set; }

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            Connect("1.0");
        }

        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="gameVersion">ゲームバージョン</param>
        void Connect(string gameVersion)
        {
            if(PhotonNetwork.IsConnected == false)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        /// <summary>
        /// ロビー
        /// </summary>
        void JoinLobby()
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        /// <summary>
        /// 部屋を作成し、入出
        /// </summary>
        /// <param name="_roomName"></param>
        public void CreateAndJoinRoom(string _roomName)
        {
            RoomOptions roomOptions = new RoomOptions
            {
                // 部屋の最大人数
                MaxPlayers = (byte)maxPlayers,

                // 公開
                IsVisible = isVisible,

                // 入室可
                IsOpen = isOpen
            };

            // 部屋を作成して入室する
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.CreateRoom(_roomName, roomOptions);
            }
        }

        /// <summary>
        /// 名前指定の部屋に入室
        /// </summary>
        /// <param name="_roomName"></param>
        public void JoinRoom(string _roomName)
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinRoom(_roomName);
            }
        }

        /// <summary>
        /// ランダムな部屋に入室
        /// </summary>
        public void JoinRandomRoom()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        // マスターサーバーに接続した時
        public override void OnConnectedToMaster()
        {
            Debug.Log("サーバーに接続");

            // ロビーに入る
            JoinLobby();
        }


        /// <summary>
        /// 部屋の入室に失敗したとき(名前指定)
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋への入室に失敗しました");
            OnJoinRoomFiled?.Invoke();
        }

        /// <summary>
        /// 部屋の入室に失敗したとき(ランダム)
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("部屋への入室に失敗しました");
            OnJoinRoomFiled?.Invoke();
        }

        /// <summary>
        /// ロビーに入ったとき
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("ロビー入室");
            OnJoinLobbySuccess?.Invoke();
        }

        /// <summary>
        /// 部屋に入室したとき
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log("部屋入室");
            OnJoinRoomSuccess?.Invoke();
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                // 退室
                PhotonNetwork.LeaveRoom();
            }
        }
    }
}