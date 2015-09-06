using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;
using SpaceStation.Station.Structure.Cell;
using SpaceStation.Station.Structure;

namespace SpaceStation.Station.Object {

	[RegisterAsObject(2, typeof(DoorObject))]
	public class DoorObject : BaseObject {

		private GameObject doorPrefab;

		#region implemented abstract members of BaseObject

		public override void Update(IntVector3 position) {
			var neighborCells = new bool[4];
			var cellCounter = 0;

			foreach (IntVector2 offset in CellRange.Values2DCardinal()) {
				var absPos2D = offset + position.ToIntVector2();
				var absPos = new IntVector3(absPos2D.x, position.y, absPos2D.z);
				var cell = RegionManager.Instance.GetCellAt(absPos);

				neighborCells[cellCounter] = cell.wall != null;

				cellCounter++;
			}

			var rotation = (neighborCells[0] && neighborCells[2] ? Rotation.EAST : Rotation.NORTH);

			SpawnDoor(position, rotation);
		}

		private void SpawnDoor(IntVector3 position, Rotation rotation) {

			if (this.goReference == null) {
				PreloadPrefabs();
				
				this.goReference = doorPrefab.Spawn();
				this.goReference.transform.position = position.ToVector3();

				SetRotation(rotation);
			}
		}

		private void SetRotation(Rotation rotation) {
			var prefabRotation = doorPrefab.transform.eulerAngles;
			
			prefabRotation.y = RotationHelper.GetEulerAngle(rotation);
			
			this.goReference.transform.eulerAngles = prefabRotation;
			this.rotation = rotation;
		}

		public override SerializedObject Serialize()
		{
			var serializedObject = new SerializedObject();
			serializedObject.Id = GameRegistry.Instance.GetObjectId<DoorObject>();
		
			serializedObject.Properties.Add(this.rotation);

			return serializedObject;
		}

		public override void Deserialize(IntVector3 position, SerializedObject serializedObject)
		{
			var rotation = (Rotation) serializedObject.Properties[0];

			SpawnDoor(position, rotation);
		}

		#endregion

		private void PreloadPrefabs() {
			if (this.doorPrefab != null) {
				return;
			}

			this.doorPrefab = Resources.Load("Models/doorFrame") as GameObject;
		}
	}

}