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

		[Header("Debugging")]
		public bool PerformSteps = false;
		public bool ShowSearchNodes = false;
		public bool ShowFinalPath = true;

		[HideInInspector()]
		public bool ContinueSteps = false;

		private RegionManager regionManager;

		private IntVector3 start, end;
		private List<PathNode> openNodes, closedNodes, finalNodes;

		private PathNode currentNode, endNode;

		private void Awake() {
			this.openNodes = new List<PathNode>();
			this.closedNodes = new List<PathNode>();
			this.finalNodes = new List<PathNode>();
			
			this.regionManager = RegionManager.Instance;
		}

		public IEnumerator FindPath(IntVector3 start, IntVector3 end, Action<bool> callback) {
			currentNode = null;
			this.endNode = null;

			this.start = start;
			this.end = end;

			this.openNodes.Clear();
			this.closedNodes.Clear();
			this.finalNodes.Clear();

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
					this.endNode = currentNode;
					break;
				}

				while (!ContinueSteps && PerformSteps) {
					yield return null;
				}

				ContinueSteps = false;
				
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

			callback(this.endNode != null);
		}

		public List<PathNode> OptimizePath() {
			ReconstructPath(false);

			if (this.finalNodes.Count == 0) {
				return null;
			}

			RemoveNodesInSight(this.finalNodes[0]);

			this.finalNodes.Reverse();

			return this.finalNodes;
		}

		private void RemoveNodesInSight(PathNode node) {
			var heightOffset = new Vector3(0, 0.25f, 0);

			if (node.ParentNode == null) {
				return;
			}

			if (Physics.Linecast(node.Position + heightOffset, node.ParentNode.Position + heightOffset)) {
				RemoveNodesInSight(node.ParentNode);
			} else {
				var parentIndex = this.finalNodes.IndexOf(node.ParentNode);

				if (node.ParentNode == null) {
					return;
				}

				node.ParentNode = node.ParentNode.ParentNode;
				this.finalNodes.RemoveAt(parentIndex);

				RemoveNodesInSight(node);
			}
		}

		public List<PathNode> ReconstructPath(bool reverse = true) {
			var currentNode = endNode;

			if (this.endNode == null) {
				Logger.Warn("Cannot reconstruct path, must find it first.");
				return null;
			}

			while (true) {
				this.finalNodes.Add(currentNode);

				if (currentNode.ParentNode == null) {
					break;
				}

				currentNode = currentNode.ParentNode;
			}

			if (reverse) {
				this.finalNodes.Reverse();
			}

			return this.finalNodes;
		}

		private int Distance(IntVector3 start, IntVector3 end) {
			return Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y) + Mathf.Abs(end.z - start.z);
		}

		private int Manhattan(IntVector3 start, IntVector3 end) {
			return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) + Mathf.Abs(start.z - end.z);
		}

		#region Debugging

		private void OnDrawGizmos() {

			if (this.ShowSearchNodes) {
				DrawNodeSearchGizmos();
			}

			if (this.ShowFinalPath) {
				DrawFinalPathGizmos();
			}
		}

		private void DrawFinalPathGizmos() {
			var currentNode = this.endNode;

			if (this.endNode == null) {
				return;
			}

			Gizmos.color = Color.red;

			while (true) {
				if (currentNode.ParentNode == null) {
					break;
				}

				Gizmos.DrawLine(currentNode.Position.ToVector3(), currentNode.ParentNode.Position.ToVector3());

				currentNode = currentNode.ParentNode;
			}
		}

		private void DrawNodeSearchGizmos() {
			var gizmoSize = new Vector3(.25f, .25f, .25f);
			
			Gizmos.color = Color.green;
			
			Gizmos.DrawWireCube(this.start.ToVector3(), gizmoSize);
			Gizmos.DrawWireCube(this.end.ToVector3(), gizmoSize);
			
			/* Draw open nodes */
			Gizmos.color = Color.blue;
			
			if (this.openNodes != null) {
				foreach (var node in this.openNodes) {
					Gizmos.DrawCube(node.Position.ToVector3(), gizmoSize);
					UnityEditor.Handles.Label(node.Position.ToVector3() + new Vector3(0, .5f, 0), string.Format("G: {0}, F: {0}", node.G, node.F));
				}
			}
			
			/* Draw closed nodes */
			Gizmos.color = Color.red;
			
			if (this.closedNodes != null) {
				foreach (var node in this.closedNodes) {
					Gizmos.DrawCube(node.Position.ToVector3(), gizmoSize);
					UnityEditor.Handles.Label(node.Position.ToVector3() + new Vector3(0, .5f, 0), string.Format("G: {0}, F: {0}", node.G, node.F));
				}
			}
			
			/* Draw current node */
			if (currentNode != null) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(currentNode.Position.ToVector3(), gizmoSize);
				UnityEditor.Handles.Label(currentNode.Position.ToVector3() + new Vector3(0, .5f, 0), string.Format("G: {0}, F: {0}", currentNode.G, currentNode.F));
			}
			
			/* Draw valid nodes */
			Gizmos.color = Color.yellow;
			
			if (this.finalNodes != null) {
				foreach (var node in this.finalNodes) {
					Gizmos.DrawCube(node.Position.ToVector3(), gizmoSize);
					UnityEditor.Handles.Label(node.Position.ToVector3() + new Vector3(0, .5f, 0), string.Format("G: {0}, F: {0}", node.G, node.F));
				}
			}
		}

		#endregion
	}

}