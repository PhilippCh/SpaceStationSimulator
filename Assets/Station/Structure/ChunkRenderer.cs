using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Structure {

	public class ChunkRenderer : MonoBehaviour {

		private Chunk[] activeChunks;
		private int centerChunk;

		private IntVector3 topLeftCorner;

		public void ReloadChunks(IntVector3 centerChunk) {
			var regionManager = RegionManager.Instance;

			topLeftCorner = centerChunk - 1;

			LoopHelper.IntXYZ(topLeftCorner, centerChunk + 1, 1, (x, y, z) => {
				var position = new IntVector3(x, y, z);
				var relativePosition = position - topLeftCorner;
				var index = GetChunkIndexFromPosition(relativePosition);
				
				Logger.Info("ReloadChunks", "Loading chunk at {0}.", position);
				this.activeChunks[index] = regionManager.GetChunkAt(position);
				this.activeChunks[index].SetActive();
			});
		}

		private void Awake() {
			this.activeChunks = new Chunk[27];
			this.ReloadChunks(new IntVector3(8, 8, 8));
		}

		private void Update() {
			if (this.activeChunks[centerChunk].Bounds.Contains(transform.position)) {
				return;
			}
		}

		private void OnDrawGizmos() {
			Gizmos.color = Color.yellow;

			LoopHelper.IntXYZ(IntVector3.zero, new IntVector3(2), 1, (x, y, z) => {
				var relativePosition = new IntVector3(x, y, z);
				
				this.DrawChunkBounds(relativePosition);
			});

			/* Render center chunk in green */
			Gizmos.color = Color.green;
			this.DrawChunkBounds(IntVector3.one);
		}

		private void DrawChunkBounds(IntVector3 relPos) {
			var cubeSize = new Vector3(Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE);
			var chunkCenter = new Vector3((topLeftCorner.x + relPos.x) * Chunk.CHUNK_SIZE - Chunk.HALF_CHUNK_SIZE,
			                              (topLeftCorner.y + relPos.y) * Chunk.CHUNK_SIZE - Chunk.HALF_CHUNK_SIZE,
			                              (topLeftCorner.z + relPos.z) * Chunk.CHUNK_SIZE - Chunk.HALF_CHUNK_SIZE);
			
			Gizmos.DrawWireCube(chunkCenter, cubeSize);
		}

		private int GetChunkIndexFromPosition(IntVector3 relativePosition) {
			return relativePosition.x + (relativePosition.y * 3) + (relativePosition.z * 9);
		}
	}

}