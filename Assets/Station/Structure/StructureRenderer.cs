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

			cellPrefabMappings.Add(CellType.FLOOR, Resources.Load(CELL_PREFAB_DIR + "floor") as GameObject);
			cellPrefabMappings.Add(CellType.WALL_N, Resources.Load(CELL_PREFAB_DIR + "wallBevelled") as GameObject);
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