using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Matsumoto.Character;
using Photon.Pun;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// 風のギミック
    /// </summary>
    public class GimmickWind : MonoBehaviour,IGimmickEffect
    {
        //--------------------------------
        // private
        //--------------------------------

        [SerializeField] int windPower;

        [SerializeField] Transform endPos;

        [SerializeField] bool isWallIgnore = false;

        [SerializeField] GameObject puropera;

		System.Tuple<Player, Rigidbody2D>[] _referencePlayers;
		bool[] _isActivePlayerEffects;

		GameObject _target;

		[SerializeField] ParticleSystem particle;

        ParticleSystem.ShapeModule particleShape;

		Player player;

		//--------------------------------
		// 関数
		//--------------------------------

		void Start() {
			_referencePlayers = FindObjectsOfType<Player>()
				.Select(item => new System.Tuple<Player, Rigidbody2D>(item, item.GetComponent<Rigidbody2D>()))
				.ToArray();

			foreach(var item in _referencePlayers) {
				Debug.Log(item.Item1.name + " " + item.Item2.name);
			}

			_isActivePlayerEffects = new bool[_referencePlayers.Length];

			particleShape = particle.shape;

			float z = (transform.parent.rotation.z > 0 ? transform.parent.rotation.z : transform.parent.rotation.z * -1);
			float w = (transform.parent.rotation.w > 0 ? transform.parent.rotation.w : transform.parent.rotation.w * -1);

			// エフェクトのサイズを変える
			Vector3 effectSize;
			Vector3 puroperaSize = puropera.transform.localScale;
			if((z >= 0.6f && z <= 0.8f) && (w >= 0.6f && w <= 0.8f)) {
				effectSize = new Vector3(transform.localScale.y * 2, transform.localScale.y * 2);

				puroperaSize = new Vector3(transform.localScale.y, puroperaSize.y, puroperaSize.z);
			}
			else {
				effectSize = new Vector3(transform.localScale.x * 2, transform.localScale.x * 2);

				puroperaSize = new Vector3(transform.localScale.x, puroperaSize.y, puroperaSize.z);
			}

			particleShape.scale = effectSize;
			puropera.transform.localScale = puroperaSize;
		}

        void Update()
        {
            Vector2 vec = new Vector2(transform.position.x, transform.position.y);

            float dis = Vector2.Distance(transform.position, endPos.position);

            //メインカメラ上のマウスカーソルのある位置からRayを飛ばす
            Ray2D ray =  new Ray2D(vec,transform.up);

            int layerMask;

            if (isWallIgnore) layerMask = 1 << 10;
            else layerMask = 1 << 10 | 1 << 8 | 1 << 9;

			if(PhotonNetwork.InRoom) {

				var hits = Physics2D.BoxCastAll(ray.origin, transform.localScale, 0.0f, ray.direction, dis, layerMask);

				for(int i = 0;i < _isActivePlayerEffects.Length;i++) {
					_isActivePlayerEffects[i] = false;
				}

				foreach(var hit in hits) {
					//なにかと衝突した時だけそのオブジェクトの名前をログに出す
					if(!hit.collider) continue;
					Debug.DrawLine(transform.position, hit.collider.transform.position, Color.green);

					for(int i = 0;i < _referencePlayers.Length;i++) {
						if(_referencePlayers[i].Item1.gameObject == hit.collider.gameObject) {
							_isActivePlayerEffects[i] = true;
							break;
						}
					}
				}

				ActiveEffect();
			}
			else {
				RaycastHit2D hit = Physics2D.BoxCast(ray.origin, transform.localScale, 0.0f, ray.direction, dis, layerMask);

				//なにかと衝突した時だけそのオブジェクトの名前をログに出す
				if(hit.collider) {
					Debug.Log(hit.collider.gameObject.name);
					Debug.DrawLine(transform.position, hit.collider.transform.position, Color.green);
					_target = hit.collider.gameObject;

					ActiveEffect();
				}
			}
			


		}

		/// <summary>
		/// 風の影響を与える
		/// </summary>
		public void ActiveEffect() {
			if(PhotonNetwork.InRoom) {
				for(int i = 0;i < _referencePlayers.Length;i++) {
					if(!_isActivePlayerEffects[i]) {
						continue;
					}

					var player = _referencePlayers[i].Item1;
					var rigidbody = _referencePlayers[i].Item2;

					if(!player) continue;
					if(player.State != PlayerState.Circle) continue;
					if(!rigidbody) continue;

					float moveForceMultiplier = 2.0f;
					rigidbody.AddForce(moveForceMultiplier * (((Vector2)transform.up.normalized * windPower - rigidbody.velocity) * Time.deltaTime));

				}
			}
			else {


				if(_referencePlayers[0].Item1.gameObject != _target) return;
				if(_referencePlayers[0].Item1.State != PlayerState.Circle) return;

				Rigidbody2D _rg = _target.GetComponent<Rigidbody2D>();

				if(_rg == null) return;
				float moveForceMultiplier = 2.0f;
				_rg.AddForce(moveForceMultiplier * (((Vector2)transform.up.normalized * windPower - _rg.velocity) * Time.deltaTime));

			}
		}
    }
}