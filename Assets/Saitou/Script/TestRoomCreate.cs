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

        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.InRoom == false)
            {
                if (Input.GetMouseButtonDown(0)) Connect.CreateAndJoinRoom("room01");
                if (Input.GetMouseButtonDown(1)) Connect.JoinRoom("room01");
            }
        }
    }
}