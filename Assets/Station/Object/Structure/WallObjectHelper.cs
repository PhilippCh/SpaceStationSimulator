using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;

namespace SpaceStation.Station.Structure.Cell {

	public class WallObjectHelper {

		public Dictionary<WallObject.WallType, CellMask> Masks;
		public Dictionary<WallObject.WallType, GameObject> Prefabs;

		public List<short> wall, floor, solid, any, empty;

		public WallObjectHelper() {
			var registry = GameRegistry.Instance;

			this.Masks = new Dictionary<WallObject.WallType, CellMask>();
			this.Prefabs = new Dictionary<WallObject.WallType, GameObject>();

			wall = new List<short>() { registry.GetObjectId<WallObject>() };
			floor = new List<short>() { registry.GetObjectId<FloorObject>() };
			empty = new List<short>() { GameRegistry.EmptyObjectId };
			any = new List<short>();

			AddWallType(WallObject.WallType.OUTER_DEFAULT, "Prefabs/wallOuter", new CellMask(
				any,	empty,	any, 
				wall,	wall,	wall,
				any,	floor,	any
			));

			AddWallType(WallObject.WallType.OUTER_EDGE_OUTER, "Prefabs/wallOuterEdgeOuter", new CellMask(
				any,	empty,	any, 
				empty,	wall,	wall,
				any,	wall,	any
			));

			AddWallType(WallObject.WallType.OUTER_EDGE_INNER, "Prefabs/wallOuterEdgeInner", new CellMask(
				any,	wall,	any, 
				wall,	wall,	floor,
				any,	floor,	any
			));
		}

		private void AddWallType(WallObject.WallType type, string prefabName, CellMask mask) {
			var prefab = Resources.Load(prefabName) as GameObject;

			if (prefab == null) {
				Logger.Warn("Could not load prefab {0}, will not add wall type {1}.", prefabName, type);
				return;
			}

			this.Prefabs.Add(type, prefab);
			this.Masks.Add(type, mask);
		}
	}
}

