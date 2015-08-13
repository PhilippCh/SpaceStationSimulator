using UnityEngine;
using System.Collections;

using SpaceStation;
using UnityEditor;
using SpaceStation.Util;

namespace SpaceStation.Editor {

	// Disabled for now
	//[CustomEditor(typeof(ObjectPool))]
	public class ObjectPoolInspector : UnityEditor.Editor {

		public override void OnInspectorGUI() {

			serializedObject.Update();

			// Draw startup pool mode
			EditorGUILayout.PropertyField(serializedObject.FindProperty("startupPoolMode"));

			DisplayPoolList(serializedObject.FindProperty("startupPools"));

			serializedObject.ApplyModifiedProperties();
		}

		private void DisplayPoolList(SerializedProperty list) {

			EditorGUILayout.PropertyField(list);
			EditorGUI.indentLevel += 1;

			EditorGUILayout.PropertyField(serializedObject.FindProperty("startupPoolSize"));

			if (list.isExpanded) {
				for (int i = 0; i < list.arraySize; i++) {
					var poolElement = list.GetArrayElementAtIndex(i);

					EditorHelper.DrawHorizontal(() => {
						EditorGUILayout.PropertyField(poolElement.FindPropertyRelative("size"), GUIContent.none, false, GUILayout.MaxWidth(60));
						EditorGUILayout.PropertyField(poolElement.FindPropertyRelative("prefab"), GUIContent.none);
					});
				}
			}
		}
	}

}