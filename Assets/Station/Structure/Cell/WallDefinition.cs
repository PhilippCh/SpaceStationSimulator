using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;

namespace SpaceStation.Station.Structure.Cell {

	public class WallDefinition {

		public enum WallType {
			OUTER_DEFAULT,
			OUTER_EDGE
		}

		private enum NeighborType {
			EMPTY,
			FLOOR,
			WALL
		}

		public Rotation Rotation;
		public WallType Type;

		private static Dictionary<WallType, GameObject> prefabs;

		private GameObject goReference;

		public void Update(IntVector3 position) {
			PreloadPrefabs();

			if (!prefabs.ContainsKey(this.Type)) {
				Logger.Warn("CreateAt", "Could not load prefab for type {0}.", Type);
				return;
			}

			var neighborCells = new CellDefinition[3, 3];

			/* Grab surrounding block information */
			LoopHelper.IntXZ(position.ToIntVector2() - 1, position.ToIntVector2() + 1, 1, (x, z) => {
				var relativePos = new IntVector2(x, z) - (position.ToIntVector2() - 1);

				neighborCells[relativePos.x, relativePos.z] = RegionManager.Instance.GetCellAt(x, position.y, z);
			});
							
			/* Spawn the new wall object and set transform */
			var wallObject = prefabs[this.Type].Spawn();
			var yRotation = RotationHelper.GetEulerAngle(this.Rotation);
			var rotation = prefabs[this.Type].transform.eulerAngles;

			rotation.y = RotationHelper.GetEulerAngle(this.Rotation);

			wallObject.transform.position = position.ToVector3();
			wallObject.transform.eulerAngles = rotation;
							
			this.goReference = wallObject;
		}

		private void PreloadPrefabs() {
			if (prefabs != null) {
				return;
			}

			Logger.Info("WallObject", "Loading variant prefabs.");

			prefabs = new Dictionary<WallType, GameObject>();
							
			prefabs.Add(WallType.OUTER_DEFAULT, Resources.Load("Prefabs/wallOuter") as GameObject);
			prefabs.Add(WallType.OUTER_EDGE, Resources.Load("Prefabs/wallEdgeOuter") as GameObject);

		}
	}

}