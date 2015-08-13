using UnityEngine;
using System.Collections;

using SpaceStation;

namespace SpaceStation.Station.Structure.Cell {
	
	public enum CellType {
		INVALID = -1,
		EMPTY = 0,

		FLOOR,

		WALL_N,
		WALL_E,
		WALL_S,
		WALL_W,

		WALL_EDGE_N,
		WALL_EDGE_E,
		WALL_EDGE_S,
		WALL_EDGE_W,
	}

}