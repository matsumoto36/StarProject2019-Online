using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Matsumoto.Gimmick;
using Matsumoto.Audio;
using Matsumoto.Character;
using Matsumoto;

public enum GameState {
	StartUp,
	Playing,
	GameClear,
	GameOver,
}

public class DefaultStageController : MonoBehaviour, IStageController {

	public const string StageFollowerDataTarget = "_FollowerData";
	public const string HalfwayPointKey = "HalfWay";
	public const string DeathCountKey = "DeathCount";
	public const string IsActiveStatusKey = "StatusCanvas";
	public const string TimerKey = "Timer";

	public event Action<IStageController> OnGameStart;
	public event Action<IStageController> OnGameClear;
	public event Action<IStageController> OnGameOver;

	public Canvas StatusCanvas;
	public Text StatusText;

	public PlayerFollower PlayerFollowerPrefab;
	public PauseMenu PauseMenuCanvas;

	public bool IsCreateStage = true;
	public bool IsOverride = false;
	public bool IsReturnToSelect = true;

	public string StagePath = "TestStage";

	private float _timer;
	private int _deathCount;
	private bool _isStatusActive;
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
		if (GameData.Instance.GetData(HalfwayPointKey, ref _halfPointData)) {

			 delayAct = () => {
				 if(_halfPointData.IsHalfPointActive) {
					 player.transform.position = _halfPointData.PlayerPosition;
					 FindObjectOfType<PlayerCamera>().SetTarget(player);
				 }
			};

			// 生成
			foreach(var item in _halfPointData.FollowerData.FindedIndexList) {
				var f = Instantiate(PlayerFollowerPrefab, _halfPointData.PlayerPosition, Quaternion.identity);
				f.Target = player;
			}

			// データを統合して取得したことにする
			_followerData.FindedIndexList.AddRange(_halfPointData.FollowerData.FindedIndexList);
		}

		// DeathCount
		GameData.Instance.GetData(TimerKey, ref _timer);
		GameData.Instance.GetData(DeathCountKey, ref _deathCount);
		GameData.Instance.GetData(IsActiveStatusKey, ref _isStatusActive);
		StatusCanvas.gameObject.SetActive(_isStatusActive);

		// ギミック
		_gimmicks = FindObjectsOfType<GimmickChip>().ToList();
		foreach(var item in _gimmicks) {
			item.Controller = this;
			item.GimmickStart();
		}

		delayAct();

		PauseMenuCanvas.SetStageController(this);
		PauseMenuCanvas.gameObject.SetActive(false);

		// プレイヤーがゴールしたらゲームクリア
		FindObjectOfType<PlayerGoalChip>().OnGoalPlayer += (_) => {
			GameClear();
		};

		// プレイヤーが死亡したらゲームオーバー
		FindObjectOfType<Player>().OnDeath += (_) => {
			GameOver();
		};

	}

	// Use this for initialization
	void Start () {

		// BGMを鳴らす
		AudioManager.FadeIn(1.0f, "Comet_Highway");

		GameStart();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Menu") && CanPause) {
			PauseSystem.Instance.IsPause = !PauseSystem.Instance.IsPause;
			PauseMenuCanvas.gameObject.SetActive(!PauseMenuCanvas.gameObject.activeSelf);
		}

		if(!PauseSystem.Instance.IsPause && State == GameState.Playing) {
			_timer += Time.deltaTime;
		}

		if(Input.GetKeyDown(KeyCode.O)) {
			_isStatusActive = !_isStatusActive;
			StatusCanvas.gameObject.SetActive(_isStatusActive);
			GameData.Instance.SetData(IsActiveStatusKey, _isStatusActive);
		}

		if(_isStatusActive) {
			var minite = (int)(_timer / 60);
			var sec = (int)(_timer - minite * 60);
			var milli = (_timer - (int)(_timer)) * 100;

			StatusText.text =
				$"Time: {minite.ToString("00")}:{sec.ToString("00")}:{milli.ToString("00")}\r\n" +
				$"Death: {_deathCount}";

			if(State == GameState.GameClear) {
				StatusText.color = Color.white;
			}
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

	public void GameClear() {

		Debug.Log("GameClear!");

		State = GameState.GameClear;

		// 中間データを削除
		GameData.Instance.DeleteData(HalfwayPointKey);
		GameData.Instance.DeleteData(TimerKey);
		GameData.Instance.DeleteData(DeathCountKey);

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

		_deathCount++;
		GameData.Instance.SetData(DeathCountKey, _deathCount);
		GameData.Instance.SetData(TimerKey, _timer);

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
		_halfPointData.IsHalfPointActive = true;
		GameData.Instance.SetData(HalfwayPointKey, _halfPointData);
	}
}

namespace Matsumoto {

	[Serializable]
	public class HalfPointData {
		public float PlayerPositionX;
		public float PlayerPositionY;
		public float PlayerPositionZ;

		public Vector3 PlayerPosition {
			get { return new Vector3(PlayerPositionX, PlayerPositionY, PlayerPositionZ); }
			set {
				PlayerPositionX	= value.x;
				PlayerPositionY	= value.y;
				PlayerPositionZ = value.z;
			}
		}

		public FollowerFindData FollowerData = new FollowerFindData();

		public bool IsHalfPointActive;
	}

}