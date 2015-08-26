using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;

namespace SpaceStation.Station.Structure.Cell {

	public class CellDefinition {

		private IntVector3 position;

		public WallObject wall;
		public FloorObject floor;

		public BaseObject containedObject;

		public static bool IsEmpty(CellDefinition cell) {
			if (cell == null) {
				return true;
			}

			return cell.wall == null && cell.floor == null && cell.containedObject == null;
		}

		public CellDefinition(IntVector3 position) {
			this.position = position;
		}

		public void Update() {

			/* If present, update floor object */
			if (this.floor != null) {
				this.floor.Update(this.position);
			}
			
			/* If present, update wall object */
			if (this.wall != null) {
				this.wall.Update(this.position);
			}
		}

		public bool IsEmpty() {
			return this.wall == null && this.floor == null && this.containedObject == null;
		}
	}

}