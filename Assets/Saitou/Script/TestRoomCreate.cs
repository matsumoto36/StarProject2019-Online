using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Saitou.Online
{
    public class TestRoomCreate : MonoBehaviourPunCallbacks
    {

        public OnlineConnect Connect;

        // Use this for initialization
        void Start()
        {
            Connect = FindObjectOfType<OnlineConnect>();

            Connect.OnJoinLobbySuccess = () =>
            {
                if (PhotonNetwork.InRoom == false)
                {
                    Connect.CreateOrJoinRoom("room01");
                }
            };
        }

        private void Update()
        {
           
        }
    }
}