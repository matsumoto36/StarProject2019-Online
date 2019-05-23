﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour {

	private const string StageBasePath = "Stages/";

	public string TargetStageName;

	private float _freq = 1.0f;
	private float _amp = 1.0f;
	private Transform _lab;
	private bool _isPlayAnim;

	[SerializeField]
	private StageNode _nextStage;
	public StageNode NextStage {
		get { return _nextStage; }
	}

	public StageNode PrevStage {
		get; private set;
	}

	public bool IsSelected {
		get; set;
	}

	public bool IsCleared {
		get; set;
	}

	private float _length = 0;
	public float Length {
		get {
			if(!NextStage) return 0;
			_length = (NextStage.transform.position - transform.position).magnitude;
			return _length;
		}
		private set { _length = value; }
	}

	// Use this for initialization
	public void SetUpNode(StageNode prevStage, int clearedCount) {

		if(transform.childCount > 0)
			_lab = transform.GetChild(0);

		IsCleared = clearedCount > 0;

		PrevStage = prevStage;

		// 簡単にクリア状態を表示
		var spr = GetComponentInChildren<SpriteRenderer>();
		if(spr) {
			spr.color = IsCleared ? Color.white : Color.gray;
		}

		if(NextStage) {
			NextStage.SetUpNode(this, --clearedCount);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(IsSelected && _lab && !_isPlayAnim) {
			StartCoroutine(SelectedAnim());
		}

	}

	private IEnumerator SelectedAnim() {
		_isPlayAnim = true;

		var t = 0.0f;

		while(t < 0.5f) {

			t = Mathf.Min(t + Time.deltaTime * _freq, 0.5f);
			_lab.transform.localScale = Vector3.one * (1 + Mathf.Sin(t * 2 * Mathf.PI) * _amp);
			yield return null;
		}

		if(IsSelected && _lab && !_isPlayAnim) {
			StartCoroutine(SelectedAnim());
		}
		else {
			_isPlayAnim = false;
		}
	}

	public void OnDrawGizmos() {
		if(!NextStage) return;

		Gizmos.color = Color.white;
		Gizmos.DrawSphere(transform.position, 0.1f);
		Gizmos.DrawLine(transform.position, NextStage.transform.position);

		var arrowLength = 0.2f;
		var deg = 30.0f;
		var vec = (transform.position - NextStage.transform.position).normalized;

		Gizmos.DrawLine(NextStage.transform.position, NextStage.transform.position + Quaternion.AngleAxis(deg, Vector3.forward) * vec * arrowLength);
		Gizmos.DrawLine(NextStage.transform.position, NextStage.transform.position + Quaternion.AngleAxis(-deg, Vector3.forward) * vec * arrowLength);
	}
}
