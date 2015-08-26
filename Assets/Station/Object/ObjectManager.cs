using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;
using SpaceStation.Game;

namespace SpaceStation.Station.Object {
	
	public class ObjectManager : MonoBehaviour {

		private GameRegistry registry;

		private void Awake() {
			registry = GameRegistry.Instance;

			/* Register objects on startup */
			registry.RegisterObject<WallObject>(0);
			registry.RegisterObject<FloorObject>(1);
		}
	}
	
}