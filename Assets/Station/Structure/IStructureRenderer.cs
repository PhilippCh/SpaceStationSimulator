using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Structure.Station.Cell;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Structure {

	public interface IStructureRenderer {

		void Initialize(Transform cellContainer);
		void RegisterPrefab(CellType cellType, string prefabPath);
		void EnableCell(IntVector3 position, CellType cellType);
	}

}