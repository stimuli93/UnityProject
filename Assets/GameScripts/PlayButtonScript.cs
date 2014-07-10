using UnityEngine;
using System.Collections;

public class PlayButtonScript : MonoBehaviour {
	public Texture2D playerNormal;
	public Texture2D playerHover;
	private DontDistroyScript[] undestroy;
	// Use this for initialization
	void Start () {
		undestroy = FindObjectsOfType<DontDistroyScript>();
		float X, Y;
		X = this.guiTexture.pixelInset.x;Y = this.guiTexture.pixelInset.y;
		guiTexture.pixelInset = new Rect (X, Y, Screen.width / 3, Screen.height / 6);
	}
	// Update is called once per frame
	void OnMouseOver()
	{guiTexture.texture = playerHover;
	}
	public void OnMouseExit()
	{guiTexture.texture = playerNormal;
	}
	void OnMouseDown() {
		Application.LoadLevel("Scene1");
		int ctr = undestroy.Length;
		foreach (DontDistroyScript go in undestroy) {
		if(ctr!=1)
				Destroy(go);
			ctr--;
		}
		OnMouseExit ();
	}
}
