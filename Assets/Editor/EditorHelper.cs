using UnityEngine;
using UnityEditor;
using System.Collections;

using System;
using System.IO;

namespace SpaceStation.Editor {
	
	public static class EditorHelper {
		
		public const int TAB_WIDTH = 15;
		public const float MINI_BUTTON_WIDTH = 20f;
		
		#region Drawing
		
		public static void IndentSpace(int indentLevel) {
			GUILayout.Space(indentLevel * TAB_WIDTH);
		}
		
		public static void DrawHorizontal(Action DrawInnerItems) {
			EditorGUILayout.BeginHorizontal();
			
			DrawInnerItems();
			
			EditorGUILayout.EndHorizontal();
		}
		
		#endregion
		
		#region Assets
		
		public static void CreateAsset<T> () where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			
			if (path == "") {
				path = "Assets";
			} else if (Path.GetExtension(path) != "") {
				path = path.Replace(Path.GetFileName(path), "");
			}
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
			
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
		
		#endregion
	}
	
}