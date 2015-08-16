﻿using UnityEngine;
using System.Collections;

using SpaceStation;

namespace SpaceStation.Station.Structure.Cell {
	
	public enum CellType {
		INVALID = -1,
		EMPTY,

		FLOOR,

		WALL_NORTH,
		WALL_EAST,
		WALL_SOUTH,
		WALL_WEST,

		WALL_EDGE_NORTHEAST,
		WALL_EDGE_NORTHWEST,
		WALL_EDGE_SOUTHEAST,
		WALL_EDGE_SOUTHWEST,
		WALL_EDGE_T,

		WALL_OUTER_NORTH,
		WALL_OUTER_EAST,
		WALL_OUTER_SOUTH,
		WALL_OUTER_WEST,

		WALL_OUTER_EDGE_NORTHEAST,
		WALL_OUTER_EDGE_NORTHWEST,
		WALL_OUTER_EDGE_SOUTHEAST,
		WALL_OUTER_EDGE_SOUTHWEST,
	}

}