using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saitou.Online;

public class OnlineData : SingletonMonoBehaviour<OnlineData> {

    int playerID = 0;
    public int PlayerID { get { return playerID; } }


    public void SetPlayerID(int id)
    {
        playerID = id;
    }

}
