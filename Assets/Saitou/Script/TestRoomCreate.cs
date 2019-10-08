using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Saitou.Online
{
    public class TestRoomCreate : MonoBehaviourPunCallbacks
    {

        // Use this for initialization
        void Start()
        {

        }

        private void Update()
        {
            Debug.Log(OnlineData.PlayerID);
            //Debug.Log(PhotonNetwork.AutomaticallySyncScene);
        }
    }
}