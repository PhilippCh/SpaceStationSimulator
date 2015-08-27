using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Game;
using SpaceStation.Util;
using System;

namespace SpaceStation.Station.Structure.Cell.Mask {

	public class WallMask {

		private GameRegistry registry;
		private List<short> wall, floor, any, empty;

		private Dictionary<WallObject.WallType, CellMask> masks;

		public CellMask this[WallObject.WallType i]
		{
			get { 
				return masks[i]; 
			}
		}

		public WallMask() {
			registry = GameRegistry.Instance;

			wall = new List<short>() { registry.GetObjectId<WallObject>() };
			floor = new List<short>() { registry.GetObjectId<FloorObject>() };
			empty = new List<short>() { GameRegistry.EmptyObjectId };
			any = new List<short>();

			masks = new Dictionary<WallObject.WallType, CellMask>();

			masks.Add(WallObject.WallType.OUTER_DEFAULT, new CellMask(
				any,	empty,	any, 
				wall,	wall,	wall,
				any,	floor,	any
			));
			masks.Add(WallObject.WallType.OUTER_EDGE_OUTER, new CellMask(
				any,	empty,	any, 
				empty,	wall,	wall,
				any,	wall,	any
			));
			masks.Add(WallObject.WallType.OUTER_EDGE_INNER, new CellMask(
				any,	wall,	any, 
				wall,	wall,	floor,
				any,	floor,	any
				));
		}
	}

}