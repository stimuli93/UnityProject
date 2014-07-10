using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

using UnityEditor;
using UnityEngine;



public class ThemeWindow : EditorWindow
{
	static string userEmail = "";
	static string userPassword = "";
	static string loginError = null;
	static string loginButton = "Sign In";
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.23f;

	[MenuItem("Themes/DynamicUnitManager")]
	public static void NewTheme() {
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = DynamicUnitManager.Instance;
	}

	void OnGUI() {
		GUILayout.Label ("Account", EditorStyles.boldLabel);
		if(loginButton.CompareTo("Sign out") != 0){ 
			userEmail = EditorGUILayout.TextField ("Email", userEmail);	
			userPassword = EditorGUILayout.PasswordField ("Password", userPassword);
		}
		if(loginError != null && loginError.Length>0) {
			EditorGUILayout.HelpBox (loginError, MessageType.Error, true);
		}

		GUILayout.BeginHorizontal ();
		GUILayout.Space (Screen.width / 8f);
		if(GUILayout.Button(loginButton, GUILayout.Width((3f/4f)*Screen.width), GUILayout.Height(28))) {
			loginButton = "Signing In ....";			

		}
		GUILayout.EndHorizontal ();
		GUILayout.Label ("Themes", EditorStyles.boldLabel);
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		myBool = EditorGUILayout.Toggle ("Toggle", myBool);
		myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup ();
	}
}