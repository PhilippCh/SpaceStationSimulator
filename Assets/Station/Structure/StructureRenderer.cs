using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Structure.Station.Cell;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Structure {

	public class StructureRenderer {

		private const string CELL_PREFAB_DIR = "Prefabs/";

		private Dictionary<CellType, GameObject> cellPrefabMappings;
		private Dictionary<IntVector3, Transform> enabledCells;

		private Transform cellContainer;

		public StructureRenderer() {
			cellPrefabMappings = new Dictionary<CellType, GameObject>();
			enabledCells = new Dictionary<IntVector3, Transform>();
		}

		public void Initialize(Transform cellContainer) {
			this.cellContainer = cellContainer;

			RegisterPrefab(CellType.FLOOR, CELL_PREFAB_DIR + "floor");

			RegisterPrefab(CellType.WALL_OUTER_NORTH, CELL_PREFAB_DIR + "wallBevelled--North");
			RegisterPrefab(CellType.WALL_OUTER_EAST, CELL_PREFAB_DIR + "wallBevelled--East");
			RegisterPrefab(CellType.WALL_OUTER_SOUTH, CELL_PREFAB_DIR + "wallBevelled--South");
			RegisterPrefab(CellType.WALL_OUTER_WEST, CELL_PREFAB_DIR + "wallBevelled--West");

			RegisterPrefab(CellType.WALL_OUTER_EDGE_NORTHEAST, CELL_PREFAB_DIR + "wallBevelledEdge--NorthEast");
			RegisterPrefab(CellType.WALL_OUTER_EDGE_NORTHWEST, CELL_PREFAB_DIR + "wallBevelledEdge--NorthWest");
			RegisterPrefab(CellType.WALL_OUTER_EDGE_SOUTHEAST, CELL_PREFAB_DIR + "wallBevelledEdge--SouthEast");
			RegisterPrefab(CellType.WALL_OUTER_EDGE_SOUTHWEST, CELL_PREFAB_DIR + "wallBevelledEdge--SouthWest");
		}

		public void RegisterPrefab(CellType cellType, string prefabPath) {
			var cellPrefab = Resources.Load(prefabPath) as GameObject;

			if (cellPrefab == null) {
				Logger.Warn("StructureRenderer.RegisterPrefab", "Could not find prefab for {0} at {1}.", cellType, prefabPath);
				return;
			}

			cellPrefabMappings.Add(cellType, cellPrefab);
		}

		public void EnableCell(IntVector3 position, CellType cellType) {

			/* Fail if we already enabled this cell */
			if (enabledCells.ContainsKey(position)) {
				Logger.Warn("EnableCell", "Trying to enable an already active cell at {0}.", position);
				return;
			}

			/* Fail if we can't find a matching prefab for this cell type */
			if (!cellPrefabMappings.ContainsKey(cellType)) {
				Logger.Warn("EnableCell", "Cannot instantiate cell of type {0} because no prefab mapping exists.", cellType);
				return;
			}

			/* Instantiate cell prefab from object pool
			 * TODO: Add rotation information */
			var cellPrefab = cellPrefabMappings[cellType];

			if (cellPrefab == null) {
				Logger.Warn("EnableCell", "Cell prefab for type {0} is null. Check your prefab names.", cellType);
				return;
			}

			var cellInstance = cellPrefab.Spawn(position.ToVector3(), cellPrefab.transform.rotation);
			cellInstance.transform.parent = cellContainer;

			enabledCells.Add(position, cellInstance.transform);
		}
	}

}