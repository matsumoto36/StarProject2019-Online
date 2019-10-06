using UnityEngine;
using System.Collections;
using Matsumoto.Character;

namespace Matsumoto {
	public class BattleScoreViewer : MonoBehaviour {

		public GameObject FollowerPrefab;
		public GameObject FollowerChipPrefab;

		public Canvas ScoreCanvas;

		public Transform[] ScoreAnchors;
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

		public void ShowScore(Player[] Players, int[] PlayerScores, int maxScore, int winnerID) {

			ScoreCanvas.gameObject.SetActive(true);

			for(int i = 0;i < Players.Length;i++) {
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

				SetColor(playerBody, Players[i].StarStatus.BodyColor);
				player.transform.localPosition = new Vector3();
				player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

				// スコア表示
				for(int j = 0;j < maxScore;j++) {
					GameObject scoreFollower;
					GameObject scoreFollowerBody;
					var scoreParent = ScoreAnchors[i].Find("Score");
					if(j >= PlayerScores[i]) {
						// 取得していないスコア
						scoreFollowerBody = scoreFollower = Instantiate(_grayFollower, scoreParent);
						scoreFollower.SetActive(true);
					}
					else {
						// 取得したスコア
						scoreFollower = Instantiate(FollowerChipPrefab, scoreParent);
						scoreFollowerBody = scoreFollower.transform.GetChild(0).gameObject;
						SetColor(scoreFollowerBody, Players[i].StarStatus.BodyColor);
					}

					if(ScoreDirections[i] < 0) {
						FlipEye(scoreFollowerBody);
					}

					scoreFollower.transform.localPosition =
						new Vector3(ScoreDirections[i] * j, 0.0f, 0.0f);
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


