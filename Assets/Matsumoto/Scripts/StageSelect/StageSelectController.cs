using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Matsumoto.Audio;

public class StageSelectController : MonoBehaviour {

	public const string LoadSceneKey = "LoadScene";
	public const string StageProgressKey = "StageProgress";

	private static int _lastSelectedStageIndex = -1;

	public StageNode FirstNode;
	public StageNode TitleNode;
	public Transform PlayerModel;

	public float MoveSpeed;
	public float CursorWait = 1.0f;
	public float Dead = 0.3f;

	private StageNode _currentSelectedStage;
	private StageNode _targetStage;
	private float _playerPositionTarget;
	private Transform _playerBody;
	private Transform _playerEye;
	private float _moveSpeedMag = 1;
	private List<IStageMoveEvent> _eventList;
	private bool _isSceneMoving;

	public float Position;

	public bool IsFreeze {
		get; set;
	}

	// Use this for initialization
	void Start () {

		_playerBody = PlayerModel.Find("Body").Find("StarWithBone");
		_playerEye = PlayerModel.Find("Eye").Find("eye");

		// イベント読み込み
		_eventList = GetComponentsInChildren<MonoBehaviour>()
			.Select(x => x as IStageMoveEvent)
			.Where(x => x != null)
			.OrderBy(x => x.GetPosition())
			.ToList();

		GameData.Instance.Load();

		// 進行度読み込み
		var clearedStages = new HashSet<string>();
		GameData.Instance.GetData(StageProgressKey, ref clearedStages);

		var stageProgress = clearedStages.Count;
		var followerCount = 0;

		var titleNodeIndex = GetStageIndex(TitleNode);

		if(_lastSelectedStageIndex == -1) {
			_lastSelectedStageIndex = titleNodeIndex;
		}

		// ステージノードのセットアップ(+1はタイトル)
		FirstNode.SetUpNode(null, titleNodeIndex + stageProgress + 1, ref followerCount);

		_targetStage = _currentSelectedStage = GetStageNode(_lastSelectedStageIndex);

		_playerPositionTarget = GetLength(_targetStage);

		// イベントを実行
		foreach(var item in _eventList) {
			if(_playerPositionTarget > item.GetPosition()) {
				Debug.Log(_playerPositionTarget + " " + item.GetPosition());
				item.OnExecute(this, true, true);
			}
		}

		// プレイヤーを移動
		MovePlayer(_playerPositionTarget);
		Position = _playerPositionTarget;

		// BGMを鳴らす
		AudioManager.FadeIn(1.0f, "vigilante");

		// ステージ選択用
		StartCoroutine(SelectionInput());
	}
	
	// Update is called once per frame
	void Update () {

		if(IsFreeze) return;

		if(Input.GetButtonDown("Attack")) {
			MoveScene();
		}

		var p = Mathf.MoveTowards(Position, _playerPositionTarget, MoveSpeed * _moveSpeedMag * Time.deltaTime);
		MovePlayer(p);

		var forward = p >= Position;
		var low = forward ? Position : p;
		var high = forward ? p : Position;
		foreach(var e in _eventList) {
			var c = e.GetPosition();
			if(low <= c && c <= high) {
				e.OnExecute(this, forward, false);
			}
		}

		if(Input.GetKeyDown(KeyCode.P) && !_isSceneMoving) {
			_isSceneMoving = true;
			_lastSelectedStageIndex = GetStageIndex(TitleNode);
			GameData.Instance.DeleteDataAll();
			GameData.Instance.Save();
			SceneChanger.Instance.MoveScene("StageSelect", 0.2f, 0.2f, SceneChangeType.BlackFade);
		}

		if(p == Position) {
			// ノードに到着した
			_currentSelectedStage = _targetStage;
			_currentSelectedStage.IsSelected = true;

			// 自動で移動するノードであれば移動開始
			if(_currentSelectedStage.NodeType == NodeType.AutoSelect) {
				MoveScene();
			}
		}
		else {
			if(_currentSelectedStage)
				_currentSelectedStage.IsSelected = false;

			_currentSelectedStage = null;
		}

		Position = p;
	}

