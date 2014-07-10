using System;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using GreedyAds;
using MiniJSON;
using ICSharpCode.SharpZipLib.Tar;

[CustomEditor(typeof(DynamicUnitManager))]
public class DynamicUnitManagerEditor : Editor {	
	private Texture2D red_circle = AssetDatabase.LoadAssetAtPath("Assets/Editor/GreedyEditor/red.png", typeof(Texture2D)) as Texture2D;
    private Texture2D green_circle = AssetDatabase.LoadAssetAtPath("Assets/Editor/GreedyEditor/green.png", typeof(Texture2D)) as Texture2D;
    private Texture2D yellow_circle = AssetDatabase.LoadAssetAtPath("Assets/Editor/GreedyEditor/yellow.png", typeof(Texture2D)) as Texture2D;
	private BackgroundWorker bw;
	private DynamicUnitManager unitManager;
	private SerializedProperty unit,list;
	private Dictionary<string, string> pathNamespace = new Dictionary<string, string> ();

	private PanelUtilities utilities;
	private string savedUserEmail, userEmail, userPassword;
	private string loginButton = "Sign in";
	private string uploadButton = "Sync";

	void OnEnable(){
		unitManager = DynamicUnitManager.Instance;//(DynamicUnitManager)target;
		list = serializedObject.FindProperty ("Units");
		utilities = new PanelUtilities ();		
		savedUserEmail = utilities.getUserEmail ();
        Debug.Log("DynamicUnitManagerEditor - OnEnable");
	}

	private Vector2 scrollPosition;
	private int LoginStatus;

	public override void OnInspectorGUI() {
		//DrawDefaultInspector();
		//Event e = Event.current;

        if (unitManager == null) 
        {
            unitManager = DynamicUnitManager.Instance;
        }

		if(savedUserEmail != null){			
			unitManager.GameProfile = EditorGUILayout.TextField ("Game Id", unitManager.GameProfile);		
			unitManager.EnableTheme = EditorGUILayout.Toggle ("Theme Enable", ThemeManager.isDevEnable);
			
			if (GUI.changed){
				EditorUtility.SetDirty(unitManager);
				if(unitManager.EnableTheme){
					String fileDev = Application.persistentDataPath+"/DevThemeon";
					if(File.Exists(fileDev) == false){
						File.Create(fileDev);
					}
				}else{
					String fileDev = Application.persistentDataPath+"/DevThemeon";
					if(File.Exists(fileDev)){
						File.Delete(fileDev);
					}
				}
			}

			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width - 20), GUILayout.Height(Screen.height - 290));
			DrawInspector();		
			GUILayout.EndScrollView();

