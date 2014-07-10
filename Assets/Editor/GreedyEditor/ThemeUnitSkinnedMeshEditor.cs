using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;

[CustomEditor(typeof(ThemeUnitSkinnedMesh))]
public class ThemeUnitSkinnedMeshEditor : Editor {
	public override void OnInspectorGUI() {
		ThemeUnitSkinnedMesh myTarget = (ThemeUnitSkinnedMesh)target;
		if (!myTarget.renderer) {
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("SkinnedMeshRenderer component not found.", MessageType.Error, true);
		}else if(!myTarget.texture){
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("No texture attached to SkinnedMeshRenderer.", MessageType.Error, true);		
		}else if(string.IsNullOrEmpty(myTarget.Namespace)){
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("Unit namespace is not found. Perform, Theme > DynamicUnits > Sync list.", MessageType.Error, true);		
		}else{
			GUI.color = Color.green;
			EditorGUILayout.HelpBox ("SkinnedMeshRenderer component is marked to be used for i18n dynamically.", MessageType.Info, true);
		}
		GUI.color = Color.white;
	}
}
