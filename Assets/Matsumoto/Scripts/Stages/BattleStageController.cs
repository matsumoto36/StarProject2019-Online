using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.SceneManagement;
using Matsumoto.Gimmick;
using Matsumoto.Audio;
using Matsumoto.Character;
using Matsumoto;

public class BattleStageController : MonoBehaviour, IStageController {

	public const string StageFollowerDataTarget = "_FollowerData";
	public const string HalfwayPointKey = "HalfWay";

	public event Action<IStageController> OnGameStart;
	public event Action<IStageController> OnGameClear;
	public event Action<IStageController> OnGameOver;

	public PlayerFollower PlayerFollowerPrefab;
	public PauseMenu PauseMenuCanvas;

	public bool IsCreateStage = true;
	public bool IsOverride = false;
	public bool IsReturnToSelect = true;

	public string StagePath = "TestStage";
	private string _followerDataKey;
	private List<GimmickChip> _gimmicks = new List<GimmickChip>();

	// 救出している最中のもの
	private HalfPointData _halfPointData = new HalfPointData();

	public bool CanPause {
		get; set;
	} = false;

	public GameState State {
		get; private set;
	} = GameState.StartUp;

	// すでに救出したもの
	private FollowerFindData _followerData = new FollowerFindData();
	public FollowerFindData FollowerData {
		get { return _followerData; }
	}

	private void Awake() {

		if(!IsOverride)
			GameData.Instance.GetData(StageSelectController.LoadSceneKey, ref StagePath);

		// ステージ生成
		CreateStage(StagePath);

		var followerChipIndex = FindObjectsOfType<FollowPlayerChip>()
			.Select(item => item.FollowerIndex)
			.ToArray();

		// フォロワーのデータ取得
		_followerDataKey = StagePath + StageFollowerDataTarget;
		GameData.Instance.GetData(_followerDataKey, ref _followerData);

		// ステージにないデータを削除
		_followerData.FindedIndexList = _followerData.FindedIndexList
			.Where(data => Array.Exists(followerChipIndex, x => x == data))
			.ToList();

		// 中間地点のデータ読み込み
		var player = FindObjectOfType<Player>();
		var delayAct = new Action(() => { });
		if(GameData.Instance.GetData(HalfwayPointKey, ref _halfPointData)) {

			delayAct = () => {
				player.transform.position = _halfPointData.PlayerPosition;
				FindObjectOfType<PlayerCamera>().SetTarget(player);
			};

			// 生成
			foreach(var item in _halfPointData.FollowerData.FindedIndexList) {
				var f = Instantiate(PlayerFollowerPrefab, _halfPointData.PlayerPosition, Quaternion.identity);
				f.Target = player;
			}

			// データを統合して取得したことにする
			_followerData.FindedIndexList.AddRange(_halfPointData.FollowerData.FindedIndexList);
		}

		// ギミック
		_gimmicks = FindObjectsOfType<GimmickChip>().ToList();
		foreach(var item in _gimmicks) {
			item.Controller = this;
			item.GimmickStart();
		}

		delayAct();

		PauseMenuCanvas.SetStageController(this);
		PauseMenuCanvas.gameObject.SetActive(false);

		// プレイヤーが死亡したらゲームオーバー
		FindObjectOfType<Player>().OnDeath += (_) => {
			GameOver();
		};

	}

	// Use this for initialization
	void Start() {

		// BGMを鳴らす
		AudioManager.FadeIn(1.0f, "Comet_Highway");

		GameStart();
	}

	// Update is called once per frame
	void Update() {

		if(Input.GetButtonDown("Menu") && CanPause) {
			PauseSystem.Instance.IsPause = !PauseSystem.Instance.IsPause;
			PauseMenuCanvas.gameObject.SetActive(!PauseMenuCanvas.gameObject.activeSelf);
		}

	}

	private void CreateStage(string stagePath) {

		if(!IsCreateStage) return;
		Instantiate(Resources.Load("Stages/" + stagePath));
	}

	public void GameStart() {

		Debug.Log("GameStart!");

		State = GameState.Playing;

		OnGameStart?.Invoke(this);

		CanPause = true;
	}

	public virtual void GameClear() {

		Debug.Log("GameClear!");

		State = GameState.GameClear;

		// 中間データを削除
		GameData.Instance.DeleteData(HalfwayPointKey);

		// フォロワーとクリアデータを保存
		GameData.Instance.SetData(_followerDataKey, _followerData);

		var clearedStages = new HashSet<string>();
		GameData.Instance.GetData(StageSelectController.StageProgressKey, ref clearedStages);
		clearedStages.Add(StagePath);
		GameData.Instance.SetData(StageSelectController.StageProgressKey, clearedStages);
		GameData.Instance.Save();

		OnGameClear?.Invoke(this);
		CanPause = false;

		if(IsReturnToSelect) {
			var player = FindObjectOfType<Player>();
			AudioManager.FadeOut(2.0f);
			AudioManager.PlaySE("GameClear", position: player.transform.position);
			SceneChanger.Instance.MoveScene("StageSelect", 2.0f, 1.0f, SceneChangeType.WhiteFade);
		}
	}

	public virtual void GameOver() {

		Debug.Log("GameOver!");
		State = GameState.GameOver;

		OnGameOver?.Invoke(this);
		CanPause = false;

		StartCoroutine(GameOverWait());
	}

	IEnumerator GameOverWait() {
		yield return new WaitForSeconds(1);

		// BGMを止める
		AudioManager.FadeOut(0.2f);

		var sceneName = SceneManager.GetActiveScene().name;
		SceneChanger.Instance.MoveScene(sceneName, 0.2f, 0.2f, SceneChangeType.BlackFade);
	}

	public void AddFollowerData(int index) {
		_followerData.FindedIndexList.Add(index);
		_halfPointData.FollowerData.FindedIndexList.Add(index);
	}

	public void SetHalfPoint(Vector3 position) {
		_halfPointData.PlayerPosition = position;
		GameData.Instance.SetData(HalfwayPointKey, _halfPointData);
	}
}