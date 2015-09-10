using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;

namespace SpaceStation.Camera {

	[System.Serializable]
	public class FollowCameraSpeed {

		public float Move = 3.0f;
		public float Zoom = 5.0f;
		public float Rotate = 0.25f;
	}

	public class FollowCameraController : MonoBehaviour {

		public Transform FollowTarget;
		public UnityEngine.Camera Camera;
	
		public FollowCameraSpeed Speed;

		[Range(0.0f, 1.0f)]
		public float Zoom = 0.0f;

		[Header("Zoomed Out")]
		public Vector3 ZoomedOutPosition;
		public Vector3 ZoomedOutTargetOffset;

		[Header("Zoomed In")]
		public Vector3 ZoomedInPosition;
		public Vector3 ZoomedInTargetOffset;

		private Vector3 targetPosition;
		private float targetRotation;

		private float currentZoom = 1;
		private Vector3 positionVelocity;
		private float zoomVelocity;
		private float rotationVelocity;

		private Vector3 lastMousePosition;
	
		private void Update() {

			UpdateInput();

			/* Update camera target positions and zoom */
			if (this.FollowTarget != null) {
				UpdateCameraTarget();
				UpdateCameraZoom();
			}
		}

		private void UpdateInput() {
			var scrollDelta = Input.GetAxis("MouseScroll");

			if (Input.GetMouseButton(0) && Input.mousePosition != this.lastMousePosition) {
				this.targetRotation += (Input.mousePosition - this.lastMousePosition).x * this.Speed.Rotate;
			}

			/* Update zoom factor */
			if (scrollDelta != 0.0f) {
				this.Zoom = Mathf.Clamp(this.Zoom + scrollDelta, 0.0f, 1.0f);
			}

			this.lastMousePosition = Input.mousePosition;
		}

		private void UpdateCameraTarget() {

			/* Set camera target position */
			if (this.targetPosition != this.FollowTarget.position) {
				this.targetPosition = this.FollowTarget.position;
			}

			/* Smooth damp actual position to target */
			if (this.transform.position != this.targetPosition) {

				this.transform.position = Vector3.SmoothDamp(
					this.transform.position, 
					this.targetPosition, 
					ref positionVelocity, 
					1 / this.Speed.Move
				);
			}

			/* Set camera rotation to target rotation */
			if (this.targetRotation != this.transform.eulerAngles.y) {
				var yRotation = Mathf.SmoothDampAngle(
					this.transform.eulerAngles.y,
					this.targetRotation,
					ref this.rotationVelocity,
					Time.deltaTime / this.Speed.Rotate
				);

				this.transform.rotation = Quaternion.Euler(0, yRotation, 0);
			}
		}

		private void OnGUI() {
			GUI.Label(new Rect(10, 10, 200, 20), "Target Rotation: " + this.targetRotation);
		}

		private void UpdateCameraZoom() {

			/* Smooth damp zoom level and adjust camera position */
			if (this.currentZoom != this.Zoom) {

				if (Mathf.Abs(this.currentZoom - this.Zoom) < 0.001f) {
					this.currentZoom = this.Zoom;
				} else {
					this.currentZoom = Mathf.SmoothDamp(
						this.currentZoom, 
						this.Zoom, 
						ref zoomVelocity, 
						1 / this.Speed.Zoom
					);
				}

				var lerpedOffset = Vector3.Lerp(this.ZoomedOutTargetOffset, this.ZoomedInTargetOffset, this.currentZoom);
			
				this.Camera.transform.localPosition = Vector3.Lerp(this.ZoomedOutPosition, this.ZoomedInPosition, this.currentZoom);
				this.Camera.transform.LookAt(this.transform.position + lerpedOffset);
			}
		}

		private void OnDrawGizmosSelected() {

			/* Draw line for zoom positions */
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.transform.position + ZoomedOutPosition, this.transform.position + ZoomedInPosition);

			/* Draw line for zoom offsets */
			Gizmos.color = Color.red;
			Gizmos.DrawLine(this.transform.position + this.ZoomedOutTargetOffset, this.transform.position + this.ZoomedInTargetOffset);

			/* Connect positions and offsets */
			Gizmos.color = Color.green;
			Gizmos.DrawLine(this.transform.position + ZoomedOutPosition, this.transform.position + this.ZoomedOutTargetOffset);
			Gizmos.DrawLine(this.transform.position + ZoomedInPosition, this.transform.position + this.ZoomedInTargetOffset);
		}
	}

}