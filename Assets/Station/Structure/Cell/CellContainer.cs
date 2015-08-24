using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;

namespace SpaceStation.Station.Structure.Cell {

	public class CellContainer {

		private Dictionary<CellLayer, CellStorage> layers;

		public CellContainer() {
			layers = new Dictionary<CellLayer, CellStorage>();
		}

		public void SetLayer(CellLayer layer, CellStorage content) {
			layers[layer] = content;
		}

		public CellStorage GetLayer(CellLayer layer) {
			return layers[layer];
		}
	}

}