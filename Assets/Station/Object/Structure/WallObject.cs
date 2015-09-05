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

	public class WallObject : BaseObject {

		private enum NeighborType {
			EMPTY,
			FLOOR,
			WALL
		}

		public Rotation Rotation;
		public WallType Type;

		private GameRegistry registry;

		public WallObject() {
			this.registry = GameRegistry.Instance;
		}

		#region implemented abstract members of BaseObject

		public override void Update(IntVector3 position) {
			var neighborCells = new short[9];
			var validMasks = new Dictionary<WallType, CellMask>(5);

			/* Properties used to refine valid tile masks */
			var containsEmptySpaces = false;
			var containsOtherWalls = false;
			var wallCount = 0;

			var cellCounter = -1;

			/* Grab surrounding block information */
			foreach (IntVector2 offset in CellRange.Values2D()) {
				var absPos2D = offset + position.ToIntVector2();
				var absPos = new IntVector3(absPos2D.x, position.y, absPos2D.z);
				CellDefinition cell;

				cellCounter++;

				cell = RegionManager.Instance.GetCellAt(absPos);
				containsEmptySpaces = containsEmptySpaces || CellDefinition.IsEmpty(cell);

				if (cell == null) {
					neighborCells[cellCounter] = GameRegistry.EmptyObjectId;
					continue;
				}

				if (cell.wall != null) {
					if (!absPos.Equals(position)) {
						containsOtherWalls = containsOtherWalls || cell.wall != null;
					}

					wallCount++;
					neighborCells[cellCounter] = registry.GetObjectId<WallObject>();
				} else if (cell.floor != null) {
					neighborCells[cellCounter] = registry.GetObjectId<FloorObject>();
				} else {
					neighborCells[cellCounter] = GameRegistry.EmptyObjectId;
				}
			}

			if (containsOtherWalls) {

				/* Default walls */
				AddValidMask(WallType.INNER_DEFAULT, validMasks);
				AddValidMask(WallType.INNER_END, validMasks);

				if (wallCount >= 3) {
					AddValidMask(WallType.INNER_EDGE_INNER, validMasks);
					AddValidMask(WallType.INNER_EDGE_T, validMasks);
				}

				if (wallCount >= 4) {
					AddValidMask(WallType.INNER_EDGE_CROSS, validMasks);
				}

				if (containsEmptySpaces) {
					
					AddValidMask(WallType.OUTER_DEFAULT, validMasks);

					if (wallCount >= 3) {
						AddValidMask(WallType.OUTER_EDGE_OUTER, validMasks);
						AddValidMask(WallType.OUTER_EDGE_INNER, validMasks);
						AddValidMask(WallType.OUTER_EDGE_T_INNER, validMasks);
						AddValidMask(WallType.OUTER_EDGE_T_OUTER, validMasks);
					}
				}
			}

			var calculatedType = WallType.OUTER_DEFAULT;
			var calculatedRotation = Rotation.NORTH;
			var foundMatch = false;

			foreach (var wallType in validMasks.Keys) {

				if (validMasks[wallType].Match(neighborCells, out calculatedRotation)) {
					calculatedType = wallType;
					foundMatch = true;
					break;
				}
			}

			if (!foundMatch) {
				Logger.Warn("Could not find matching type for wall at {0}", position);
			}

			/* Spawn the new wall object and set transform */
			if (this.Type != calculatedType) {
				SpawnWall(calculatedType, position, calculatedRotation);
			} else if (this.Rotation != calculatedRotation) {
				SetRotation(calculatedRotation);
			}
		}

		public override SerializedObject Serialize()
		{
			var serializedObject = new SerializedObject();
			serializedObject.Id = GameRegistry.Instance.GetObjectId<WallObject>();

			serializedObject.Properties.Add(this.Type);
			serializedObject.Properties.Add(this.Rotation);

			return serializedObject;
		}

		public override void Deserialize(IntVector3 position, SerializedObject serializedObject)
		{
			this.Type = (WallType) serializedObject.Properties[0];
			this.Rotation = (Rotation) serializedObject.Properties[1];

			SpawnWall(this.Type, position, this.Rotation);
		}

		#endregion

		private void AddValidMask(WallType type, Dictionary<WallType, CellMask> validMasks) {
			var cellMasks = registry.WallObjectHelper.Masks;

			validMasks.Add(type, cellMasks[type]);
		}

		private void SpawnWall(WallType type, IntVector3 position, Rotation rotation) {
			var prefabs = registry.WallObjectHelper.Prefabs;

			if (!prefabs.ContainsKey(type)) {
				Logger.Warn("Could not load prefab for type {0}.", type);
				return;
			}

			var prefab = prefabs[type].Spawn();
			prefab.transform.position = position.ToVector3();

			this.goReference = prefab;
			this.Type = type;

			SetRotation(rotation);
			Logger.Info("Spawned wall {0} at {1} {2}.", type, position, this.goReference.transform.eulerAngles);
		}

		private void SetRotation(Rotation rotation) {
			var prefabs = registry.WallObjectHelper.Prefabs;
			var prefabRotation = prefabs[this.Type].transform.eulerAngles;

			prefabRotation.y = RotationHelper.GetEulerAngle(rotation);

			this.goReference.transform.eulerAngles = prefabRotation;
			this.Rotation = rotation;
		}
	}

}