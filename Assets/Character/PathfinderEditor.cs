using UnityEngine;
using System.Collections;

using SpaceStation;
using UnityEditor;
using SpaceStation.Character;

namespace SpaceStation {

	[CustomEditor(typeof(Pathfinder))]
	public class PathfinderEditor : Editor {

		public override void OnInspectorGUI() {
			var pathfinder = (Pathfinder) target;

			pathfinder.PerformSteps = EditorGUILayout.Toggle("Step-by-Step", pathfinder.PerformSteps);

			if (GUILayout.Button("Next step") && pathfinder.PerformSteps) {
				pathfinder.ContinueSteps = true;
			}
		}
	}

}