	private void MoveScene() {
		if(!_currentSelectedStage) return;
		if(_currentSelectedStage.NodeType == NodeType.None) return;
		if(_currentSelectedStage.NodeType == NodeType.Skip) return;
		if(_isSceneMoving) return;
		_isSceneMoving = true;

		Debug.Log("MoveScene");

		// ステージ用BGM
		AudioManager.FadeOut(1.0f);

		_lastSelectedStageIndex = GetStageIndex(_currentSelectedStage);
		GameData.Instance.SetData(LoadSceneKey, _currentSelectedStage.TargetStageName);
		_isSceneMoving = _currentSelectedStage.MoveScene();
	}

	private void MovePlayer(float position) {
		var current = FirstNode;

		while(true) {
			position -= current.Length;
			if(position < 0) break;
			if(!current.NextStage) {
				position = 0;
				break;
			}

			current = current.NextStage;
		}

		position *= -1;

		var nextPos = new Vector3();
		var angle = 0.0f;

		if(position == 0) {
			nextPos = current.transform.position;
		}
		else {
			var ratio = 1 - position / current.Length;
			nextPos = Vector3.Lerp(current.transform.position, current.NextStage.transform.position, ratio);

			angle = (1 - ratio) * Mathf.Floor(current.Length) * 360f;
		}

		PlayerModel.transform.position = nextPos;
		_playerBody.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private StageNode GetStageNode(int number) {

		var current = FirstNode;
		for(int i = 0;i < number;i++) {
			if(!current.NextStage) break;
			current = current.NextStage;
		}

		return current;
	}

	private int GetStageIndex(StageNode stage) {

		if(!stage) return 0;

		var current = FirstNode;
		var count = 0;
		while(current != stage) {
			if(!current.NextStage) break;
			current = current.NextStage;
			count++;
		}

		return count;
	}

	private float GetLength(StageNode to) {

		var length = 0.0f;
		var current = FirstNode;
		while(current != to) {
			if(!current.NextStage) break;
			length += current.Length;
			current = current.NextStage;
		}

		return length;
	}

	private void SetTarget(StageNode node) {
		_targetStage = node;
		_playerPositionTarget = GetLength(_targetStage);
		var diff = _playerPositionTarget - Position;
		_moveSpeedMag = 1 + Mathf.Abs(diff) * 0.8f;
		_playerEye.transform.localScale = new Vector3(Mathf.Sign(diff), 1, 1);
	}

	public Vector3 GetPosition(float position) {

		var current = FirstNode;

		while(true) {
			position -= current.Length;
			if(position < 0) break;
			if(!current.NextStage) {
				position = 0;
				break;
			}

			current = current.NextStage;
		}

		position *= -1;

		var pos = new Vector3();

		if(position == 0) {
			pos = current.transform.position;
		}
		else {
			var ratio = 1 - position / current.Length;
			pos = Vector3.Lerp(current.transform.position, current.NextStage.transform.position, ratio);
		}
		return pos;

	}

	IEnumerator SelectionInput() {

		var t = 0.0f;

		while(true) {

			t = Mathf.MoveTowards(t, 0.0f, Time.deltaTime);

			if(Input.GetAxisRaw("Horizontal") < -Dead && t <= 0.0f) {
				t = CursorWait;

				StageNode prev = _targetStage.PrevStage;
				while(prev && prev.NodeType == NodeType.Skip) {
					prev = prev.PrevStage;
				}

				if (prev) {
					AudioManager.PlaySE("MenuSelect", position: prev.transform.position);
					SetTarget(prev);
				}
			}
			if(Input.GetAxisRaw("Horizontal") > Dead && t >= 0.0f) {
				t = -CursorWait;

				StageNode next = _targetStage.NextStage;
				while(next && next.NodeType == NodeType.Skip) {
					next = next.NextStage;
				}

				if (next && _targetStage.IsCleared) {
					AudioManager.PlaySE("MenuSelect", position: next.transform.position);
					SetTarget(next);
				}
			}

			yield return null;
		}
	}
}
