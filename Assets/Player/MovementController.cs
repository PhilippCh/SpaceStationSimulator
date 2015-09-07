using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure;
using SpaceStation.Station.Structure.Cell;
using SpaceStation.Character;
using System.Collections.Generic;

namespace SpaceStation.Player {

	public class MovementController : MonoBehaviour {

		private const float MAX_CAST_DISTANCE = 10f;

		public float Speed = 2f;
		public float RotationSpeed = 5f;

		public Animator Animator;

		private LayerMask structureLayer;
		private List<PathNode> pathToTarget;

		private bool isWalking = false;

		private void Awake() {
			structureLayer = LayerMask.GetMask("structure");
		}

		private void Update () {
	
			if (Input.GetMouseButtonDown(0)) {
				SelectTargetCell();
			}
		}

		private void SelectTargetCell() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;

			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
				return;
			}

			if (!Physics.Raycast(ray, out hitInfo, MAX_CAST_DISTANCE, structureLayer)) {
				Logger.Info("Hit nothing :(");
				return;
			}
			
			var gridPos = hitInfo.transform.position.ToIntVector3();
			var targetCell = GetClosestWalkableCell(gridPos);
			
			if (targetCell != null) {
				Logger.Info("Hit at {0}.", targetCell.Position);

				var pathfinder = this.GetComponent<Pathfinder>();

				StartCoroutine(pathfinder.FindPath(transform.position.ToIntVector3(), targetCell.Position, PathfinderCallback));
			} else {
				Logger.Info("Could not find walkable cell in range.");
			}
		}

		private void PathfinderCallback(bool success, List<PathNode> path) {
			if (!success) {
				return;
			}

			this.pathToTarget = path;

			if (!isWalking) {
				isWalking = true;
				StartCoroutine(WalkToTarget());
			}
		}

		private IEnumerator WalkToTarget() {
			var animator = GetComponent<Animator>();

			animator.SetBool("moving", true);

			while (this.pathToTarget.Count > 0) {
				var nextNode = this.pathToTarget.PopAt(0);

				yield return StartCoroutine(WalkToNode(nextNode));
			}

			isWalking = false;
			animator.SetBool("moving", false);
		}

		private IEnumerator WalkToNode(PathNode node) {
			var targetPosition = node.Position.ToVector3();

			while (targetPosition != transform.position) {
				
				/* Linearly move player to target cell */
				this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
				
				var _direction = (targetPosition - transform.position).normalized;
				var _lookRotation = Quaternion.LookRotation(_direction);
				
				//rotate us over time according to speed until we are in the required rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);

				yield return null;
			}
		}

		private CellDefinition GetClosestWalkableCell(IntVector3 position) {
			var regionManager = RegionManager.Instance;
			var centerCell = regionManager.GetCellAt(position);

			/* If our target is a floor tile, return it */
			if (centerCell.floor != null) {
				return centerCell;
			}

			/* If not, search in a 9x9 grid around the target to find a walkable cell */
			foreach (var offset in CellRange.Values2D(false)) {
				var absPosGrid = offset + position.ToIntVector2();
				var cell = regionManager.GetCellAt(absPosGrid.ToIntVector3(position.y));
				
				if (cell != null && cell.floor != null) {
					return cell;
				}
			}

			return null;
		}
	}

}