using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitItem {
	public string path;
	public Texture2D texture;
	public string gameObjectName;
	public string scenePath;
	public string unitId;
	public int instanceId;
	public string assetMD5;

	public UnitItem(){
		this.path = null;
		this.texture = null;
		this.unitId = null;
		this.gameObjectName = null;
		this.scenePath = null;
		this.instanceId = 0;
		this.assetMD5 = null;
	}
}
