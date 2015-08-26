using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Collections.Generic;
using SpaceStation.Station.Object;
using SpaceStation.Util;
using System;

namespace SpaceStation.Station.Structure.Cell {

	public class CellMask {

		private static Rotation[] rotations = new Rotation[] {
			Rotation.NORTH,
			Rotation.EAST,
			Rotation.SOUTH,
			Rotation.WEST
		};

		private List<short>[][] templates;

		public CellMask(List<short> nW, List<short> n, List<short> nE, 
		                List<short> w, List<short> center, List<short> e, 
		                List<short> sW, List<short> s, List<short> sE) {

			var template = new List<short>[9] {
				nW, n,      nE,
				w,  center, e,
				sW, s,      sE
			};

			this.templates = new List<short>[4][] {
				RotateLocalRange(template, 0),
				RotateLocalRange(template, 1),
				RotateLocalRange(template, 2),
				RotateLocalRange(template, 3)
			};
		}
		
		private List<short>[] RotateLocalRange (List<short>[] localRange, int rotations) {
			List<short>[] rotatedList = new List<short>[9] {
				localRange[0], localRange[1], localRange[2], 
				localRange[3], localRange[4], localRange[5], 
				localRange[6], localRange[7], localRange[8]
			};
			
			for (int i = 0; i < rotations; i++)
			{
				List<short>[] tempList = new List<short>[9] {
					rotatedList[6], rotatedList[3], rotatedList[0], 
					rotatedList[7], rotatedList[4], rotatedList[1], 
					rotatedList[8], rotatedList[5], rotatedList[2]
				};

				rotatedList = tempList;
			}
			
			return rotatedList;
		}

		public bool Match(List<short> objectIds, out Rotation rotation) {
			return Match(objectIds.ToArray(), out rotation);
		}

		public bool Match(short[] objectIds, out Rotation rotation) {
			rotation = Rotation.NORTH;
			
			for (int i = 0; i < (templates.Length); i++) {
				for (int j = 0; j < 9; j++) {
					if (j == 4) continue; // Skip checking the centre position (no need to ascertain that a block is what it says it is).
					
					if (templates[i][j].Count != 0) {
						if (templates[i][j].Contains(objectIds[j]) == false) {
							break;
						}
					}
					
					if (j == 8) { // The loop has iterated nine times without stopping, so all tiles must match.
						rotation = rotations[i];

						return true;
					}
				}
			}

			return false;
		}
	}
	
}