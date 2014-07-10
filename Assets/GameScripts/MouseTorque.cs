using UnityEngine;
using System.Collections;


public class MouseTorque : MonoBehaviour {
	
	public Font MyFont;
	GUIStyle LargeFont;
	public Vector3 acc;
	public int number;
	public int upside=6;
	void Start()
	{   
		LargeFont = new GUIStyle ();
		LargeFont.fontSize = 24;
	}
	int CalcSideUp() {
		float [] pos = new float[6];
		float dotFwd = Vector3.Dot (this.transform.forward, Vector3.up);
		if (dotFwd > 0.99f)
			return 1;
		else if (dotFwd < -0.99f)
			return 6;
		else {
			pos [0] = 0.99f - dotFwd;
			pos [1] = dotFwd + 0.99f;
		}
		float dotRight = Vector3.Dot (this.transform.right, Vector3.up);
		if (dotRight > 0.99f)
			return 4;
		else if (dotRight < -0.99f)
			return 3;
		else {
			pos [2] = 0.99f - dotRight;
			pos [3] = dotRight + 0.99f;
		}
		float dotUp = Vector3.Dot (this.transform.up, Vector3.up);
		if (dotUp > 0.99f)
			return 5;
		else if (dotUp < -0.99f)
			return 2;
		else {
			pos [4] = 0.99f - dotUp;
			pos [5] = dotUp + 0.99f;		
		}
		
		float min = 21;
		int i, ipos = 0;
		for (i=0; i<6; i++) {
			if (pos [i] < min) {
				min = pos [i];
				ipos = i;
			}
		}
		i = ipos;
		
		if (i == 0) {
			return 1;
		}
		if (i == 1) {
			return 6;
		}
		if (i == 2) {
			return 4;
		}
		if (i == 3) {
			return 3;
		}
		if (i == 4)
		{
			return 5;
		}
		return 2;
	
	}
	float abs(float a)
	{
	if (a < 0)
						return -a;
		return a;
	}
	public float speed = 40.0F;
	void Update() {
		upside = CalcSideUp ();
				#if UNITY_ANDROID
				rigidbody.AddTorque (acc*2);
				rigidbody.AddForce (acc);
				#endif

	}
	void OnGUI() {
		LargeFont.normal.textColor = Color.yellow;
		LargeFont.font = MyFont;
		//GUI.Label (new Rect (Screen.width * 0.8f, Screen.height * number*0.1f,100,20), "Dice "+number.ToString()+" :" + upside.ToString (),LargeFont);
		
	}
}