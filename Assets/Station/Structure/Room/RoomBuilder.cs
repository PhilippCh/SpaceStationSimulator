using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;
using System.Collections.Generic;

namespace SpaceStation.Station.Structure.Room {

	/**
	 * Provides functionality to construct rooms
	 */
	public class RoomBuilder : MonoBehaviour {

		/* TODO: Implement the actual method */
		public void BuildMockRoom() {
			Logger.Info("BuildMockRoom", "This should build a room.");

			RegionManager.Instance.UpdateRenderedCells(Vector2.zero);
		}

		/**
		 * Builds a room including floor and walls from a list of cells.
		 * Already occupied cells (CellType != EMPTY) will not be overwritten.
		 */
		public void BuildRoom(List<IntVector2> cells, int level) {

		}
	}

}