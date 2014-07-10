using UnityEngine;
using System.Collections;

public class DiceSlider : MonoBehaviour {
	// Use this for initialization
	public DontDistroyScript script;
	public GUISkin mySkin;
	void Start () {
		Debug.Log (Screen.width);
		mySkin.horizontalSlider.fixedHeight = Screen.height / 6;
		mySkin.horizontalSliderThumb.fixedHeight = Screen.height / 6+1;
		mySkin.horizontalSliderThumb.fixedWidth = Screen.width / 12;
	}
	int dices = 1;
	
	void OnGUI(){
		GUI.skin = mySkin;
				GUI.Label (new Rect (Screen.width * 0.35f, Screen.height / 2 - 15, 150, 45), "Dice Count");
				dices = (int)(GUI.HorizontalSlider (new Rect (Screen.width * 0.35f + 5, Screen.height / 2 + 10, Screen.width / 3 + 5, 50), dices, 1, 5));
				GUI.Label (new Rect (Screen.width * 0.35f + Screen.width / 3 + 10, Screen.height*0.55f, 80, 45), dices.ToString ());
		}
	// Update is called once per frame
	void Update () {
		script.passingdice = dices;
	}
}
