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

			rotationMap.Add(Rotation.NORTH, 0);
			rotationMap.Add(Rotation.EAST, 90);
			rotationMap.Add(Rotation.SOUTH, 180);
			rotationMap.Add(Rotation.WEST, 270);
		}

		public static int GetEulerAngle(Rotation rotation) {
			return rotationMap[rotation];
		}
	}

}