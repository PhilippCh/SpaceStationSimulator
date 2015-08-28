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

		public enum WallType {
			INVALID,

			OUTER_DEFAULT,
			OUTER_EDGE_OUTER,
			OUTER_EDGE_INNER
		}

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

		public override void Update(IntVector3 position) {
			var cellMasks = registry.WallObjectHelper.Masks;

			var neighborCells = new short[9];
			var validMasks = new Dictionary<WallType, CellMask>(5);
			var containsEmptySpaces = false;
			var containsOtherWalls = false;

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

					neighborCells[cellCounter] = registry.GetObjectId<WallObject>();
				} else if (cell.floor != null) {
					neighborCells[cellCounter] = registry.GetObjectId<FloorObject>();
				} else {
					neighborCells[cellCounter] = GameRegistry.EmptyObjectId;
				}
			}
							
			if (containsEmptySpaces && containsOtherWalls) {
				validMasks.Add(WallType.OUTER_DEFAULT, cellMasks[WallType.OUTER_DEFAULT]);
				validMasks.Add(WallType.OUTER_EDGE_OUTER, cellMasks[WallType.OUTER_EDGE_OUTER]);
			}

			validMasks.Add(WallType.OUTER_EDGE_INNER, cellMasks[WallType.OUTER_EDGE_INNER]);

			var calculatedType = WallType.OUTER_DEFAULT;
			var calculatedRotation = Rotation.NORTH;
		
			foreach (var wallType in validMasks.Keys) {

				if (validMasks[wallType].Match(neighborCells, out calculatedRotation)) {
					calculatedType = wallType;
					break;
				}
			}

			/* Spawn the new wall object and set transform */
			if (this.Type != calculatedType) {
				SpawnWall(calculatedType, position, calculatedRotation);
			} else if (this.Rotation != calculatedRotation) {
				SetRotation(calculatedRotation);
			}
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