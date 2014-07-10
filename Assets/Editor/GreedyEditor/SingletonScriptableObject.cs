using UnityEditor;
using UnityEngine;
using System.Collections;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
	private static T s_Instance = null;
	
	public static T Instance {
		get {
			return GetInstance();
		}
	}
	
	private static T GetInstance() {
		// If there's no instance, load or create one
		if( s_Instance == null ) {
			string assetPathAndName = GeneratePath();

			// Check the asset database for an existing instance of the asset
			T asset = null;
			asset = AssetDatabase.LoadAssetAtPath( assetPathAndName, typeof( ScriptableObject ) ) as T;
			
			// If the asset doesn't exist, create it
			if( asset == null ) {
				asset = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset( asset, assetPathAndName );
				AssetDatabase.SaveAssets();	
			}

			s_Instance = asset;
		}
		
		return s_Instance;
	}
	
	public void SaveInstanceData() {
		EditorUtility.SetDirty( s_Instance );
		AssetDatabase.SaveAssets();
	}
	
	private static string GeneratePath() {
		return "Assets/" + typeof(T).ToString () + ".asset";
	}

}

