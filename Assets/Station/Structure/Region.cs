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

		public CubeBounds Bounds;

		private Chunk[] chunks;

		public Region() {
			var indexedSize = (int)Mathf.Pow(REGION_SIZE, 3);
			var upperBound = (int)Mathf.Pow(REGION_SIZE, 2);

			this.chunks = new Chunk[indexedSize];
			this.Bounds = new CubeBounds(0, 0, 0, upperBound, upperBound, upperBound);
		}

		public Chunk GetChunkAt(IntVector3 position) {
			if (!this.Bounds.Contains(position)) {
				Logger.Warn("GetChunkAt", "Cannot get chunk at {0}, does not belong inside this region.", position);
				return null;
			}

			var indexedPos = ConvertPositionToIndex(position);

			/* Initialize chunk if it does not exist yet */
			if (this.chunks[indexedPos] == null) {
				this.chunks[indexedPos] = new Chunk(position);
			}

			return this.chunks[indexedPos];
		}

		private int ConvertPositionToIndex(IntVector3 position) {
			var relativePos = new IntVector3();

			relativePos.x = (int) Mathf.Floor(position.x / Chunk.CHUNK_SIZE);
			relativePos.y = (int) Mathf.Floor(position.y / Chunk.CHUNK_SIZE);
			relativePos.z = (int) Mathf.Floor(position.z / Chunk.CHUNK_SIZE);

			return relativePos.x + (relativePos.y * REGION_SIZE) + (relativePos.z * REGION_SIZE * REGION_SIZE);
		}
	}

}