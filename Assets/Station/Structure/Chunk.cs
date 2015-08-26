using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;
using System.Collections.Generic;
using System;

namespace SpaceStation.Station.Structure {

	/**
	 * A chunk contains a 16x16x16 area of cells.
	 */
	public class Chunk {

		/* Base size of a chunk square */
		public const int CHUNK_SIZE = 8;
		public const int HALF_CHUNK_SIZE = CHUNK_SIZE / 2;

		public Guid ChunkId = Guid.NewGuid();

		public CubeBounds Bounds;

		private CellDefinition[] cells;
		private List<CellDefinition> dirtyCells;

		private bool isActive;

		public Chunk(IntVector3 position) {
			var indexedSize = (int) Mathf.Pow(CHUNK_SIZE, 3);
			var chunkPos = ClampAbsPosition(position);

			this.cells = new CellDefinition[indexedSize];
			this.dirtyCells = new List<CellDefinition>(128);
			this.Bounds = new CubeBounds(chunkPos, CHUNK_SIZE);
		}

		public static IntVector3 ClampAbsPosition(IntVector3 absPos) {
			var clampedPos = new IntVector3();

			clampedPos.x = (int)Mathf.Floor(absPos.x / Chunk.CHUNK_SIZE) * CHUNK_SIZE;
			clampedPos.y = (int)Mathf.Floor(absPos.y / Chunk.CHUNK_SIZE) * CHUNK_SIZE;
			clampedPos.z = (int)Mathf.Floor(absPos.z / Chunk.CHUNK_SIZE) * CHUNK_SIZE;

			return clampedPos;
		}

		public static IntVector3 ConvertAbsToRelPosition(IntVector3 absPos) {
			return absPos - ClampAbsPosition(absPos);
		}

		public IntVector3 ConvertRelToAbsPosition(IntVector3 relPos) {
			var absolutePos = new IntVector3();

			absolutePos = this.Bounds.Position.ToIntVector3();

			absolutePos.x += relPos.x;
			absolutePos.y += relPos.y;
			absolutePos.z += relPos.z;
			
			return absolutePos;
		}

		public void Update() {
			for (int i = 0; i < this.dirtyCells.Count; i++) {

				if (this.dirtyCells[i] != null) {
					this.dirtyCells[i].Update();
					this.dirtyCells.RemoveAt(i);
				}
			}
		}

		public void SetActive() {
			LoopHelper.IntXYZ(IntVector3.zero, new IntVector3(Chunk.CHUNK_SIZE - 1), 1, (x, y, z) => {
				var index = x + (y * Chunk.CHUNK_SIZE) + (z * Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE);

				if (this.cells[index] != null) {
					this.cells[index].Update();
				}
			});

			this.isActive = true;
		}

		public CellDefinition GetCell(IntVector3 position) {
			if (!this.Bounds.Contains(position)) {
				Logger.Warn("GetCell", "Cell index out of bounds. {0}", position);
				return null;
			}

			var relPos = ConvertAbsToRelPosition(position);

			return cells[relPos.x + (relPos.y * CHUNK_SIZE) + (relPos.z * CHUNK_SIZE * CHUNK_SIZE)];
		}

		public void SetCell(IntVector3 position, CellDefinition cell) {
			var relPos = ConvertAbsToRelPosition(position);
			var index = relPos.x + (relPos.y * CHUNK_SIZE) + (relPos.z * CHUNK_SIZE * CHUNK_SIZE);

			cells[index] = cell;

			SetCellDirty(cell);
		}

		private void SetCellDirty(CellDefinition cell) {
			if (cell != null && !dirtyCells.Contains(cell)) {
				dirtyCells.Add(cell);
			}
		}
	}

}