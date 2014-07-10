using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using MiniJSON;


public class PanelUtilities {

	private static string AssetContentType = "image/*";
	private SqliteDatabase db;

	public static string TempPath = String.Format ("{0}{1}ThemeAssets{1}temp", Directory.GetParent(Application.dataPath).ToString(), Path.DirectorySeparatorChar);

	public PanelUtilities() {
		if(!Directory.Exists(TempPath)){
			Directory.CreateDirectory(TempPath);
		}
		ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, ssl) => true;
		db = new SqliteDatabase ();
		string appPath = Directory.GetParent(Application.persistentDataPath).Parent.ToString();
		String db_file = String.Format ("{0}{1}gg.sqlite", appPath, Path.DirectorySeparatorChar);
		db.Open (db_file);
		db.ExecuteNonQuery ("CREATE Table IF NOT EXISTS Account" +
		                    "(id INTEGER PRIMARY KEY, email TEXT, token Text)");
	}

	public void addFileToTemp(string path, Texture2D texture){
		string org_temp = TempPath + Path.DirectorySeparatorChar + ".original" + Path.DirectorySeparatorChar + path;
		saveAsPNG(texture, org_temp);			
		string png_path = Path.ChangeExtension (org_temp, "png");
		string png_stage_path = getStageFilePath (path);
		string folder = Path.GetDirectoryName (png_stage_path);				
		if(!Directory.Exists(folder)){
			Directory.CreateDirectory(folder);
		}
		if(File.Exists(png_stage_path) == false){
			File.Copy(png_path, png_stage_path);
		}
	}
	
	
	public static string getStageFilePath(string path){
		path = "ThemeAssets/stage/"+ path;
		return Path.ChangeExtension(path, "png");
	}

	public static string getExportFilePath(string path){
		path = "ThemeAssets/export/"+ path;
		return Path.ChangeExtension(path, "png");
	}

	
	public static string getTempOriginalFilePath(string path){
		path = TempPath + Path.DirectorySeparatorChar + ".original" + Path.DirectorySeparatorChar + path;
		return Path.ChangeExtension(path, "png");
	}


	public static string saveAsPNG(Texture2D t, string savePath){		
		savePath = Path.ChangeExtension (savePath, "png");
		//Create folder
		string folder = Path.GetDirectoryName (savePath);
		bool isExists = Directory.Exists(folder);				
		if(!isExists){
			Directory.CreateDirectory(folder);
		}

		//Writing PNG
		string texturePath = AssetDatabase.GetAssetPath(t);
		TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(texturePath);
		ti.isReadable = true;
		AssetDatabase.ImportAsset(texturePath);
		AssetDatabase.SaveAssets ();

		var tex = new Texture2D(t.width,t.height);
		tex.SetPixels32(t.GetPixels32());
		tex.Apply(false);
		File.WriteAllBytes(savePath, tex.EncodeToPNG());
		ti.isReadable = false;
		AssetDatabase.SaveAssets ();

		return savePath;
	}



	public string HttpUploadFile(string file, string paramName, NameValueCollection nvc, string gameProfile) {
		string assetURL = "https://api.greedygame.com/v1/units/" + gameProfile + "?format=json";
		string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
		byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
		
		HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(assetURL);
		wr.ContentType = "multipart/form-data; boundary=" + boundary;
		wr.Method = "POST";
		wr.KeepAlive = true;
		wr.Headers.Add (String.Format ("Authorization: Token {0}", getUserToken ()));
		wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
		
		Stream rs = wr.GetRequestStream();
		
		string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
		foreach (string key in nvc.Keys) {
			rs.Write(boundarybytes, 0, boundarybytes.Length);
			string formitem = string.Format(formdataTemplate, key, nvc[key]);
			byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
			rs.Write(formitembytes, 0, formitembytes.Length);
		}
		rs.Write(boundarybytes, 0, boundarybytes.Length);
		
		string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\nAuthorization: Token {3}\r\n\r\n";
		string header = string.Format(headerTemplate, paramName, file, AssetContentType, getUserToken());
		byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
		rs.Write(headerbytes, 0, headerbytes.Length);
		
		FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
		byte[] buffer = new byte[4096];
		int bytesRead = 0;
		while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
			rs.Write(buffer, 0, bytesRead);
		}
		fileStream.Close();
		
		byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
		rs.Write(trailer, 0, trailer.Length);
		rs.Close();
		
		WebResponse wresp = null;
		try {
			wresp = wr.GetResponse();
			Stream stream2 = wresp.GetResponseStream();
			StreamReader reader2 = new StreamReader(stream2);
			string res = reader2.ReadToEnd();
			Debug.Log(string.Format("[File uploaded] {0}", file));
			return res;
		} catch(System.Net.WebException ex) {
			Debug.Log("Error uploading file " + ex);
			
			using (var stream = ex.Response.GetResponseStream())
				using (var reader = new StreamReader(stream))
			{
				Debug.Log(reader.ReadToEnd());
			}
			if(wresp != null) {
				wresp.Close();
				wresp = null;
			}
		} finally {
			wr = null;
		}
		return null;
	}

	public static string getFileMD5(string file){
		MD5 md5 = MD5.Create ();
		FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
		byte[] bytes = md5.ComputeHash (fileStream);
		StringBuilder result = new StringBuilder(bytes.Length*2);
		for (int i = 0; i < bytes.Length; i++) {
			result.Append (bytes [i].ToString ("x2"));
		}
		string md5File = result.ToString();
		return md5File;
	}
	
	public string checkMD5(string file, string gameProfile, string relativeFolder) {
		string md5 = getFileMD5 (file);
		string folder = Path.GetDirectoryName (relativeFolder);
		string name = Path.GetFileName(file);
		string post_data = "checksum="+md5+"&folder="+folder+"&name="+name;
		//Debug.Log (post_data);
		//47838008

		string uri = "https://api.greedygame.com/v1/units/"+gameProfile+"/check?format=json";
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create (uri);
		request.Method = "POST";
		request.Credentials = System.Net.CredentialCache.DefaultCredentials;
		request.Headers.Add (String.Format("Authorization: Token {0}", getUserToken()));
		byte[] postBytes = Encoding.ASCII.GetBytes (post_data);
		request.ContentType = "application/x-www-form-urlencoded";
		request.ContentLength = postBytes.Length;
		Stream requestStream = request.GetRequestStream ();
		requestStream.Write (postBytes, 0, postBytes.Length);
		requestStream.Close ();
		HttpWebResponse response = null;
		string responseText = null;
		try {
			response = (HttpWebResponse)request.GetResponse ();
			responseText = new StreamReader (response.GetResponseStream ()).ReadToEnd ();
			response.Close();
		} catch(System.Net.WebException ex) {
			Debug.LogWarning(ex.ToString());
			if(response!=null) response.Close();
		} finally {
			response = null;
		}
		return responseText;
	}


	public string getUserEmail(){
		string email = null;
		string query_userAll = String.Format ("SELECT * FROM Account");
		DataTable cachedTable = db.ExecuteQuery(query_userAll);
		if (cachedTable.Rows.Count > 0){
			email = (string)cachedTable.Rows[0]["email"];
		}
		return email;
	}

	public string getUserToken(){
		string token = null;
		string query_userAll = String.Format ("SELECT * FROM Account");
		DataTable cachedTable = db.ExecuteQuery(query_userAll);
		if (cachedTable.Rows.Count > 0){
			token = (string)cachedTable.Rows[0]["token"];
		}
		return token;
	}

	public void userLogOut() {
		string delete_query = String.Format("DELETE FROM Account");
		db.ExecuteNonQuery (delete_query);
	}

	public int userLogin(string userEmail,  string userPassword) {
		string post_data = String.Format("email={0}&password={1}", userEmail, userPassword);
		string uri = "https://api.greedygame.com/users/authenticate/auth";
		
		// create a request
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create (uri);
		request.KeepAlive = false;
		request.ProtocolVersion = HttpVersion.Version10;
		request.Method = "POST";
		
		// turn our request string into a byte stream
		byte[] postBytes = Encoding.ASCII.GetBytes (post_data);
		
		// this is important - make sure you specify type this way
		request.ContentType = "application/x-www-form-urlencoded";
		request.ContentLength = postBytes.Length;
		Stream requestStream = request.GetRequestStream ();
		
		// now send it
		requestStream.Write (postBytes, 0, postBytes.Length);
		requestStream.Close ();
		
		// grab te response and print it out to the console along with the status code
		string responseText = null;
		int status = 0;
		try{
			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			responseText = new StreamReader (response.GetResponseStream ()).ReadToEnd ();
			status = (int)response.StatusCode;
			if(status == 200){			
				var dict = Json.Deserialize(responseText) as Dictionary<string, object>;
				string delete_query = String.Format("DELETE FROM Account");
				db.ExecuteNonQuery (delete_query);
				string cached_q = String.Format ("INSERT INTO Account VALUES({0},'{1}','{2}')", dict["id"], dict["email"], dict["token"]);
				Debug.Log (cached_q);
				db.ExecuteNonQuery(cached_q);
			}
		}catch (WebException ex) {
			string error = new StreamReader (ex.Response.GetResponseStream ()).ReadToEnd ();
			status = (int)((HttpWebResponse)ex.Response).StatusCode;
			Debug.Log (error);
		}
		return status;		
	}
}