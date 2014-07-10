using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float X, Y;
		X = this.guiTexture.pixelInset.x;Y = this.guiTexture.pixelInset.y;
		guiTexture.pixelInset = new Rect (X, Y, Screen.width/2, Screen.height/5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
