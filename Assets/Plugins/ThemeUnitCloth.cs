using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using GreedyAds;
using System.IO;

public class ThemeUnitCloth : MonoBehaviour {
    public string Namespace = null;
	public string stagePath = null;

	public ClothRenderer renderer{
		get	{
			return GetComponent<ClothRenderer>();
		}
	}

    public Texture2D texture {
        get {
            if (renderer != null && renderer.sharedMaterial.mainTexture != null)
            {
                return renderer.sharedMaterial.mainTexture as Texture2D;
            }
            else
            {
                return null;
            }
        }
    }

	/*public GameObject getGameObject() {
		return gameObject;
	}*/

    void Awake() {
        Debug.Log(string.Format("Awake {0}, with namespace = {1}", gameObject.name, Namespace));
        if (!renderer)
        {
            Debug.LogError("AdUnitAbstract<" + typeof(ClothRenderer) + "> no renderer found " + gameObject.name);
        }
        SetUpTexture();
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
				Texture.Destroy(renderer.materials [0].mainTexture);
				renderer.materials [0].mainTexture = a;
			}catch(KeyNotFoundException e){
				Debug.LogWarning("Couldnt Download. Using old Texture");
			}
		}
	}
	
	IEnumerator _fetchUnits(string url) {
		Debug.Log ("_fetchUnits from " + url);
		WWW www = new WWW(url);
		yield return www;
		Texture.Destroy(renderer.materials [0].mainTexture);
		renderer.materials [0].mainTexture = www.texture;
		yield return null;
	}
}
