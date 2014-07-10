using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicUnitManager : ScriptableObject
{
	public List<UnitItem> Units;
	public string GameProfile = null;
	public bool EnableTheme = false;

    private static DynamicUnitManager s_Instance = null;

    public static DynamicUnitManager Instance
    {
		get {
			return GetInstance();
		}
	}

    private static DynamicUnitManager GetInstance()
    {
		// If there's no instance, load or create one
		if( s_Instance == null ) {
			string assetPathAndName = GeneratePath();

			// Check the asset database for an existing instance of the asset
			DynamicUnitManager asset = null;
            asset = AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(ScriptableObject)) as DynamicUnitManager;
			
			// If the asset doesn't exist, create it
			if( asset == null ) {
                asset = ScriptableObject.CreateInstance<DynamicUnitManager>();
                asset.Units = new List<UnitItem>();
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
        return "Assets/" + typeof(DynamicUnitManager).ToString() + ".asset";
	}

}

