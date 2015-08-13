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

			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 0), CellType.WALL_EDGE_N);
			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 3), CellType.WALL_EDGE_E);
			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 0), CellType.WALL_EDGE_S);
			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 3), CellType.WALL_EDGE_W);

			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 1), CellType.WALL_N);
			RegionManager.Instance.SetCellAt(new IntVector3(0, 0, 2), CellType.WALL_N);

			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 1), CellType.WALL_E);
			RegionManager.Instance.SetCellAt(new IntVector3(3, 0, 2), CellType.WALL_E);

			RegionManager.Instance.SetCellAt(new IntVector3(2, 0, 0), CellType.WALL_S);
			RegionManager.Instance.SetCellAt(new IntVector3(1, 0, 0), CellType.WALL_S);

			RegionManager.Instance.SetCellAt(new IntVector3(2, 0, 3), CellType.WALL_W);
			RegionManager.Instance.SetCellAt(new IntVector3(1, 0, 3), CellType.WALL_W);

			RegionManager.Instance.UpdateRenderedCells(Vector2.zero);
		}
	}

}