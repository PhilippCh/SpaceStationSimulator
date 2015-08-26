using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;

namespace SpaceStation.Station.Object {

	public enum Rotation {
		NORTH,
		EAST,
		SOUTH,
		WEST
	}

	public static class RotationHelper {

		private static Dictionary<Rotation, int> rotationMap;

		static RotationHelper() {
			rotationMap = new Dictionary<Rotation, int>();

			rotationMap.Add(Rotation.NORTH, 90);
			rotationMap.Add(Rotation.EAST, 0);
			rotationMap.Add(Rotation.SOUTH, 270);
			rotationMap.Add(Rotation.WEST, 180);
		}

		public static int GetEulerAngle (Rotation rotation) {
			return rotationMap[rotation];
		}
	}

}