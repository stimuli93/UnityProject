using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using GreedyAds;
using System.IO;

public class ThemeUnitMesh : MonoBehaviour {
    public string Namespace = null;
	public string stagePath = null;
    public MeshRenderer renderer
    {
		get	{
            return GetComponent<MeshRenderer>();
		}
	}

    void Awake() {
        Debug.Log(string.Format("Awake {0}, with namespace = {1}", gameObject.name, Namespace));
        if (!renderer)
        {
            Debug.LogError("AdUnitAbstract<" + typeof(MeshRenderer) + "> no renderer found " + gameObject.name);
        }
        SetUpTexture();
    }


	public Texture2D texture{
		get	{
			Debug.Log ("AdUnitMesh texture - in");
			if(renderer != null && renderer.sharedMaterial.mainTexture != null){
                return renderer.sharedMaterial.mainTexture as Texture2D;
			}else{
				return null;
			}
		}
	}

	public GameObject getGameObject() {
		Debug.Log ("AdunitSprite gameObject - in " + gameObject.name);
		return gameObject;
	}


	// Use this for initialization
	public void SetUpTexture() {
		if (Application.isEditor)
		{
			if(String.IsNullOrEmpty(stagePath)) {
				Debug.Log(gameObject.name + " adunit out of sync");
            }
			else if (ThemeManager.isDevEnable)
			{	
				string stageUnitPath = String.Format("{0}/{1}", Directory.GetParent(Application.dataPath), stagePath);
				if (File.Exists(stageUnitPath))
				{
					StartCoroutine(_fetchUnits("file://" + stageUnitPath));
				}
			}
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			try{
				Texture2D a = ThemeManager.AdUnitTextures[Namespace];
				//Texture.Destroy(renderer.sharedMaterials [0].mainTexture);
				renderer.sharedMaterials [0].mainTexture = a;
			}catch(KeyNotFoundException e){
				Debug.LogWarning("Couldnt Download. Using old Texture");
			}
		}
	}

	IEnumerator _fetchUnits(string url) {
		Debug.Log ("_fetchUnits from " + url);
		WWW www = new WWW(url);
		yield return www;
	//	Texture.DestroyImmediate(renderer.materials [0].mainTexture,true);
		renderer.materials[0].mainTexture = www.texture;
		yield return null;
	}
}
