using UnityEngine;
using System.Collections;

using SpaceStation.Station.Structure;
using SpaceStation.Util;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;

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

		public void Update() {
			for (int i = 0; i < chunks.Length; i++) {

				if (chunks[i] != null) {
					chunks[i].Update();
				}
			}
		}

		public Chunk GetChunkAt(IntVector3 position) {
			if (!this.Bounds.Contains(position)) {
				Logger.Warn("GetChunkAt", "Cannot get chunk at {0}, does not belong inside this region.", position);
				return null;
			}

			var indexedPos = ConvertPositionToIndex(position);

			/* Initialize chunk if it does not exist yet */
			if (this.chunks[indexedPos] == null) {
				this.chunks[indexedPos] = new Chunk(Chunk.ClampAbsPosition(position));
			}

			return this.chunks[indexedPos];
		}

		public void Save(string path) {
			FileStream regionFile = File.Create(path);
			BinaryFormatter formatter = new BinaryFormatter();

			List<SerializedChunk> serializedChunks = new List<SerializedChunk>();

			for (int i = 0; i < this.chunks.Length; i++) {
				serializedChunks.Add(SerializedChunk.Serialize(this.chunks[i]));
			}

			formatter.Serialize(regionFile, serializedChunks);

			regionFile.Close();

			Logger.Info("Saved region file.");
		}

		public void Load(string path) {
			var regionFile = File.OpenRead(path);
			var formatter = new BinaryFormatter();

			List<SerializedChunk> serializedChunks = (List<SerializedChunk>) formatter.Deserialize(regionFile);
			
			regionFile.Close();
			
			Logger.Info("Loaded region file, will initialize chunks.");

			if (serializedChunks.Count != this.chunks.Length) {
				Logger.Error("Could not load region file, chunk length does not match actual chunk count.");
				return;
			}

			for (int i = 0; i < serializedChunks.Count; i++) {
				if (serializedChunks[i] != null) {
					this.chunks[i] = serializedChunks[i].Deserialize();
				} else {
					this.chunks[i] = null;
				}
			}
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