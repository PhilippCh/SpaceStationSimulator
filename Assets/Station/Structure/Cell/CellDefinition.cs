using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;
using System;

namespace SpaceStation.Station.Structure.Cell {

	[System.Serializable]
	public class SerializedCellDefinition {

		public SerializedObject wall;
		public SerializedObject floor;
		public SerializedObject containedObject;

		public static SerializedCellDefinition Serialize(CellDefinition cell) {
			if (cell == null) {
				return null;
			}

			var serializedCell = new SerializedCellDefinition();

			if (cell.wall != null) {
				serializedCell.wall = cell.wall.Serialize();
			}

			if (cell.floor != null) {
				serializedCell.floor = cell.floor.Serialize();
			}

			if (cell.containedObject != null) {
				serializedCell.containedObject = cell.containedObject.Serialize();
			}

			return serializedCell;
		}

		public static CellDefinition Deserialize(IntVector3 position, Chunk parentChunk, SerializedCellDefinition serializedCell) {
			var cell = new CellDefinition(position, parentChunk);

			if (serializedCell.wall != null) {
				cell.wall = new WallObject();
				cell.wall.Deserialize(position, serializedCell.wall);
			}
			
			if (serializedCell.floor != null) {
				cell.floor = new FloorObject();
				cell.floor.Deserialize(position, serializedCell.floor);
			}
			
			if (serializedCell.containedObject != null) {
				var type = GameRegistry.Instance.GetObjectType(serializedCell.containedObject.Id);

				cell.containedObject = (BaseObject) Activator.CreateInstance(type);
				cell.containedObject.Deserialize(position, serializedCell.containedObject);
			}

			parentChunk.SetCellAt(position, cell);
			
			return cell;
		}
	}

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

		public static bool IsWalkable(CellDefinition cell) {
			return cell == null ? false : 
				cell.floor != null && cell.containedObject == null;
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

			/* Finally, update contained object */
			if (this.containedObject != null) {
				this.containedObject.Update(this.Position);
			}
		}

		public bool IsEmpty() {
			return this.wall == null && this.floor == null && this.containedObject == null;
		}

		#region Creating Objects

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

		public void CreateObject(BaseObject newObject) {
			if (this.containedObject != null) {
				this.containedObject.Recycle();
			}

			this.containedObject = newObject;
			
			this.ParentChunk.SetCellDirty(this);
		}

		#endregion

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