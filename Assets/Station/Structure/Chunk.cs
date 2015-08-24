using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

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

		private bool isActive;

		public Chunk(IntVector3 position) {
			var indexedSize = (int) Mathf.Pow(CHUNK_SIZE, 3);

			this.cells = new CellDefinition[indexedSize];
			this.Bounds = new CubeBounds(position, CHUNK_SIZE);
		}

		public static IntVector3 ConvertAbsToRelPosition(IntVector3 absPos) {
			var relativePos = new IntVector3();

			relativePos.x = absPos.x - ((int)Mathf.Floor(absPos.x / Chunk.CHUNK_SIZE) * CHUNK_SIZE);
			relativePos.y = absPos.y - ((int) Mathf.Floor(absPos.y / Chunk.CHUNK_SIZE) * CHUNK_SIZE);
			relativePos.z = absPos.z - ((int) Mathf.Floor(absPos.z / Chunk.CHUNK_SIZE) * CHUNK_SIZE);

			return relativePos;
		}

		public IntVector3 ConvertRelToAbsPosition(IntVector3 relPos) {
			var absolutePos = new IntVector3();

			absolutePos = this.Bounds.Position.ToIntVector3();
			
			absolutePos.x += relPos.x;
			absolutePos.y += relPos.y;
			absolutePos.z += relPos.z;
			
			return absolutePos;
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

		public void UpdateAll() {
			LoopHelper.IntXYZ(IntVector3.zero, new IntVector3(Chunk.CHUNK_SIZE - 1), 1, (x, y, z) => {
				var index = x + (y * Chunk.CHUNK_SIZE) + (z * Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE);
				
				if (this.cells[index] != null) {
					this.cells[index].Update();
				}
			});
		}

		public CellDefinition GetCell(IntVector3 position) {
			if (!this.Bounds.Contains(position)) {
				Logger.Warn("GetCell", "Cell index out of bounds. {0}", position);
				return null;
			}

			var relPos = ConvertAbsToRelPosition(position);

			return cells[relPos.x + (relPos.y * CHUNK_SIZE) + (relPos.z * CHUNK_SIZE * CHUNK_SIZE)];
		}

		public void SetCell(IntVector3 relativePosition, CellDefinition cell) {
			var index = relativePosition.x + (relativePosition.y * CHUNK_SIZE) + (relativePosition.z * CHUNK_SIZE * CHUNK_SIZE);

			cells[index] = cell;

			if (this.isActive && cell != null) {
				var absPosition = ConvertRelToAbsPosition(relativePosition);
			}
		}
	}

}