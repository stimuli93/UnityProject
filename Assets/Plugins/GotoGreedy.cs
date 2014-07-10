using UnityEngine;
using System.Collections;
using GreedyAds;

public class GotoGreedy : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		Debug.Log ("ThemeManager.Instance.isDone " + ThemeManager.Instance.isDone);
		if (ThemeManager.Instance.isDone == false) {
			Application.LoadLevel("ggLoader");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
