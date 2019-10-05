using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

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

        // 2. 部屋に入室する （存在しなければ作成して入室する）
        public void JoinOrCreateRoom()
        {
            // ルームオプションの基本設定
            RoomOptions roomOptions = new RoomOptions
            {
                // 部屋の最大人数
                MaxPlayers = (byte)maxPlayers,

                // 公開
                IsVisible = isVisible,

                // 入室可
                IsOpen = isOpen
            };

            // 入室 (存在しなければ部屋を作成して入室する)
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }
        }
    }
}