using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;

namespace SpaceStation.Station.Object {

	public class FloorObject : BaseObject {

		public Texture2D Texture;

		private GameObject floorPrefab;

		#region implemented abstract members of BaseObject

		public override void Update(IntVector3 position) {
			SpawnFloor(position);
		}

		private void SpawnFloor(IntVector3 position) {

			/* Spawn floor object and initialize if it does not exist yet */
			if (this.goReference == null) {
				PreloadPrefabs();
				
				this.goReference = floorPrefab.Spawn();
				
				this.goReference.transform.position = position.ToVector3();
				this.goReference.transform.eulerAngles = floorPrefab.transform.eulerAngles;
			}
			
			/* TODO: Add texture update logic */
		}

		public override SerializedObject Serialize()
		{
			var serializedObject = new SerializedObject();
			serializedObject.Id = GameRegistry.Instance.GetObjectId<FloorObject>();
			
			return serializedObject;
		}

		public override void Deserialize(IntVector3 position, SerializedObject serializedObject)
		{
			SpawnFloor(position);
		}

		#endregion

		private void PreloadPrefabs() {
			if (this.floorPrefab != null) {
				return;
			}

			this.floorPrefab = Resources.Load("floor") as GameObject;
		}
	}

}