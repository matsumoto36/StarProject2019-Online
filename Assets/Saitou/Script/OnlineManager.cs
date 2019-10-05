using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Matsumoto.Character;

namespace Saitou.Online
{

    public enum PlayerList
    {
        player_1 = 1,
        player_2
    }

    public class OnlineManager : SingletonMonoBehaviour<OnlineManager>
    {
        int playerID = 1;
        public int PlayerID { get { return playerID; } }

        OnlineConnect _connect;
        PlayerCamera _camera;

        Player[] player = new Player[2];

        void Start()
        {
            _camera = FindObjectOfType<PlayerCamera>();

            _connect = FindObjectOfType<OnlineConnect>();
            _connect.OnJoinRoomSuccess = () =>
            {
                playerID = PhotonNetwork.PlayerList.Length;

                OnlineState[] tmpState = FindObjectsOfType<OnlineState>();

                for (int i = 0; i < tmpState.Length; i++)
                {
                    if (tmpState[i]._PlayerList == PlayerList.player_1) player[0] = tmpState[i].GetComponent<Player>();
                    else player[1] = tmpState[i].GetComponent<Player>();
                }

                Debug.Log(PlayerID - 1);

                _camera.TargetPlayer = player[PlayerID - 1];
            };
        }

        void Update()
        {

        }
    }
}