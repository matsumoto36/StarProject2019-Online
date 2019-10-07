using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Saitou.Online
{

    public class PlayerPosSync : MonoBehaviour
    {

        PhotonTransformViewClassic _photonTransform;

        OnlineState state;
        OnlineManager manager;

        void Start()
        {
            _photonTransform = GetComponent<PhotonTransformViewClassic>();
            state = GetComponent<OnlineState>();
            manager = FindObjectOfType<OnlineManager>();
        }

        // Update is called once per frame
        void Update()
        {
            //if ((int)state._PlayerList == manager.PlayerID)
            //{
            //    //現在の移動速度
            //    var velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            //    //移動速度を指定
            //    _photonTransform.SetSynchronizedValues(speed: velocity, turnSpeed: 0);
            //}
        }
    }
}