			GUILayout.Space (20);
			GUILayout.BeginHorizontal ();
				GUILayout.Space (Screen.width / 10f);
				if(GUILayout.Button("Refresh", GUILayout.Width((3f/(4f*3))*Screen.width), GUILayout.Height(34))) {
					buildList ();
					EditorUtility.SetDirty(unitManager);				
					AssetDatabase.SaveAssets ();					
				}
				if(GUILayout.Button(uploadButton, GUILayout.Width((3f/(4f*3))*Screen.width), GUILayout.Height(34))) {
					if(!String.IsNullOrEmpty(unitManager.GameProfile))
						syncList();
					else
						EditorUtility.DisplayDialog("Sync Error","Please Enter Game Profile","Ok");
				}
				if(GUILayout.Button("Export", GUILayout.Width((3f/(4f*3))*Screen.width), GUILayout.Height(34))) {
					exportList ();
				}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
				GUILayout.Space (Screen.width / 10f);
				if(GUILayout.Button("Log out ("+savedUserEmail+")", GUILayout.Width((3f/3.9f)*Screen.width), GUILayout.Height(34))) {
					utilities.userLogOut();
					savedUserEmail = utilities.getUserEmail();
				}
			GUILayout.EndHorizontal ();
		}else{
			GUILayout.Label ("GreedyGame Account", EditorStyles.boldLabel);
			if(loginButton.CompareTo("Sign out") != 0){ 
				userEmail = EditorGUILayout.TextField ("Email", userEmail);	
				userPassword = EditorGUILayout.PasswordField ("Password", userPassword);
			}
			if(LoginStatus > 200) {
				EditorGUILayout.HelpBox ("Can not be login with provided set of email and passowrd.", MessageType.Error, true);
			}else if(savedUserEmail == null){
				savedUserEmail = utilities.getUserEmail();
			}
			GUILayout.BeginHorizontal ();
			GUILayout.Space (Screen.width / 8f);
			if(GUILayout.Button(loginButton, GUILayout.Width((3f/4f)*Screen.width), GUILayout.Height(28))) {
				LoginStatus = utilities.userLogin(userEmail, userPassword);
			}
			GUILayout.EndHorizontal ();
		}
	}

	void DrawInspector () {
		EditorGUILayout.PropertyField(list);
		EditorGUI.indentLevel += 1;
		if (list.isExpanded) {
			for (int i = 0; i < list.arraySize; i++) {
				SerializedProperty unit = list.GetArrayElementAtIndex(i);
				GUILayout.BeginHorizontal();

				UnitItem u = unitManager.Units[i];
				Texture2D tex = (Texture2D)unit.FindPropertyRelative("texture").objectReferenceValue;
				if(tex){
					if(!String.IsNullOrEmpty(u.unitId)){					
						GUILayout.Label(green_circle,GUILayout.Width(16),GUILayout.Height(16));
					}else{					
						GUILayout.Label(yellow_circle,GUILayout.Width(16),GUILayout.Height(16));
					}
				}else{					
					GUILayout.Label(red_circle,GUILayout.Width(16),GUILayout.Height(16));
				}
				
				EditorGUILayout.PropertyField(unit);
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel += 1;
				if (unit.isExpanded) {
					string name = "Texute is missing!";
					GUILayout.BeginHorizontal();
					GUILayout.Space(Screen.width/8);
					if(tex != null){
						name = tex.name;
						float ratio = Screen.width/(3.5f*tex.width);
						GUILayout.Box(tex,GUILayout.Width(Screen.width/3.5f),GUILayout.Height(ratio*tex.height));
					}
					GUILayout.BeginVertical();	
					GUILayout.Label(String.Format("Name: {0}",name));
					string  scenePath = (string)unit.FindPropertyRelative("scenePath").stringValue;
					GUILayout.Label(String.Format("Scene: {0}",scenePath));
					if(String.IsNullOrEmpty(u.unitId)){
						GUILayout.Label(String.Format("Unit-Id: {0}","Sync reqd!"));
					}else{
						GUILayout.Label(String.Format("Unit-Id: {0}",u.unitId));
					}
					if(GUILayout.Button("GameObject", GUILayout.Width((3f/8f)*Screen.width), GUILayout.Height(28))) {
                        Selection.activeGameObject = GameObject.Find(u.gameObjectName);
					}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel -= 1;
			}
		}
		EditorGUI.indentLevel -= 1;
	}

	/*
	 * BUILD LIST
	 */
	void buildList () {		
		EditorApplication.SaveCurrentSceneIfUserWantsTo ();		
		EditorUtility.DisplayProgressBar ("Building dynamic assets list", "", 0);
		string currentScene = EditorApplication.currentScene;
		Dictionary<string, UnitItem> storedUnitIds = new Dictionary<string, UnitItem>();		
		unitManager = DynamicUnitManager.Instance;
        if (unitManager.Units == null)
        {
            unitManager.Units = new List<UnitItem>();
        }
        foreach (UnitItem ut in unitManager.Units)
        {
            if (storedUnitIds.ContainsKey(ut.path) == false)
            {
                storedUnitIds.Add(ut.path, ut);
            }
		}
        unitManager.Units.Clear();

		//HashSet<string> currentUnitIds = new HashSet<string>();

		for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
			EditorBuildSettingsScene scene = EditorBuildSettings.scenes [i];
			EditorApplication.OpenScene (scene.path);
			EditorUtility.DisplayProgressBar ("Building dynamic assets list", scene.path, i*1.0f / EditorBuildSettings.scenes.Length);
			//Sprite
			foreach (ThemeUnitSprite y in Resources.FindObjectsOfTypeAll<ThemeUnitSprite> ()) {
				Texture2D texture = y.texture;
				string path = AssetDatabase.GetAssetPath (texture);				
				string md5 = null;
				UnitItem u = null;
				bool isUnitChanged = false;
				if(!String.IsNullOrEmpty(path)){		
					md5 = PanelUtilities.getFileMD5(path);
					if(storedUnitIds.ContainsKey(path)){
						u = storedUnitIds[path];
					}
				}
				
				if(u == null){ 
					u = new UnitItem ();
				}
				
				if( !String.IsNullOrEmpty(path) && texture != null){
					if( (!String.IsNullOrEmpty(md5) && !md5.Equals(u.assetMD5)) || !File.Exists(PanelUtilities.getTempOriginalFilePath(path))){						
						utilities.addFileToTemp(path, texture);
					}
				}
				u.assetMD5 = md5;
				u.path = path;
				u.texture = texture;
				u.gameObjectName = y.gameObject.name;
				u.scenePath = scene.path;
				u.instanceId = y.GetInstanceID ();
				unitManager.Units.Add (u);
				y.stagePath = PanelUtilities.getStageFilePath (u.path);
				EditorUtility.SetDirty (y);
			}
			//Cloth
			foreach (ThemeUnitCloth y in Resources.FindObjectsOfTypeAll<ThemeUnitCloth> ()) {
				Texture2D texture = y.texture;
				string path = AssetDatabase.GetAssetPath (texture);				
				string md5 = null;
				UnitItem u = null;
				bool isUnitChanged = false;
				if(!String.IsNullOrEmpty(path)){		
					md5 = PanelUtilities.getFileMD5(path);
					if(storedUnitIds.ContainsKey(path)){
						u = storedUnitIds[path];
					}
				}
				
				if(u == null){ 
					u = new UnitItem ();
				}
				
				if( !String.IsNullOrEmpty(path) && texture != null){
					if( (!String.IsNullOrEmpty(md5) && !md5.Equals(u.assetMD5)) || !File.Exists(PanelUtilities.getTempOriginalFilePath(path))){						
						utilities.addFileToTemp(path, texture);
					}
				}
				u.assetMD5 = md5;
				u.path = path;
				u.texture = texture;
				u.gameObjectName = y.gameObject.name;
				u.scenePath = scene.path;
				u.instanceId = y.GetInstanceID ();
				unitManager.Units.Add (u);
				y.stagePath = PanelUtilities.getStageFilePath (u.path);
				EditorUtility.SetDirty (y);
			}
			//Mesh
			foreach (ThemeUnitMesh y in Resources.FindObjectsOfTypeAll<ThemeUnitMesh> ()) {
				Texture2D texture = y.texture;
				string path = AssetDatabase.GetAssetPath (texture);				
				string md5 = null;
				UnitItem u = null;
				bool isUnitChanged = false;
				if(!String.IsNullOrEmpty(path)){		
					md5 = PanelUtilities.getFileMD5(path);
					if(storedUnitIds.ContainsKey(path)){
						u = storedUnitIds[path];
					}
				}
				
				if(u == null){ 
					u = new UnitItem ();
				}

				if( !String.IsNullOrEmpty(path) && texture != null){
					if( (!String.IsNullOrEmpty(md5) && !md5.Equals(u.assetMD5)) || !File.Exists(PanelUtilities.getTempOriginalFilePath(path))){						
						utilities.addFileToTemp(path, texture);
					}
				}
				u.assetMD5 = md5;
				u.path = path;
				u.texture = texture;
				u.gameObjectName = y.gameObject.name;
				u.scenePath = scene.path;
				u.instanceId = y.GetInstanceID ();
				unitManager.Units.Add (u);
				y.stagePath = PanelUtilities.getStageFilePath (u.path);
				EditorUtility.SetDirty (y);
			}
			//SkinnedMesh
			foreach (ThemeUnitSkinnedMesh y in Resources.FindObjectsOfTypeAll<ThemeUnitSkinnedMesh> ()) {
				Texture2D texture = y.texture;
				string path = AssetDatabase.GetAssetPath (texture);				
				string md5 = null;
				UnitItem u = null;
				bool isUnitChanged = false;
				if(!String.IsNullOrEmpty(path)){		
					md5 = PanelUtilities.getFileMD5(path);
					if(storedUnitIds.ContainsKey(path)){
						u = storedUnitIds[path];
					}
				}
				
				if(u == null){ 
					u = new UnitItem ();
				}
				
				if( !String.IsNullOrEmpty(path) && texture != null){
					if( (!String.IsNullOrEmpty(md5) && !md5.Equals(u.assetMD5)) || !File.Exists(PanelUtilities.getTempOriginalFilePath(path))){						
						utilities.addFileToTemp(path, texture);
					}
				}
				u.assetMD5 = md5;
				u.path = path;
				u.texture = texture;
				u.gameObjectName = y.gameObject.name;
				u.scenePath = scene.path;
				u.instanceId = y.GetInstanceID ();
				unitManager.Units.Add (u);
				y.stagePath = PanelUtilities.getStageFilePath (u.path);
				EditorUtility.SetDirty (y);
			}
			EditorApplication.SaveAssets ();
			EditorApplication.SaveScene ();
		}
		EditorUtility.ClearProgressBar ();
		EditorApplication.OpenScene (currentScene);
	}

	void syncList() {
		EditorApplication.SaveCurrentSceneIfUserWantsTo ();
		EditorUtility.DisplayProgressBar ("Syncing assets", "", 0);
		for (int i = 0; (i < unitManager.Units.Count); i++) {
			UnitItem u = unitManager.Units[i];			
			EditorUtility.DisplayProgressBar("Syncing assets", u.path, i*1.0f/unitManager.Units.Count);
			string responseAssetJson = null;
			if(!String.IsNullOrEmpty(u.path)){
				string path = PanelUtilities.getTempOriginalFilePath(u.path);
				responseAssetJson = utilities.checkMD5(path, unitManager.GameProfile, u.path);
				if(responseAssetJson == null){
					string folder = Path.GetDirectoryName (u.path);
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("folder", folder);
					responseAssetJson = utilities.HttpUploadFile(path, "creative", nvc, unitManager.GameProfile);
				}else{
					Debug.Log (String.Format("[Already synced] {0}", u.path));
				}
				
				if(responseAssetJson == null){
					EditorUtility.ClearProgressBar ();
					return;
				}
				
				var dict = Json.Deserialize(responseAssetJson) as Dictionary<string, object>;
				string unitNamespace = null;
				try{
					unitNamespace = (string)dict["namespace"];
				}catch(Exception ex) {
					Debug.LogWarning(ex.ToString());
				}
				u.unitId = unitNamespace;					
				if(pathNamespace.ContainsKey(u.path) == false){
					pathNamespace.Add(u.path, unitNamespace);
				}
			}
		}		
		EditorUtility.SetDirty(unitManager);
		AssetDatabase.SaveAssets ();

		/* 
		 * Writing Android values.xml
		 */
		using(XmlWriter writer = XmlWriter.Create ("Assets/Plugins/Android/res/values/dynamic_themes.xml"))
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement ("resources");			
			writer.WriteStartElement ("string");
			writer.WriteAttributeString ("name", "gg_game_id");
			writer.WriteRaw (unitManager.GameProfile);
			writer.WriteEndElement ();
			writer.WriteStartElement ("string-array");
			writer.WriteAttributeString ("name", "gg_units");

			string[] totalUnits = new string[pathNamespace.Keys.Count];
			pathNamespace.Keys.CopyTo(totalUnits, 0);
			for (int i = 0; (i < totalUnits.Length); i++) {
				EditorUtility.DisplayProgressBar("Creating Android values list", totalUnits[i], i*1.0f/totalUnits.Length);
				writer.WriteStartElement ("item");
				writer.WriteRaw (pathNamespace[totalUnits[i]]);
				writer.WriteEndElement ();	
			}			
			writer.WriteEndElement ();
			writer.WriteEndElement ();
			writer.WriteEndDocument ();
		}
		
		string currentScene = EditorApplication.currentScene;
		int unit_counter = 0;
		foreach (EditorBuildSettingsScene x in EditorBuildSettings.scenes) {
			EditorApplication.OpenScene (x.path);
			//AdUnitSprite
			foreach (ThemeUnitSprite y in Resources.FindObjectsOfTypeAll<ThemeUnitSprite> ()) {
				if(y.texture != null){
					string path = AssetDatabase.GetAssetPath(y.texture);
					y.Namespace = pathNamespace[path];
					EditorUtility.SetDirty(y);
					EditorUtility.DisplayProgressBar("Syncing unit namepace", path, unit_counter*1.0f/unitManager.Units.Count);
				}
				unit_counter++;
			}
			//AdUnitCloth
			foreach (ThemeUnitCloth y in Resources.FindObjectsOfTypeAll<ThemeUnitCloth> ()) {
				if(y.texture != null){
					string path = AssetDatabase.GetAssetPath(y.texture);
					y.Namespace = pathNamespace[path];
					EditorUtility.SetDirty(y);
					EditorUtility.DisplayProgressBar("Syncing unit namepace", path, unit_counter*1.0f/unitManager.Units.Count);
				}
				unit_counter++;
			}
			//AdUnitMesh
			foreach (ThemeUnitMesh y in Resources.FindObjectsOfTypeAll<ThemeUnitMesh> ()) {
				if(y.texture != null){
					string path = AssetDatabase.GetAssetPath(y.texture);
					y.Namespace = pathNamespace[path];
					EditorUtility.SetDirty(y);
					EditorUtility.DisplayProgressBar("Syncing unit namepace", path, unit_counter*1.0f/unitManager.Units.Count);
				}
				unit_counter++;
			}
			//AdUnitSkinnedMesh
			foreach (ThemeUnitSkinnedMesh y in Resources.FindObjectsOfTypeAll<ThemeUnitSkinnedMesh> ()) {
				if(y.texture != null){
					string path = AssetDatabase.GetAssetPath(y.texture);
					y.Namespace = pathNamespace[path];
					EditorUtility.SetDirty(y);
					EditorUtility.DisplayProgressBar("Syncing unit namepace", path, unit_counter*1.0f/unitManager.Units.Count);
				}
				unit_counter++;
			}
			
			EditorApplication.SaveAssets();
			EditorApplication.SaveScene();
		}
		EditorApplication.OpenScene (currentScene);
		EditorUtility.ClearProgressBar ();
	}
	
	void exportList () {
		EditorUtility.DisplayProgressBar ("Exporting modified assets", "", 0);		
		string tarFilePath = EditorUtility.SaveFilePanel("Export Theme","ThemeAssets/","theme","tar");
		Stream outStream = File.Create(tarFilePath);
		string sourceDirectory = "ThemeAssets/export";
        if (!Directory.Exists(sourceDirectory))
        {
            Directory.CreateDirectory(sourceDirectory);
        }

		TarArchive tarArchive = TarArchive.CreateOutputTarArchive (outStream);
		tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
		if (tarArchive.RootPath.EndsWith("/")){
			tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
		}

		HashSet<string> exportedPath = new HashSet<string>();

		for (int i = 0; i < unitManager.Units.Count; i++) {
			UnitItem u = unitManager.Units [i];
			EditorUtility.DisplayProgressBar ("Exporting modified assets", PanelUtilities.getStageFilePath(u.path), i / unitManager.Units.Count);
			if (String.IsNullOrEmpty(u.path) == false) {
				string stagePath = PanelUtilities.getStageFilePath(u.path);
				string stageMd5 = PanelUtilities.getFileMD5 (stagePath);
				string originalMd5 = PanelUtilities.getFileMD5 (PanelUtilities.getTempOriginalFilePath(u.path));

				if (stageMd5.Equals (originalMd5) == false) {
					string exportFile = PanelUtilities.getExportFilePath(u.path);
					string folder = Path.GetDirectoryName (exportFile);
					bool isExists = System.IO.Directory.Exists (folder);
					if (!isExists) {
						Debug.Log("creating "+folder);
						Directory.CreateDirectory (folder);
					}
					bool isAssetExists = System.IO.File.Exists (exportFile);
					if (isAssetExists) {
						File.Delete (exportFile);
					}
					System.IO.File.Copy (stagePath, exportFile);
                    
					if(!exportedPath.Contains(exportFile)){
                    	TarEntry tarEntry = TarEntry.CreateEntryFromFile(exportFile);
                    	tarArchive.WriteEntry(tarEntry, false);
						exportedPath.Add(exportFile);
					}
				}
			}
		}

		tarArchive.Close();
		EditorUtility.ClearProgressBar ();
	}

}
