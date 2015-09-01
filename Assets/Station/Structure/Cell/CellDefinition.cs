using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;

namespace SpaceStation.Station.Structure.Cell {

	public class CellDefinition {

		public WallObject wall;
		public FloorObject floor;

		public BaseObject containedObject;

		public IntVector3 Position;
		public Chunk ParentChunk;

		public static bool IsEmpty(CellDefinition cell) {
			if (cell == null) {
				return true;
			}
			
			return cell.wall == null && cell.floor == null && cell.containedObject == null;
		}

		public CellDefinition(IntVector3 position, Chunk parentChunk) {
			this.Position = position;
			this.ParentChunk = parentChunk;
		}

		public void Update() {

			/* If present, update floor object */
			if (this.floor != null) {
				this.floor.Update(this.Position);
			}
			
			/* If present, update wall object */
			if (this.wall != null) {
				this.wall.Update(this.Position);
			}
		}

		public bool IsEmpty() {
			return this.wall == null && this.floor == null && this.containedObject == null;
		}

		public void CreateWall() {
			if (this.wall == null) {
				this.wall = new WallObject();
			}

			this.ParentChunk.SetCellDirty(this);
		}

		public void CreateFloor() {
			if (this.floor == null) {
				this.floor = new FloorObject();
			}
			
			this.ParentChunk.SetCellDirty(this);
		}

		public void Destroy() {
			if (this.wall != null) {
				this.wall.Destroy();
			}

			if (this.floor != null) {
				this.floor.Destroy();
			}

			if (this.containedObject != null) {
				this.containedObject.Destroy();
			}
		}
	}

}