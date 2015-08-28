using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using System;

namespace SpaceStation {

	public class CameraController : MonoBehaviour {

		public float smoothTime = 0.3f;

		private Transform followTarget;
		private Vector3 target;

		private static Vector3 offset = new Vector3(-.5f, 4f, -.5f);

		public static CameraController GetMainController() {
			var mainCamera = Camera.main;

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

		public void MoveTo(IntVector3 target, bool instant = false) {
			Logger.QuickInfo("Moving to" + target.ToString());
			MoveTo(target.ToVector3(), instant);
		}

		public void MoveTo(Vector3 target, bool instant = false) {
			Logger.QuickInfo("Moving to" + target.ToString());

			if (instant) {
				this.transform.position = target + offset;
			} else {
				throw new NotImplementedException("Camera smooth movement not implemented.");
			}
		}
	}

}