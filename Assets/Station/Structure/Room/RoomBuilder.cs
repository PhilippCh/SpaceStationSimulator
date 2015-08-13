using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Structure.Room {

	/**
	 * Provides functionality to construct rooms
	 */
	public class RoomBuilder : MonoBehaviour {

		/* TODO: Implement the actual method */
		public void BuildMockRoom() {
			Logger.Info("BuildMockRoom", "This should build a room.");

			RegionManager.Instance.SetCellAt(new IntVector3(1, 0, 1), CellType.FLOOR);
			RegionManager.Instance.SetCellAt(new IntVector3(1, 0, 2), CellType.FLOOR);
			RegionManager.Instance.SetCellAt(new IntVector3(2, 0, 1), CellType.FLOOR);
			RegionManager.Instance.SetCellAt(new IntVector3(2, 0, 2), CellType.FLOOR);

			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 0), CellType.WALL_OUTER_EDGE_NORTHEAST);
			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 3), CellType.WALL_OUTER_EDGE_SOUTHEAST);
			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 0), CellType.WALL_OUTER_EDGE_NORTHWEST);
			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 3), CellType.WALL_OUTER_EDGE_SOUTHWEST);

			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 1), CellType.WALL_OUTER_EAST);
			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 2), CellType.WALL_OUTER_EAST);

			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 1), CellType.WALL_OUTER_WEST);
			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 2), CellType.WALL_OUTER_WEST);

			RegionManager.Instance.SetCellAt(new IntVector3(1, 0, 0), CellType.WALL_OUTER_NORTH);
			RegionManager.Instance.SetCellAt(new IntVector3(2, 0, 0), CellType.WALL_OUTER_NORTH);

			RegionManager.Instance.SetCellAt(new IntVector3(1, 0, 3), CellType.WALL_OUTER_SOUTH);
			RegionManager.Instance.SetCellAt(new IntVector3(2, 0, 3), CellType.WALL_OUTER_SOUTH);

			RegionManager.Instance.UpdateRenderedCells(Vector2.zero);
		}
	}

}