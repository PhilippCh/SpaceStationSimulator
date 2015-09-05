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

		public CellDefinition[] GetAllCells() {
			return this.cells;
		}

		public CellDefinition GetCell(IntVector3 position) {
			if (!this.Bounds.Contains(position)) {
				Logger.Warn("GetCell", "Cell index out of bounds. {0}", position);
				return null;
			}

			var relPos = ConvertAbsToRelPosition(position);

			return cells[relPos.x + (relPos.y * CHUNK_SIZE) + (relPos.z * CHUNK_SIZE * CHUNK_SIZE)];
		}

		public CellDefinition CreateCell(IntVector3 position) {
			var index = ConvertAbsPositionToIndex(position);

			if (cells[index] != null) {
				Logger.Warn("CreateCell", "Cell at {0} already exists.", position);
				return cells[index];
			}

			cells[index] = new CellDefinition(position, this);

			return cells[index];
		}

		public void DestroyCellAt(IntVector3 position) {
			var index = ConvertAbsPositionToIndex(position);

			if (cells[index] != null) {
				cells[index].Destroy();
				cells[index] = null;
			}
		}

		public void SetCellAt(IntVector3 absPos, CellDefinition cell) {
			var index = ConvertAbsPositionToIndex(absPos);

			if (this.cells[index] != cell) {
				this.cells[index] = cell;
			}
		}

		public void SetCellDirty(CellDefinition cell) {
			if (cell != null && !dirtyCells.Contains(cell)) {
				dirtyCells.Add(cell);
			}
		}

		private int ConvertAbsPositionToIndex(IntVector3 absPos) {
			var relPos = ConvertAbsToRelPosition(absPos);

			return ConvertRelPositionToIndex(relPos);
		}

		public static IntVector3 ConvertIndexToRelPosition(int index) {
			var z = index / (CHUNK_SIZE * CHUNK_SIZE);
			var y = (index - (z * CHUNK_SIZE * CHUNK_SIZE)) / CHUNK_SIZE;
			var x = index - y * CHUNK_SIZE - z * CHUNK_SIZE * CHUNK_SIZE;

			return new IntVector3(x, y, z);
		}

		private int ConvertRelPositionToIndex(IntVector3 relPos) {
			return relPos.x + (relPos.y * CHUNK_SIZE) + (relPos.z * CHUNK_SIZE * CHUNK_SIZE);
		}
	}

	[System.Serializable]
	public class SerializedChunk {

		public IntVector3 Position;
		public SerializedCellDefinition[] Cells;

		public static SerializedChunk Serialize(Chunk chunk) {
			if (chunk == null) {
				return null;
			}

			var cells = chunk.GetAllCells();
			var serializedChunk = new SerializedChunk();

			serializedChunk.Position = chunk.Bounds.Position.ToIntVector3();
			serializedChunk.Cells = new SerializedCellDefinition[cells.Length];

			for (int i = 0; i < cells.Length; i++) {
				serializedChunk.Cells[i] = SerializedCellDefinition.Serialize(cells[i]);
			}

			return serializedChunk;
		}

		public Chunk Deserialize() {
			var chunk = new Chunk(this.Position);

			for (int i = 0; i < this.Cells.Length; i++) {
				if (this.Cells[i] != null) {
					var cellPos = this.Position + Chunk.ConvertIndexToRelPosition(i);

					SerializedCellDefinition.Deserialize(cellPos, chunk, this.Cells[i]);
				}
			}

			return chunk;
		}
	}

}