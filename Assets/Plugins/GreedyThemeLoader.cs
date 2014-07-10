using UnityEngine;
using System.Collections;
using System;
using System.IO;
using GreedyAds;

public class GreedyThemeLoader : MonoBehaviour {

	public string PostLevel = null;	
	public GUIStyle LaterBtnStyle, DownloadBtnStyle;
	public float BtnWidth, BtnHeight;
	public GUITexture loading;	
	private ThemeManager themeManager = null;	

	void Awake(){		
		themeManager = ThemeManager.Instance;
		if(themeManager.Supported){
			themeManager.init (PostLevel);
		}else{
			Application.LoadLevel (PostLevel);
		}
	}

	void OnGUI () {
		if(themeManager.isNewContent){
			loading.enabled = false;
			Rect a = new Rect (0, Screen.height - 200, Screen.width*themeManager.progress/100.0f, 30);
			DrawRectangle (a, Color.black);
			if(themeManager.isForced == false){
				if (GUI.Button(new Rect ( (Screen.width - BtnWidth)/2 - BtnWidth/2, Screen.height - 100, BtnWidth, BtnHeight), "Next Time", LaterBtnStyle)) {
					themeManager.cancelDownload();
				}
				if (GUI.Button(new Rect ( (Screen.width - BtnWidth)/2 + BtnWidth/2, Screen.height - 100, BtnWidth, BtnHeight), "Download", DownloadBtnStyle)) {
					themeManager.backgroundDownload();
				}
			}
		}
	}

	void DrawRectangle (Rect position, Color color) {    
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0,color);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(position, GUIContent.none);
	}
}
