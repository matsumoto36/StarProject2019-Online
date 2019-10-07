using UnityEngine;
using System.Collections;
using Matsumoto.Character;

namespace Matsumoto {
	public class BattleScoreViewer : MonoBehaviour {

		public GameObject FollowerPrefab;
		public GameObject FollowerChipPrefab;
		public GameObject AddScoreEffect;
		public GameObject LoseEffect;

		public Canvas ScoreCanvas;

		public Transform[] ScoreAnchors;
		public Transform[] ScoreCanvasAnchors;
		public int[] ScoreDirections;

		private GameObject _grayFollower;

		private void Awake() {
			_grayFollower = Instantiate(FollowerPrefab);
			// 灰色にする
			var renders = _grayFollower.GetComponentsInChildren<Renderer>();
			foreach(var item in renders) {
				item.material.EnableKeyword("EMISSION");
				item.material.SetColor("_EmissionColor", new Color(.25f, .25f, .25f));
			}
			_grayFollower.SetActive(false);

			ScoreCanvas.gameObject.SetActive(false);
		}

		public IEnumerator ShowScore(Player[] players, int[] oldPlayerScores, int maxScore, int winnerID) {

			ScoreCanvas.gameObject.SetActive(true);
			GameObject tradeObject = null;

			for(int i = 0;i < players.Length;i++) {
				// プレイヤー表示
				GameObject player;
				GameObject playerBody;
				var parent = ScoreAnchors[i].Find("PlayerAnchor");
				if(i == winnerID) {
					player = Instantiate(FollowerChipPrefab, parent);
					playerBody = player.transform.GetChild(0).gameObject;
				}
				else {
					playerBody = player = Instantiate(FollowerPrefab, parent);
				}

				if(ScoreDirections[i] < 0) {
					FlipEye(playerBody);
				}

				SetColor(playerBody, players[i].StarStatus.BodyColor);
				player.transform.localPosition = new Vector3();
				player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


				// スコア表示
				for(int j = 0;j < maxScore;j++) {
					GameObject scoreFollower;
					GameObject scoreFollowerBody;
					var scoreParent = ScoreAnchors[i].Find("Score");
					if(j >= oldPlayerScores[i]) {
						// 取得していないスコア
						scoreFollowerBody = scoreFollower = Instantiate(_grayFollower, scoreParent);
						scoreFollower.SetActive(true);

						if(i == winnerID && !tradeObject) {
							tradeObject = scoreFollowerBody;
						}
					}
					else {
						// 取得したスコア
						scoreFollower = Instantiate(FollowerChipPrefab, scoreParent);
						scoreFollowerBody = scoreFollower.transform.GetChild(0).gameObject;
						SetColor(scoreFollowerBody, players[i].StarStatus.BodyColor);
					}

					if(ScoreDirections[i] < 0) {
						FlipEye(scoreFollowerBody);
					}

					scoreFollower.transform.localPosition =
						new Vector3(ScoreDirections[i] * j, 0.0f, 0.0f);
				}
			}

			yield return StartCoroutine(AddScoreAnimation(players[winnerID], winnerID, tradeObject));
		}

		private IEnumerator AddScoreAnimation(Player player, int playerID, GameObject tradeObject) {

			yield return new WaitForSeconds(2.0f);

			// モデル入れ替え
			var position = tradeObject.transform.position;

			var score = Instantiate(FollowerChipPrefab, tradeObject.transform.parent);
			var scoreBody = score.transform.GetChild(0).gameObject;
			var e = Instantiate(AddScoreEffect, tradeObject.transform.position, Quaternion.identity);
			var effect = e.transform.GetChild(2).GetComponent<ParticleSystem>().main;
			effect.startColor = player.StarStatus.BodyColor;

			var renderers = e.transform.GetComponentsInChildren<ParticleSystemRenderer>();
			foreach(var item in renderers) {
				item.material = Instantiate(item.material);
				item.material.shader = Shader.Find("Particles/Alpha Blended RenderTexture");
			}

			Destroy(e, 5.0f);

			Audio.AudioManager.PlaySE("AttackHit_3", position: player.transform.position);

			score.transform.localPosition =
				tradeObject.transform.localPosition;

			SetColor(scoreBody, player.StarStatus.BodyColor);

			if(ScoreDirections[playerID] < 0) {
				FlipEye(scoreBody);
			}

			Destroy(tradeObject);
		}
		
		public void ShowWinner(Player[] players, int playerID) {

			ScoreCanvasAnchors[playerID]
				.Find("Win")
				.gameObject
				.SetActive(true);

			for(int i = 0;i < players.Length;i++) {

				if(i == playerID) {
					continue;
				}

				var anchor = ScoreAnchors[i].Find("PlayerAnchor");

				var e = Instantiate(LoseEffect, anchor.position, Quaternion.identity);
				foreach(var item in e.GetComponentsInChildren<ParticleSystem>()) {
					var effect = item.main;
					effect.startColor = players[i].StarStatus.BodyColor;
				}

				Destroy(e, 5.0f);
				Destroy(anchor.GetChild(0).gameObject);

				var renderers = e.GetComponentsInChildren<ParticleSystemRenderer>();
				foreach(var item in renderers) {
					item.material = Instantiate(item.material);
					item.material.shader = Shader.Find("Particles/Alpha Blended RenderTexture");
				}
			}

		}

		private void SetColor(GameObject target, Color color) {
			var body = target.transform.Find("Body").GetComponent<SpriteRenderer>();
			var bodyWithBone = body.transform.Find("StarWithBone").GetComponent<SpriteRenderer>();
			var bodyMaterial = Instantiate(body.material);
			body.material = bodyMaterial;
			bodyWithBone.material = bodyMaterial;
			bodyMaterial.EnableKeyword("_EMISSION");
			bodyMaterial.SetColor("_EmissionColor", color);
		}

		private void FlipEye(GameObject body) {
			var eye = body.transform.Find("Eye").Find("eye");
			eye.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
		}

	}
}


