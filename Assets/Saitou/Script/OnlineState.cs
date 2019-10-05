using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saitou.Online
{

    public class OnlineState : MonoBehaviour
    {
        [SerializeField] PlayerList _list;
        public PlayerList _PlayerList
        {
            get { return _list; }
        }
    }
}