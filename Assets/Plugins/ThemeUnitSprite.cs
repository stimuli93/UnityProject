using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using GreedyAds;
using System.IO;

public class ThemeUnitSprite : MonoBehaviour {
    public string Namespace = null;
	public string stagePath = null;

	public SpriteRenderer renderer{
		get	{
			return GetComponent<SpriteRenderer>();
		}
	}
	public Texture2D texture{
		get	{
			if(renderer != null && renderer.sprite != null){
				return renderer.sprite.texture;
			}else{				
				Debug.LogError ("AdunitSprite no texture found.");
				return null;
			}
		}
	}

	public GameObject getGameObject() {
		return gameObject;
	}

    void Awake() {
        Debug.Log(string.Format("Awake {0}, with namespace = {1}", gameObject.name, Namespace));
        if (!renderer)
        {
            Debug.LogError("AdUnitSprite no renderer found " + gameObject.name);
        }
        SetUpTexture();
    }

	// Use this for initialization
	public void SetUpTexture() {
		Debug.Log (Application.dataPath + "/StreamingAssets");
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
			try {
                Texture2D a = ThemeManager.AdUnitTextures[Namespace];
                MaterialPropertyBlock block = new MaterialPropertyBlock();
				Texture.Destroy(block.GetTexture("_MainTex"));
                block.AddTexture("_MainTex", a);
                renderer.SetPropertyBlock(block);
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogWarning("Couldnt Download. Using old Texture");
            }
        }
	}

	IEnumerator _fetchUnits(string url) {
		Debug.Log ("_fetchUnits from " + url);
		WWW www = new WWW(url);
		yield return www;
		MaterialPropertyBlock block = new MaterialPropertyBlock();		
		Texture.Destroy(block.GetTexture("_MainTex"));
		block.AddTexture("_MainTex", www.texture);
		renderer.SetPropertyBlock(block);
		yield return null;
	}
}
