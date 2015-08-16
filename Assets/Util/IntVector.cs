using System;
using UnityEngine;

namespace SpaceStation.Util {

	public struct IntVector2 {

		public static IntVector2 zero = new IntVector2(0, 0);

		public int x, z;
		
		public IntVector2 (int x, int z) {
			this.x = x;
			this.z = z;
		}

		public Vector2 ToVector2() {
			return new Vector2(this.x, this.z);
		}

		public static IntVector2 operator + (IntVector2 a, IntVector2 b) {
			a.x += b.x;
			a.z += b.z;

			return a;
		}

		public override string ToString()
		{
			return string.Format("[x: {0} z: {1}]", this.x, this.z);
		}
	}

	public struct IntVector3 {

		public static IntVector3 zero = new IntVector3(0, 0, 0);

		public int x, y, z;

		public IntVector3 (int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3 ToVector3() {
			return new Vector3(this.x, this.y, this.z);
		}

		public static IntVector3 operator + (IntVector3 a, IntVector3 b) {
			a.x += b.x;
			a.y += b.y;
			a.z += b.z;
			
			return a;
		}

		public override string ToString()
		{
			return string.Format("[x: {0} y: {1} z: {2}]", this.x, this.y, this.z);
		}
	}

	public static class IntVectorExtensions {

		public static IntVector2 ToIntVector2(this Vector2 original) {
			return new IntVector2((int) original.x, (int) original.y);
		}

		public static IntVector3 ToIntVector3(this Vector3 original) {
			return new IntVector3((int) original.x, (int) original.y, (int) original.z);
		}
	}
}
