using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using System;

namespace SpaceStation.Camera {

	public class CameraController : MonoBehaviour {

		public float SmoothTime = 0.3f;

		public Vector3 OffsetPosition = new Vector3(-.5f, 4f, -.5f);
		public Vector3 OffsetRotation;

		public Transform Target;

		public static CameraController GetMainController() {
			var mainCamera = UnityEngine.Camera.main;

			if (mainCamera == null) {
				Logger.Error("No main camera present in scene.");
				return null;
			}

			var controller = mainCamera.GetComponent<CameraController>();

			if (controller == null) {
				Logger.Error("Main camera has no controller attached.");
				return null;
			}

			return controller;
		}

		private void LateUpdate() {
			if (!this.Target) {
				return;
			}

			this.transform.position = this.Target.transform.position + this.OffsetPosition;
			this.transform.eulerAngles = this.OffsetRotation;
		}
	}

}