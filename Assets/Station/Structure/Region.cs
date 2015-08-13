using UnityEngine;
using System.Collections;

using SpaceStation.Station.Structure;
using SpaceStation.Util;

namespace SpaceStation.Station.Structure {

	/**
	 * A region contains 16x16x16 chunks and is the largest in-game cell storage format.
	 */
	public class Region {

		public const int REGION_SIZE = 16;

		private Chunk[] chunks;

		public Region() {
			var indexedSize = (int) Mathf.Pow(REGION_SIZE, 3);
			chunks = new Chunk[indexedSize];
		}

		public bool Contains(IntVector3 position) {
			int regionUpperBound = (int) Mathf.Pow(REGION_SIZE, 2);
			bool contains;
			
			contains = position.x < regionUpperBound && position.y < regionUpperBound && position.z < regionUpperBound;
			contains = contains && position.x >= 0 && position.y >= 0 && position.z >= 0;
			
			return contains;
		}

		public Chunk GetChunkAt(IntVector3 position) {
			if (!Contains(position)) {
				Logger.Warn("GetChunkAt", "Cannot get chunk at {0}, does not belong inside this region.", position);
				return null;
			}

			var indexedPos = ConvertPositionToIndex(position);

			/* Initialize chunk if it does not exist yet */
			if (chunks[indexedPos] == null) {
				chunks[indexedPos] = new Chunk();
			}

			return chunks[indexedPos];
		}

		private void InitializeChunk(IntVector3 position) {
			if (!Contains(position)) {
				Logger.Warn("InitializeChunk", "Cannot initialize chunk at {0}, does not belong inside this region.", position);
				return;
			}

			chunks[ConvertPositionToIndex(position)] = new Chunk();
		}

		private int ConvertPositionToIndex(IntVector3 position) {
			return position.x + (position.y * REGION_SIZE) + (position.z * REGION_SIZE);
		}
	}

}