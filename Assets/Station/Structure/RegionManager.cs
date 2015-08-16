using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Structure {

	/**
	 * We currently support a playable area of 256*256*256 cells.
	 * 
	 * Theoretically, we could add support for an infinite number of regions.
	 */
	public class RegionManager : MonoBehaviour {

		public static RegionManager Instance { get; private set; }

		public Transform CellContainer;

		private Region activeRegion;
		private IStructureRenderer structureRenderer;

		public RegionManager() {
			Instance = this;
		}

		private void Awake() {
			activeRegion = new Region();
			structureRenderer = new StructureRenderer();

			structureRenderer.Initialize(CellContainer);
		}

		public CellType GetCellAt(Vector3 position) {
			return GetCellAt(position.ToIntVector3());
		}

		public CellType GetCellAt(IntVector3 position) {
			var targetChunk = activeRegion.GetChunkAt(position);
			var relativePos = Chunk.ConvertAbsToRelPosition(position);

			if (targetChunk == null) {
				return CellType.EMPTY;
			} else {
				return targetChunk.GetCell(relativePos);
			}
		}

		public void SetCellAt(IntVector3 position, CellType cellData) {
			var targetChunk = activeRegion.GetChunkAt(position);
			var relativePos = Chunk.ConvertAbsToRelPosition(position);
			
			targetChunk.SetCell(relativePos, cellData);
		}

		public void UpdateRenderedCells(Vector2 targetPos) {

			// For mock purposes, render 10x10x10 slice
			for (int x = 0; x < 11; x++) {
				for (int z = 0; z < 11; z++) {
					var position = new IntVector3(x, 0, z);
					var cell = GetCellAt(position);
				
					if (cell != CellType.EMPTY) {
						structureRenderer.EnableCell(position, cell);
					}
				}	
			}

			Logger.Info("UpdateRenderedCells", "Finished updating");
		}
	}

}