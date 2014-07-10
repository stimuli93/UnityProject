using UnityEngine;
using System.Collections;

public class BackButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float X, Y;
		X = this.guiTexture.pixelInset.x;Y = this.guiTexture.pixelInset.y;
		guiTexture.pixelInset = new Rect (X, Y, Screen.width / 6, Screen.height / 8);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown() {
		Debug.Log("Ouch");
		Application.LoadLevel("StartScene");
	}
}
