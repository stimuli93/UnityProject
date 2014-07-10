using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using GreedyAds;
using UnityEditor;

[CustomEditor(typeof(ThemeUnitSprite))]
public class ThemeUnitSpriteEditor : Editor {
	public override void OnInspectorGUI() {
		ThemeUnitSprite myTarget = (ThemeUnitSprite)target;
		if (!myTarget.renderer) {
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("SpriteRenderer component not found.", MessageType.Error, true);
		}else if(!myTarget.texture){
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("No texture attached to SpriteRenderer.", MessageType.Error, true);		
		}else if(string.IsNullOrEmpty(myTarget.stagePath)){
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("Unit is not added to list. Perform, Theme > DynamicUnits > Refresh list.", MessageType.Error, true);		
		}else if(string.IsNullOrEmpty(myTarget.Namespace)){
			GUI.color = Color.yellow;
			EditorGUILayout.HelpBox ("Unit namespace is not found. Perform, Theme > DynamicUnits > Sync list.", MessageType.Error, true);		
		}else{
			GUI.color = Color.green;
			EditorGUILayout.HelpBox ("SpriteRenderer component is marked to be used for i18n dynamically.", MessageType.Info, true);
		}
		GUI.color = Color.white;
	}
}
