using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

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

		Player[] _referencePlayers;

        [SerializeField] ParticleSystem particle;

        ParticleSystem.ShapeModule particleShape;

        //--------------------------------
        // 関数
        //--------------------------------

        void Start()
        {

			_referencePlayers = FindObjectsOfType<Player>();

			particleShape = particle.shape;

            float z = (transform.parent.rotation.z > 0 ? transform.parent.rotation.z : transform.parent.rotation.z * -1);
            float w = (transform.parent.rotation.w > 0 ? transform.parent.rotation.w : transform.parent.rotation.w * -1);

            // エフェクトのサイズを変える
            Vector3 effectSize;
            Vector3 puroperaSize = puropera.transform.localScale;
            if ((z >= 0.6f && z <= 0.8f) && (w >= 0.6f && w <= 0.8f))
            {
                effectSize = new Vector3(transform.localScale.y * 2, transform.localScale.y * 2);

                puroperaSize = new Vector3(transform.localScale.y, puroperaSize.y, puroperaSize.z);
            }
            else
            {
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

            var hits = Physics2D.BoxCastAll(ray.origin,transform.localScale,0.0f, ray.direction,dis,layerMask);

			foreach(var hit in hits) {
				//なにかと衝突した時だけそのオブジェクトの名前をログに出す
				if(hit.collider) {
					Debug.Log(hit.collider.gameObject.name);
					Debug.DrawLine(transform.position, hit.collider.transform.position, Color.green);

					//TODO
					_target = hit.collider.gameObject;
					if(_chachedPlayer.gameObject != _target) {
						_chachedPlayer = _target.GetComponent<Player>();
					}


					ActiveEffect();
				}
			}

 
        }

        /// <summary>
        /// 風の影響を与える
        /// </summary>
        public void ActiveEffect()
        {

			if(!_chachedPlayer) return;
			if(_chachedPlayer.State != PlayerState.Circle) return;
			if(!_chachedRig) return;

			float moveForceMultiplier = 2.0f;
			_chachedRig.AddForce(moveForceMultiplier * (((Vector2)transform.up.normalized * windPower * Time.deltaTime) - _chachedRig.velocity));

        }
    }
}