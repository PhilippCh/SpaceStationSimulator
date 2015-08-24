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
		public const int CHUNK_SIZE = 16;

		private CellContainer[] cells;
		private CubeBounds bounds;

		public Chunk() {
			var indexedSize = (int) Mathf.Pow(CHUNK_SIZE, 3);
			cells = new CellContainer[indexedSize];
			bounds = new CubeBounds(0, 0, 0, CHUNK_SIZE - 1, CHUNK_SIZE - 1, CHUNK_SIZE - 1);
		}

		public static IntVector3 ConvertAbsToRelPosition(IntVector3 absPos) {
			var relativePos = new IntVector3();
			
			relativePos.x = absPos.x - (int) Mathf.Floor(absPos.x / Chunk.CHUNK_SIZE);
			relativePos.y = absPos.y - (int) Mathf.Floor(absPos.y / Chunk.CHUNK_SIZE);
			relativePos.z = absPos.z - (int) Mathf.Floor(absPos.z / Chunk.CHUNK_SIZE);

			return relativePos;
		}

		public CellContainer GetCell(IntVector3 position) {
			if (!bounds.Contains(position)) {
				Logger.Warn("GetCell", "Cell index out of bounds. {0}", position);
				return null;
			}

			return cells[position.x + (position.y * CHUNK_SIZE) + (position.z * CHUNK_SIZE)];
		}

		public void SetCell(IntVector3 position, CellContainer cell) {
			if (!bounds.Contains(position)) {
				Logger.Warn("SetCell", "Cell index out of bounds. {0}", position);
				return;
			}

			cells[position.x + (position.y * CHUNK_SIZE) + (position.z * CHUNK_SIZE)] = cell;
		}
	}

}