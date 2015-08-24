using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;

namespace SpaceStation.Station.Structure.Cell {

	public class FloorDefinition {

		public Texture2D Texture;

		private GameObject floorPrefab;
		private GameObject goReference;

		public void Update(IntVector3 position) {
			PreloadPrefabs();

			/* Spawn the new floor object and set transform */
			this.goReference = floorPrefab.Spawn();

			this.goReference.transform.position = position.ToVector3();
			this.goReference.transform.eulerAngles = floorPrefab.transform.eulerAngles;
		}

		private void PreloadPrefabs() {
			if (this.floorPrefab != null) {
				return;
			}

			this.floorPrefab = Resources.Load("Prefabs/floor") as GameObject;
		}
	}

}