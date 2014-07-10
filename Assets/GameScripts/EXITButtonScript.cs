using UnityEngine;
using System.Collections;

public class EXITButtonScript : MonoBehaviour {

	public Texture2D playerNormal;
	public Texture2D playerHover;
	// Use this for initialization
	void Start () {
		float X, Y;
		X = this.guiTexture.pixelInset.x;Y = this.guiTexture.pixelInset.y;
		guiTexture.pixelInset = new Rect (X, Y, Screen.width / 3, Screen.height /6);
	}
	
	// Update is called once per frame
	void OnMouseOver()
	{guiTexture.texture = playerHover;
	}
	void OnMouseExit()
	{guiTexture.texture = playerNormal;
	}
	
	// Update is called once per frame
	void OnMouseDown() {
		Application.Quit ();
	}

}
