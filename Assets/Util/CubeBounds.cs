using UnityEngine;
using System.Collections;

using SpaceStation;

namespace SpaceStation.Util {

	public struct CubeBounds {

		public Vector3 Position;
		public Vector3 Dimensions;

		#region Helper properties

		public float x {
			get {
				return Position.x;
			}
		}

		public float y {
			get {
				return Position.y;
			}
		}

		public float z {
			get {
				return Position.z;
			}
		}

		public float Width {
			get {
				return Dimensions.x;
			}
		}
		
		public float Height {
			get {
				return Dimensions.y;
			}
		}
		
		public float Length {
			get {
				return Dimensions.z;
			}
		}

		#endregion

		public CubeBounds(IntVector3 position, float size) 
			: this(position, size, size, size) {}

		public CubeBounds(IntVector3 position, float width, float height, float length)
			: this(position.x, position.y, position.z, width, height, length) {}

		public CubeBounds(float x, float y, float z, float width, float height, float length) {
			this.Position = new Vector3(x, y, z);
			this.Dimensions = new Vector3(width, height, length);
		}

		public bool Contains(Vector3 position) {
			return Contains(position.x, position.y, position.z);
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

		public override string ToString()
		{
			return string.Format("[Position: {0} Size: {1}]", Position.ToString(), Dimensions.ToString());
		}
	}

}