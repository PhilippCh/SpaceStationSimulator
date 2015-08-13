using UnityEngine;
using System.Collections;

using SpaceStation;

namespace SpaceStation.Util {

	public struct CubeBounds {

		public float x;
		public float y;
		public float z;

		public float Width;
		public float Height;
		public float Length;

		public CubeBounds(float x, float y, float z, float width, float height, float length) {
			this.x = x;
			this.y = y;
			this.z = z;

			this.Width = width;
			this.Height = height;
			this.Length = length;
		}

		public bool Contains(IntVector3 position) {
			return Contains(position.x, position.y, position.z);
		}

		public bool Contains(float x, float y, float z) {
			bool contains;
			
			contains = x >= this.x && y >= this.y && z >= this.z;
			contains = contains && x <= this.x + this.Width && y <= this.y + this.Height && z < this.z + this.Length;
			
			return contains;
		}
	}

}