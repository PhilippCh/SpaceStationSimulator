using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;

namespace SpaceStation.Station.Structure.Cell {

	public enum WallType {
		INVALID,
		
		OUTER_DEFAULT,
		OUTER_EDGE_OUTER,
		OUTER_EDGE_INNER,
		OUTER_EDGE_T_INNER,
		OUTER_EDGE_T_OUTER,

		INNER_DEFAULT,
		INNER_EDGE_INNER,
		INNER_EDGE_CROSS,
		INNER_EDGE_T,
		INNER_END
	}

	public class WallObjectHelper {

		public Dictionary<WallType, CellMask> Masks;
		public Dictionary<WallType, GameObject> Prefabs;

		public List<short> wall, floor, solid, any, empty;

		public WallObjectHelper() {
			var registry = GameRegistry.Instance;

			this.Masks = new Dictionary<WallType, CellMask>();
			this.Prefabs = new Dictionary<WallType, GameObject>();

			wall = new List<short>() { registry.GetObjectId<WallObject>(), registry.GetObjectId<DoorObject>() };
			floor = new List<short>() { registry.GetObjectId<FloorObject>() };
			empty = new List<short>() { GameRegistry.EmptyObjectId };
			any = new List<short>();

			/* Outer walls */

			AddWallType(WallType.OUTER_DEFAULT, "wallOuter", new CellMask(
				any,	empty,	any, 
				wall,	wall,	wall,
				any,	floor,	any
			));
			AddWallType(WallType.OUTER_EDGE_OUTER, "wallOuterEdgeOuter", new CellMask(
				any,	empty,	any, 
				empty,	wall,	wall,
				any,	wall,	any
			));
			AddWallType(WallType.OUTER_EDGE_INNER, "wallOuterEdgeInner", new CellMask(
				any,	wall,	any, 
				wall,	wall,	floor,
				any,	floor,	any
			));

			/* Inner walls */

			AddWallType(WallType.INNER_DEFAULT, "wallInner", new CellMask(
				any,	floor,	any, 
				wall,	wall,	wall,
				any,	floor,	any
			));
			AddWallType(WallType.INNER_END, "wallInnerEnd", new CellMask(
				any,	floor,	any, 
				floor,	wall,	wall,
				any,	floor,	any
			));
			AddWallType(WallType.INNER_EDGE_INNER, "wallInnerEdgeInner", new CellMask(
				floor,	wall,	any, 
				wall,	wall,	floor,
				any,	floor,	any
			));
			AddWallType(WallType.INNER_EDGE_T, "wallInnerEdgeT", new CellMask(
				floor,	wall,	floor, 
				wall,	wall,	wall,
				any,	floor,	any
			));
			AddWallType(WallType.INNER_EDGE_CROSS, "wallInnerEdgeCross", new CellMask(
				floor,	wall,	floor, 
				wall,	wall,	wall,
				floor,	wall,	floor
			));

			/* Outer/inner wall connections */

			AddWallType(WallType.OUTER_EDGE_T_INNER, "wallOuterEdgeTInner", new CellMask(
				any,	empty,	any, 
				wall,	wall,	wall,
				any,	wall,	any
			));
			AddWallType(WallType.OUTER_EDGE_T_OUTER, "wallOuterEdgeTOuter", new CellMask(
				floor,	floor,	any, 
				wall,	wall,	wall,
				floor,	wall,	empty
			));
		}

		private void AddWallType(WallType type, string prefabName, CellMask mask) {
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

