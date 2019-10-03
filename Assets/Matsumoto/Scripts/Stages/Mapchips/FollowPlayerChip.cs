using System.Collections;
using System.Collections.Generic;
using Matsumoto.Audio;
using UnityEngine;
using Matsumoto.Character;
using UnityEngine.Experimental.U2D.Animation;

namespace Matsumoto.Gimmick {
	public class FollowPlayerChip : GimmickChip {

		public PlayerFollower PlayerFollowerPrefab;
		public GameObject GetFollowerEffectPrefab;
		public SpriteRenderer BodyRenderer;

		public int FollowerIndex;
		public float Amplitude = 0.1f;
		public float Speed = 2.0f;

		private DefaultStageController _defaultController;
		private string _dataKey;
		private float _randomTime;
		private float _startY;

		public override void GimmickStart() {
			base.GimmickStart();

			_defaultController = Controller as DefaultStageController;
			if(!_defaultController) return;

			Debug.Log("_" + FollowerIndex);
			foreach(var item in _defaultController.FollowerData.FindedIndexList) {
				Debug.Log(item);
			}

			if(_defaultController.FollowerData.FindedIndexList
				.Exists(x => x == FollowerIndex)) {
				Destroy(gameObject);
			}
		}

		private void Start() {

			// 色を設定
			var m = BodyRenderer.material;
			m = Instantiate(m);
			m.EnableKeyword("_EMISSION");
			var c = PlayerFollowerPrefab.StarStatus.BodyColor;
			m.SetColor("_EmissionColor", c);

			_randomTime = Random.Range(0, 1);
			_startY = transform.position.y;

			GetComponentInChildren<SpriteSkin>().enabled = true;
		}

		private void Update() {
			// 移動
			var pos = transform.position;
			pos.y = _startY + Mathf.Abs(Mathf.Sin((Time.time + _randomTime) * Speed)) * Amplitude;
			transform.position = pos;
		}

		private void OnTriggerEnter2D(Collider2D collision) {

			if(!_defaultController) return;
			var player = collision.GetComponent<Player>();
			if(!player) return;

			var follower = Instantiate(PlayerFollowerPrefab, transform.position, transform.rotation);
			follower.Target = player;

			_defaultController.AddFollowerData(FollowerIndex);

			AudioManager.PlaySE("GetFollower", position: transform.position);

			var g = Instantiate(GetFollowerEffectPrefab, transform.position, transform.rotation);
			Destroy(g, 5.0f);
			Destroy(gameObject);
		}
	}
}
