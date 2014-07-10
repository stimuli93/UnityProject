using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GreedyAds {
	public class ThemeManager : MonoBehaviour{
		private static ThemeManager instance = null;
		private static string gameObjectName = "GreedyGameRootObject";

        public const int VERSION_CODE = 11;

		public static ThemeManager Instance {
			get {
				if (instance == null) {
					//instance = new ThemeManager();					
					GameObject go = new GameObject();
					go.name = gameObjectName;
					instance = go.AddComponent<ThemeManager>();
                    DontDestroyOnLoad(go);
                    Debug.Log("ThemeManager created instance !");
				}
                Debug.Log(String.Format("[GreedyGame] ThemeManager Instance, gameObject = {0}, version_code = {1}", instance.gameObject.name, VERSION_CODE));
				return instance;
			}
		}

        private bool _isDone = false;
        public bool isDone {
			get {
                return instance._isDone;
			}
		}

/*
 * ANDROID
 */ 
		private AndroidJavaClass unity_jc;
		private AndroidJavaObject currentActivity_jo, gg_jc;

		private string[] _units;
		private float _progress = 0;
		private int _themeId = -1, _forced_update = -1;
		private bool _supported = false;		
		private bool _isNewContent = false;
       
		public bool Supported {
			get {
				return _supported;
			}
		}

		public bool isNewContent {
			get {
				return _isNewContent;
			}
		}

        public static bool isDevEnable {
			get {
				if(Application.isEditor){
					String fileDev = Application.persistentDataPath+"/DevThemeon";
					return File.Exists (fileDev);
				}
				return false;
			}
        }

		public void Dispose() {
			unity_jc.Dispose ();
			currentActivity_jo.Dispose ();
			gg_jc.Dispose ();
			unity_jc = null;
			currentActivity_jo = null;
			gg_jc = null;			
			_units = null;
		}
		
		public static Dictionary<string, Texture2D> AdUnitTextures = new Dictionary<string, Texture2D>();

		private ThemeManager() {			
			Debug.Log (String.Format("Constructor ThemeManager"));
			if(Application.platform == RuntimePlatform.Android){
				unity_jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				currentActivity_jo = unity_jc.GetStatic<AndroidJavaObject>("currentActivity");  
				gg_jc = new AndroidJavaObject("com.greedygame.android.GreedyGame", currentActivity_jo);
				_units = gg_jc.Call<string[]>("units");
				_supported = true;
				Debug.Log (String.Format("_units = {0}", _units.Length));
			}

            if (_supported == false)
            {
                _isDone = true;
            }
		}

		void Awake() {			
			/***********
			 * Just to add identitiy permission on manifest
			 */
			string c = SystemInfo.deviceUniqueIdentifier;
		}

		public void init(string _postLevel) {
			PostLevel = _postLevel;
			currentLevel = Application.loadedLevel;
			if(Application.platform == RuntimePlatform.Android){
				gg_jc.Call("gg_init", gameObjectName);
			}
			Debug.Log (String.Format ("ThemeManager init - out gameObject = {0}, platform = {1}", gameObject.name, Application.platform)); 
		}

		public float progress{
			get{
				if(_progress < 100){
					if(Application.platform == RuntimePlatform.Android){
						_progress = gg_jc.Call<float> ("getProgress");
					}
				}
				return _progress;
			}
		}

		public int activeThemeId{
			get{
				if(_themeId<0){
					if(Application.platform == RuntimePlatform.Android){
						_themeId = gg_jc.Call<int> ("activeTheme");
					}
				}
				return _themeId;						
			}
		}

		public int newThemeId{
			get{
				if(Application.platform == RuntimePlatform.Android){
					return gg_jc.Call<int> ("newTheme");
				}
				return 0;								
			}
		}
		
		public bool isForced{
			get{
				if(_forced_update<0){
					if(Application.platform == RuntimePlatform.Android){
						_forced_update = gg_jc.Call<int> ("isForceUpdate");
					}
				}
				if(_forced_update == 1){
					return true;
				}
				return false;					
			}
		}

		public string[] units{
			get{
				return _units;
			}
		}

		public void download(){
			if(Application.platform == RuntimePlatform.Android){
				gg_jc.Call("download");
			}
		}

		public void cancelDownload(){
			if(Application.platform == RuntimePlatform.Android){
				gg_jc.Call("cancelDownload");
			}
			StartCoroutine(wearActiveThemeCoroutine());
		}

		public void backgroundDownload(){			
			StartCoroutine(wearActiveThemeCoroutine());
		}

		public IEnumerator fetchAssets () {
			if(Application.platform == RuntimePlatform.Android){
				_themeId = gg_jc.Call<int> ("activeTheme");
			}
			//wait for the completion of FetchUnits
			IEnumerator e = _fetchUnits ();
			while (e.MoveNext())
				yield return e.Current;			

			Dispose ();
			yield return null;
		}
		
		private IEnumerator _fetchUnits() {
			Debug.Log ("_fetchUnits");
			ThemeManager.AdUnitTextures.Clear ();
			for(int i =0; i<units.Length; i++) {
				if(AdUnitTextures.ContainsKey(units[i])) {
					continue;
				}
				string localpath = String.Format("file://{0}/units/{1}/{2}", Application.persistentDataPath, activeThemeId, units[i]);
				Debug.Log(String.Format("Fetching from {0}", localpath));
				WWW www = new WWW (localpath);
				yield return www;
				/*
			 	* TODO : Set timeout time
				*/
				/*while( !www.isDone ) {
					progress[i] = www.progress;
					yield return null;
				}*/
				
				if (!String.IsNullOrEmpty(www.error)){
					Debug.LogError(String.Format("Error while fetching {0}", www.error));				
				}else {
					Texture2D t = www.texture as Texture2D;
					Debug.Log (www.text + "|" + String.IsNullOrEmpty(www.text));
					ThemeManager.AdUnitTextures.Add(units[i], t);
				}
				www.Dispose();

			}
			yield break;
		}


		public static string PostLevel = null;		
		private int currentLevel;	
		void GG_onInit(string __isNewContent) {
			int content = Int16.Parse (__isNewContent);
			//0 no-change
			//1 change
			//-1 default
			if(content == 1){
				_isNewContent = true;
				StartCoroutine(wearCover(Instance.newThemeId));
				Instance.download();
			}else if(content == 0){
				//No change load cached theme
				StartCoroutine(wearActiveThemeCoroutine());
			}else if(content == -1){
                _isDone = true;
				Application.LoadLevel (PostLevel);
			}
			Debug.Log(String.Format("GG_onInit content = {0}, Forced = {1}", content, Instance.isForced));
		}

		void GG_postDownload(string _theme) {
			Debug.Log(String.Format("GG_postDownload theme = {0}", _theme));
			if(currentLevel == Application.loadedLevel){
				StartCoroutine(wearActiveThemeCoroutine());
			}
		}


		IEnumerator wearActiveThemeCoroutine() {
			yield return StartCoroutine(Instance.fetchAssets());
			_isNewContent = false;
            _isDone = true;
			Application.LoadLevel (PostLevel);
			yield return null;
		}

		IEnumerator wearCover(int themeId) {
			Debug.Log("wearCover");
			string coverPath = String.Format("{0}/units/{1}/cover", Application.persistentDataPath, themeId);
			Debug.Log(String.Format("Fetching cover from {0}", coverPath));
			if(File.Exists(coverPath)){
				Debug.Log("Cover existed");
				WWW www = new WWW ("file://"+coverPath);
				yield return www;
				GameObject cover = GameObject.Find ("ggCover");
				SpriteRenderer sp = cover.GetComponent<SpriteRenderer>();
				MaterialPropertyBlock block = new MaterialPropertyBlock();
				Texture.Destroy(block.GetTexture ("_MainTex"));
				block.AddTexture("_MainTex", www.texture);
				sp.SetPropertyBlock(block);
				www.Dispose();
			}
			yield return null;
		}
	}
}


