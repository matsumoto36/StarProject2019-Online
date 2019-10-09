﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressEnemy : EnemyBase, IEnemy
{
    // 自身の初期位置
    private Vector2 originPos;
    // 索敵範囲
    [SerializeField]
    private float pressRange;
    // 移動に使う変数
    float time = 0.0f;

    /// <summary>
    /// 初期化
    /// </summary>
    void Start () {
        target = GameObject.Find("Player");
        myRig = GetComponent<Rigidbody2D>();
        originPos = transform.position;
        canAction = true;
    }
	
	/// <summary>
    /// 更新処理
    /// </summary>
	void Update () {
        CheckAction();

        Move();
	}

    /// <summary>
    /// 移動
    /// </summary>
    public void Move()
    {
        if (canAction == false) return;

        if (Mathf.Abs(transform.position.y - originPos.y) <= 0.5f) {
            time = 0.0f;
            return;
        }

        time += Time.deltaTime;

        // 目標地点に移動
        transform.position = Vector2.Lerp(transform.position, originPos, time * Time.deltaTime);
    }

    /// <summary>
    /// アクションを起こすかチェック
    /// </summary>
    protected override void CheckAction()
    {
        // ターゲットがいないならリターン
        if (target == null || canAction == false) return;

        // プレイヤーとの距離を検出
        playerDis = CheckDistanceX(target.transform.position);
        // 高低差
        float offsetY = CheckDistanceY(target.transform.position);

        // アクション範囲にいるなら
        if (playerDis <= actionRange && offsetY <= pressRange) Action();

    }

    /// <summary>
    /// アクション(攻撃など)
    /// </summary>
    public void Action()
    {
        time = 0.0f;

        canAction = false;

        // 重力適用
        myRig.gravityScale = 2.0f;

        StartCoroutine(IntervalAction(2.0f));
    }

    /// <summary>
    /// アクションのインターバル
    /// </summary>
    /// <param name="_interval">待機時間</param>
    /// <returns></returns>
    protected override IEnumerator IntervalAction(float _interval)
    {
        yield return new WaitForSeconds(_interval);

        // 重力無視
        myRig.gravityScale = 0.0f;

        canAction = true;
    }

    /// <summary>
    /// ダメージを受けた際の処理
    /// </summary>
    public bool ApplyDamage(GameObject damager, DamageType type, float power = 1.0f)
    {
        Destroy(gameObject);
		return true;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="col">当たったコリジョン</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        // Playerの取得
        var player = col.gameObject.GetComponent<Matsumoto.Character.Player>();
        if (player == null) return;

        // プレイヤーにダメージを与える
        player.ApplyDamage(gameObject, DamageType.Enemy);

        target = null;
    }
}
