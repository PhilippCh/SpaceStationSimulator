using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Player {

	public class MovementController : MonoBehaviour {

		private const float MAX_CAST_DISTANCE = 10f;

		public float Speed = 2f;
		public float RotationSpeed = 5f;

		public Animator Animator;

		private LayerMask structureLayer;
		private Vector3 targetPosition;

		private void Awake() {
			structureLayer = LayerMask.GetMask("structure");
			targetPosition = new Vector3(65, 64, 65);
		}

		private void Update () {
	
			if (Input.GetMouseButtonDown(0)) {
				SelectTargetCell();
			}

			Animator.SetBool("moving", targetPosition != transform.position);

			if (targetPosition != transform.position) {

				/* Linearly move player to target cell */
				this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);

				var _direction = (targetPosition - transform.position).normalized;
				
				//create the rotation we need to be in to look at the target
				var _lookRotation = Quaternion.LookRotation(_direction);
				
				//rotate us over time according to speed until we are in the required rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
			}
		}

		private void SelectTargetCell() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			
			if (!Physics.Raycast(ray, out hitInfo, MAX_CAST_DISTANCE, structureLayer)) {
				Logger.Info("Hit nothing :(");
				return;
			}
			
			var gridPos = hitInfo.transform.position.ToIntVector3();
			var targetCell = GetClosestWalkableCell(gridPos);
			
			if (targetCell != null) {
				Logger.Info("Hit at {0}.", targetCell.Position);
				targetPosition = targetCell.Position.ToVector3();
			} else {
				Logger.Info("Could not find walkable cell in range.");
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