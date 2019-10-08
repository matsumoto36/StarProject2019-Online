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

public class LobbyStageController : MonoBehaviour, IStageController {

	public event Action<IStageController> OnGameStart;
	public event Action<IStageController> OnGameClear;
	public event Action<IStageController> OnGameOver;

	public LobbyButtonController ButtonController;

	private List<GimmickChip> _gimmicks = new List<GimmickChip>();

	public bool CanPause {
		get; set;
	} = false;

	public GameState State {
		get; private set;
	} = GameState.StartUp;

	private void Awake() {

		// ギミック
		_gimmicks = FindObjectsOfType<GimmickChip>().ToList();
		foreach(var item in _gimmicks) {
			item.Controller = this;
			item.GimmickStart();
		}

		ButtonController.OnBackButtonClick += () => {
			AudioManager.FadeOut(1.0f);
			SceneChanger.Instance.MoveScene("StageSelect", 1.0f, 1.0f, SceneChangeType.BlackFade);
		};

		// プレイヤーがゴールしたらゲームクリア
		FindObjectOfType<PlayerGoalChip>().OnGoalPlayer += (_) => {
			BackToSelectScene();
		};
	}

	// Use this for initialization
	void Start () {

		// BGMを鳴らす
		AudioManager.FadeIn(1.0f, "vigilante");

		ButtonController.OnCreateRoomButtonClick += () => {
			GameStart();
			ButtonController.IsActive = false;
		};

		ButtonController.OnInRoomButtonClick += () => {
			GameStart();
			ButtonController.IsActive = false;
		};

	}

	public void GameStart() {

		Debug.Log("GameStart!");

		State = GameState.Playing;

		OnGameStart?.Invoke(this);

		CanPause = true;
	}

	public void BackToSelectScene() {
        ButtonController.CallBackButtonClick();
	}
}
