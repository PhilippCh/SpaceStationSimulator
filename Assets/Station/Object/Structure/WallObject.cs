using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Station.Structure.Cell;
using System.Collections.Generic;
using SpaceStation.Util;

namespace SpaceStation.Station.Object.Structure {
	
	public class WallObject : BaseObject {

		public enum WallType {
			OUTER_DEFAULT,
			OUTER_EDGE
		}

		private static Dictionary<WallType, GameObject> prefabs;

		public WallObject() {

			/* Preload wall prefabs */
			if (prefabs == null) {
				Logger.Info("WallObject", "Loading variant prefabs.");

				prefabs = new Dictionary<WallType, GameObject>();
				
				prefabs.Add(WallType.OUTER_DEFAULT, Resources.Load("Prefabs/wallOuter") as GameObject);
				prefabs.Add(WallType.OUTER_EDGE, Resources.Load("Prefabs/wallEdgeOuter") as GameObject);
			}
		}

		public override void OnCreate(CellStorage data) {
			this.cellReference = data;

			CreateWall();

			/* TODO: Implement visibility algorithm */
			this.Show();
		}

		public override void OnUpdateMetadata(CellStorage data) {
			this.cellReference = data;

			if (this.isVisible) {
				CreateWall();
			}
		}

		private void CreateWall() {
			var wallType = (WallType) this.cellReference.Metadata;

			if (!prefabs.ContainsKey(wallType)) {
				Logger.Warn("OnUpdateMetadata", "Could not load prefab for type {0}.", wallType);
				return;
			}
			
			/* Recycle any active GO */
			this.RecycleGameObject();
			
			/* Spawn the new wall object and set transform */
			var wallObject = prefabs[wallType].Spawn();
			var yRotation = RotationHelper.GetEulerAngle(this.cellReference.Rotation);

			wallObject.transform.position = this.cellReference.Position.ToVector3();
			wallObject.transform.eulerAngles = new Vector3(0.0f, (float) yRotation, 0.0f);
			
			this.goReference = wallObject;
		}
	}

}