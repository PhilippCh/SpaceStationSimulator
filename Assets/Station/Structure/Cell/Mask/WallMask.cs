﻿using UnityEngine;
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

		public void Initialize() {
			if (masks != null) {
				return;
			}

			registry = GameRegistry.Instance;

			wall = new List<short>() { registry.GetObjectId<WallObject>() };
			floor = new List<short>() { registry.GetObjectId<FloorObject>() };
			empty = new List<short>() { GameRegistry.EmptyId };
			any = new List<short>();

			masks = new Dictionary<WallObject.WallType, CellMask>();

			masks.Add(WallObject.WallType.OUTER_DEFAULT, new CellMask(
				any,	empty,	any, 
				wall,	wall,	wall,
				any,	any,	any
			));
			masks.Add(WallObject.WallType.OUTER_EDGE, new CellMask(
				any,	empty,	any, 
				empty,	wall,	wall,
				any,	wall,	any
				));
		}
	}

}