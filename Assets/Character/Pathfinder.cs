using UnityEngine;
using System.Collections;
using System.Linq;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;
using System.Collections.Generic;
using SpaceStation.Station.Structure;
using System;

namespace SpaceStation.Character {

	public class PathNode : ICloneable {

		public IntVector3 Position;
		public PathNode ParentNode;

		public int F, G;

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class Pathfinder : MonoBehaviour {

		private RegionManager regionManager;

		private IntVector3 start, end;
		private List<PathNode> openNodes, closedNodes, validNodes;

		private PathNode currentNode;

		private void Awake() {
			this.openNodes = new List<PathNode>();
			this.closedNodes = new List<PathNode>();
			this.validNodes = new List<PathNode>();
			
			this.regionManager = RegionManager.Instance;
		}

		public bool FindPath(IntVector3 start, IntVector3 end, out List<PathNode> path) {
			PathNode endNode = null;
			currentNode = null;

			this.start = start;
			this.end = end;

			this.openNodes.Clear();
			this.closedNodes.Clear();
			this.validNodes.Clear();

			/* Create start node and add to open list */
			var startNode = new PathNode() {
				Position = start,
				G = 0,
				F = Manhattan(start, end)
			};
			
			this.openNodes.Add(startNode);
			
			while (this.openNodes.Count > 0) {
	
				/* Sort open nodes by lowest f(G, H) = G + H */
				this.openNodes = this.openNodes.OrderBy( node => node.F ).ToList();
				
				currentNode = this.openNodes.PopAt(0);
				this.closedNodes.Add(currentNode);
				
				if (currentNode.Position.Equals(end)) {
					endNode = currentNode;
					break;
				}
				
				/* Visit all neighbors of our current node */
				foreach(var offset in CellRange.Values2D(false)) {
					var absPos = currentNode.Position + offset.ToIntVector3(0);
					
					/* Continue if we already visited that neighbor */
					var isClosedNode = this.closedNodes.Find( node => node.Position.Equals(absPos) ) != null;
					var isWalkable = CellDefinition.IsWalkable(regionManager.GetCellAt(absPos));

					if (isClosedNode || !isWalkable) {
						continue;
					}
					
					var tentativeG = currentNode.G + Distance(currentNode.Position, absPos);
					var openNeighbor = this.openNodes.Find( node => node.Position.Equals(absPos) );
					
					if (openNeighbor == null || tentativeG < openNeighbor.G) {
						if (openNeighbor == null) {
							openNeighbor = new PathNode();
							
							this.openNodes.Add(openNeighbor);
						}
						
						openNeighbor.ParentNode = currentNode;
						openNeighbor.Position = absPos;
						openNeighbor.G = tentativeG;
						openNeighbor.F = openNeighbor.G + Manhattan(absPos, end);
					}
				}
			}

			if (endNode == null) {
				Logger.Warn("Could not find a valid path.");
				path = null;

				return false;
			} else {
				path = ReconstructPath(endNode);
				path.Reverse();

				return true;
			}
		}

		private List<PathNode> ReconstructPath(PathNode endNode) {
			var currentNode = endNode;

			while (true) {
				this.validNodes.Add(currentNode);
				currentNode = currentNode.ParentNode;

				if (currentNode.ParentNode == null) {
					break;
				}
			}

			return this.validNodes;
		}

		private int Distance(IntVector3 start, IntVector3 end) {
			return Mathf.Abs((end.x - start.x) + (end.y - start.y) + (end.z - start.z));
		}

		private int Manhattan(IntVector3 start, IntVector3 end) {
			return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) + Mathf.Abs(start.z - end.z);
		}

		private void OnDrawGizmos() {
			var gizmoSize = new Vector3(.25f, .25f, .25f);

			/* Draw open nodes */
			Gizmos.color = Color.blue;

			if (this.openNodes != null) {
				foreach (var node in this.openNodes) {
					Gizmos.DrawCube(node.Position.ToVector3(), gizmoSize);
				}
			}

			/* Draw closed nodes */
			Gizmos.color = Color.red;

			if (this.closedNodes != null) {
				foreach (var node in this.closedNodes) {
					Gizmos.DrawCube(node.Position.ToVector3(), gizmoSize);
				}
			}

			/* Draw current node */
			if (currentNode != null) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(currentNode.Position.ToVector3(), gizmoSize);
			}

			/* Draw valid nodes */
			Gizmos.color = Color.yellow;
			
			if (this.validNodes != null) {
				foreach (var node in this.validNodes) {
					Gizmos.DrawCube(node.Position.ToVector3(), gizmoSize);
				}
			}
		}
	}

}