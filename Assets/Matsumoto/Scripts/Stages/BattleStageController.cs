using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Matsumoto.Gimmick;
using Matsumoto.Audio;
using Matsumoto.Character;
using Matsumoto;
using Photon.Pun;

public class BattleStageController : MonoBehaviourPunCallbacks, IStageController {

	public event Action<IStageController> OnGameStart;
	public event Action<IStageController> OnGameClear;
	public event Action<IStageController> OnGameOver;

	public BattleScoreViewer Viewer;

	public bool IsCreateStage = true;
	public bool IsOverride = false;
	public bool IsReturnToSelect = true;

	public string StagePath = "TestStage";
	private List<GimmickChip> _gimmicks = new List<GimmickChip>();

	private Saitou.Online.OnlineManager _onlineManager;
    private PlayerCamera _playerCamera;

	private Player[] _players;
	private bool[] _deathPlayers;

    private PhotonView _view;

	public bool CanPause {
		get; set;
	} = false;

	public GameState State {
		get; private set;
	} = GameState.StartUp;

	private void Awake() {

		if(!IsOverride)
			GameData.Instance.GetData(StageSelectController.LoadSceneKey, ref StagePath);

		// ステージ生成
		CreateStage(StagePath);

		// ギミック
		_gimmicks = FindObjectsOfType<GimmickChip>().ToList();
		foreach(var item in _gimmicks) {
			item.Controller = this;
			item.GimmickStart();
		}


		//PauseMenuCanvas.SetStageController(this);
		//PauseMenuCanvas.gameObject.SetActive(false);

		_players = FindObjectsOfType<Player>()
			.OrderBy(item => item.GetComponent<Saitou.Online.OnlineState>()._PlayerList)
			.ToArray();

		_deathPlayers = new bool[_players.Length];

		for(int i = 0;i < _players.Length;i++) {
			var playerID = i;
			_players[playerID].OnDeath += (_) => {
                _view.RPC(nameof(Death),RpcTarget.All,playerID);
            };
		}

        var player = FindObjectsOfType<Saitou.Online.OnlineState>();
        _playerCamera = FindObjectOfType<PlayerCamera>();

        for (int i = 0; i < player.Length; i++)
        {
            if (OnlineData.PlayerID == (int)player[i]._PlayerList)
            {
                Player p = player[i].GetComponent<Player>();
                _playerCamera.TargetPlayer = p;
                p.GetComponent<AudioListener>().enabled = true;
                break;
            }
        }

       _onlineManager = FindObjectOfType<Saitou.Online.OnlineManager>();
        _view = GetComponent<PhotonView>();
	}

	// Use this for initialization
	void Start() {

		// BGMを鳴らす
		AudioManager.FadeIn(1.0f, "Comet_Highway");

		GameStart();
	}

	// Update is called once per frame
	void Update() {

		//if(Input.GetButtonDown("Menu") && CanPause) {
		//	PauseSystem.Instance.IsPause = !PauseSystem.Instance.IsPause;
		//	PauseMenuCanvas.gameObject.SetActive(!PauseMenuCanvas.gameObject.activeSelf);
		//}

		if(Input.GetKeyDown(KeyCode.P)) {
			MoveLobby();
		}

	}

    [PunRPC]
    void Death(int playerID)
    {
        Debug.Log("DeathPlayer " + playerID);
        _deathPlayers[playerID] = true;
        _players[playerID].IsFreeze = true;

        if(!PhotonNetwork.IsMasterClient)
        {
            _players[playerID].ApplyDamage(_players[playerID].gameObject, DamageType.Enemy);
        }

        var count = 0;
        var leaveID = -1;
        foreach (var item in _deathPlayers)
        {
            if (!item)
            {
                leaveID = count;
            }
            else
            {
                count++;
            }
        }
        if (count <= 1)
        {
            StartCoroutine(Result(leaveID));
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

	private IEnumerator Result(int winPlayerId) {

		Debug.Log("WinPlayer " + winPlayerId);

		for(int i = 0;i < _players.Length;i++) {
			if(i == winPlayerId) continue;
			if(!_players[i].IsDeath) {
				_players[i].ApplyDamage(gameObject, DamageType.Gimmick);
			}
		}

		// BGMを止める
		AudioManager.FadeOut(0.2f);

		_players[winPlayerId].IsFreeze = true;

		// TODO 得点の設定と一時保存
		var points = new int[_players.Length];
		GameData.Instance.GetData("BattlePoints", ref points);

		yield return new WaitForSeconds(1.0f);
		yield return Viewer.ShowScore(_players, points, 3, winPlayerId);
		yield return new WaitForSeconds(1.0f);

		//points[winPlayerId]++;

		var sceneName = SceneManager.GetActiveScene().name;

		//if(points[winPlayerId] >= 3) {
		//	// 勝利
		//	Viewer.ShowWinner(_players, winPlayerId);
		//	yield return new WaitForSeconds(5.0f);
		//	points = new int[_players.Length];
		//	GameData.Instance.SetData("BattlePoints", points);
		//	MoveLobby();

		//	yield break;
		//}

		GameData.Instance.SetData("BattlePoints", points);
		SceneChanger.Instance.MoveScene(sceneName, 0.2f, 0.2f, SceneChangeType.BlackFade, true);
	}

	[ContextMenu("ShowScore")]
	public void ShowResult() {
		Viewer.ShowScore(_players, new int[] { 1, 2 }, 3, 0);
	}

	private void MoveLobby() {
		// 退出
		FindObjectOfType<Saitou.Online.OnlineConnect>()
			.LeaveRoom();

		SceneChanger.Instance.MoveScene("LobbyScene", 0.2f, 0.2f, SceneChangeType.BlackFade);
	}
}