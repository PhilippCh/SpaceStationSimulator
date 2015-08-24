using UnityEngine;
using System.Collections;

using SpaceStation;
using System;

namespace SpaceStation.Util {

	public static class LoopHelper {

		public static void IntXYZ (IntVector3 from, IntVector3 to, int step, Action<int, int, int> callback) {

			for (int x = from.x; x <= to.x; x += step) {
				for (int y = from.y; y <= to.y; y += step) {
					for (int z = from.z; z <= to.z; z += step) {

						callback(x, y, z);
					}
				}
			}
		}

		public static void IntXZ (IntVector2 from, IntVector2 to, int step, Action<int, int> callback) {
			
			for (int x = from.x; x <= to.x; x += step) {
				for (int z = from.z; z <= to.z; z += step) {
						
					callback(x, z);
				}
			}
		}

	}

}