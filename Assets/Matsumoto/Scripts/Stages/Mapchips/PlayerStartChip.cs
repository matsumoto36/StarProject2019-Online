using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class PlayerStartChip : GimmickChip {

		public string TargetPlayerName = "Player";

		public override void GimmickStart() {

			var player = transform
				.Find(TargetPlayerName)
				.GetComponent<Player>();

			player.transform.position = transform.position;

			Destroy(gameObject);
		}
	}
}