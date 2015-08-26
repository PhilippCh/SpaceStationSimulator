using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;

namespace SpaceStation.Station.Structure.Cell {

	public class CellRange {

		public static IntVector2 NORTHWEST = new IntVector2(-1, -1);
		public static IntVector2 NORTH = new IntVector2(0, -1);
		public static IntVector2 NORTHEAST = new IntVector2(1, -1);

		public static IntVector2 WEST = new IntVector2(-1, 0);
		public static IntVector2 CENTER = new IntVector2(0, 0);
		public static IntVector2 EAST = new IntVector2(1, 0);

		public static IntVector2 SOUTHWEST = new IntVector2(-1, 1);
		public static IntVector2 SOUTH = new IntVector2(0, 1);
		public static IntVector2 SOUTHEAST = new IntVector2(1, 1);

		public static IEnumerable<IntVector2> Values2D(bool includeCenter = true) {
			yield return NORTHWEST;
			yield return NORTH;
			yield return NORTHEAST;

			yield return WEST;

			if (includeCenter) {
				yield return CENTER;
			}

			yield return EAST;

			yield return SOUTHWEST;
			yield return SOUTH;
			yield return SOUTHEAST;
		}
	}

}