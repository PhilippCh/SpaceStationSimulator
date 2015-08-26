using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Game;
using SpaceStation.Station.Structure.Cell.Mask;

namespace SpaceStation.Station.Structure.Cell {

	public class WallObject : BaseObject {

		public enum WallType {
			INVALID,

			OUTER_DEFAULT,
			OUTER_EDGE
		}

		private enum NeighborType {
			EMPTY,
			FLOOR,
			WALL
		}

		public Rotation Rotation;
		public WallType Type;

		private static WallMask masks;
		private static Dictionary<WallType, GameObject> prefabs;

		public override void Update(IntVector3 position) {
			PreloadPrefabs();
			PreloadMasks();

			var neighborCells = new short[9];
			var validMasks = new Dictionary<WallType, CellMask>(5);
			var registry = GameRegistry.Instance;
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
				validMasks.Add(WallType.OUTER_DEFAULT, masks[WallType.OUTER_DEFAULT]);
				validMasks.Add(WallType.OUTER_EDGE, masks[WallType.OUTER_EDGE]);
			}

			var calculatedType = WallType.OUTER_DEFAULT;
			var calculatedRotation = Rotation.NORTH;

			foreach (var wallType in validMasks.Keys) {

				if (validMasks[wallType].Match(neighborCells, out calculatedRotation)) {
					calculatedType = wallType;
					Logger.QuickInfo("found match");
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

		private void PreloadPrefabs() {
			if (prefabs != null) {
				return;
			}

			Logger.Info("WallObject", "Loading variant prefabs.");

			prefabs = new Dictionary<WallType, GameObject>();
							
			prefabs.Add(WallType.OUTER_DEFAULT, Resources.Load("Prefabs/wallOuter") as GameObject);
			prefabs.Add(WallType.OUTER_EDGE, Resources.Load("Prefabs/wallEdgeOuter") as GameObject);
		}

		private void PreloadMasks() {
			if (masks != null) {
				return;
			}

			masks = new WallMask();
		}

		private void SpawnWall(WallType type, IntVector3 position, Rotation rotation) {
			if (!prefabs.ContainsKey(type)) {
				Logger.Warn("CreateAt", "Could not load prefab for type {0}.", type);
				return;
			}

			var prefab = prefabs[type].Spawn();
			prefab.transform.position = position.ToVector3();

			this.goReference = prefab;
			this.Type = type;

			SetRotation(rotation);
			Logger.Info("SpawnWall", "Spawned wall {0} at {1} {2}.", type, position, this.goReference.transform.eulerAngles);
		}

		private void SetRotation(Rotation rotation) {
			var prefabRotation = prefabs[this.Type].transform.eulerAngles;

			prefabRotation.y = RotationHelper.GetEulerAngle(rotation);

			this.goReference.transform.eulerAngles = prefabRotation;
			this.Rotation = rotation;
		}
	}

}