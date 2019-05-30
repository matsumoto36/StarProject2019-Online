﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ターゲット
    private GameObject targetObj;

    // 爆破範囲
    [SerializeField]
    private float explosionRange;

	// Use this for initialization
	void Start () {
        targetObj = FindObjectOfType<Matsumoto.Character.Player>().gameObject;

        float distance =  CheckDistance(targetObj.transform.position);

        if(distance <= explosionRange) {
            var player = targetObj.GetComponent<Matsumoto.Character.Player>();
            if (player == null) return;

            player.ApplyDamage(gameObject, DamageType.Enemy);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 目標との距離を返す
    /// </summary>
    /// <param name="_targetPos">目標地点の座標</param>
    /// <returns></returns>
    private float CheckDistance(Vector2 _targetPos)
    {
        // 目標座標との距離の検出
        float _distance = Vector2.Distance(_targetPos, transform.position);

        // 距離を返す
        return _distance;
    }
}
