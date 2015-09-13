using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Station.Object;

namespace SpaceStation.Game {

	public class GameInit : MonoBehaviour {

		private void Awake() {
			var registry = GameRegistry.Instance;

			registry.PostAwake();

			var save = new GameSave();
			save.Load();
		}
	}

}