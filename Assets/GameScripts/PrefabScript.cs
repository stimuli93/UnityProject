using UnityEngine;
using System.Collections;

public class PrefabScript : MonoBehaviour {

	// Use this for initialization
	public Font MyFont;
	public Transform dice;
	GUIStyle LargeFont;
	GUIStyle TitleFont;
	public int diceCount=0;
	public Vector3 accelerate;
	private MouseTorque[] diceobj;
	private int sum;
	private string displaysum;
	private DontDistroyScript[] undestroy;
	void Awake()
	{  LargeFont = new GUIStyle ();
		LargeFont.fontSize = 26;
		TitleFont = new GUIStyle ();
		TitleFont.fontSize = 36;
		sum = diceCount;
		undestroy = FindObjectsOfType<DontDistroyScript>();
		diceCount = undestroy[0].passingdice;
		Debug.Log (undestroy.Length);
	}
	void Start() {	
		float[] arx = new float[10];
		float[] arz = new float[10];
						for (int i=0; i<10; i++) {
								arx [i] = 0;
								arz [i] = 0;		
						}
						arx [0] = -7.5f;
						arx [1] = -3.0f;
						arx [2] = 3.0f;
						arx [3] = 7.5f;
						arz [4] = -7.5f;
						arz [5] = -3.0f;
						arz [6] = 3.0f;
						arz [7] = 7.5f;
						for (int x =0; x < diceCount; x++) {
								Instantiate (dice, new Vector3 (arx [x], -4.52f, arz [x]), Quaternion.identity);
		}
		diceobj = FindObjectsOfType<MouseTorque>();
		int ctr = 1;
		foreach (MouseTorque dcob in diceobj)
		{
			dcob.number=ctr;ctr++;
			Debug.Log(dcob.name);
		}         
				}
		
	
	// Update is called once per frame
	void Update () {
		sum = 0;
		accelerate.x = Input.acceleration.x*120;
		accelerate.y = 0;accelerate.z = Input.acceleration.y*120;
		foreach (MouseTorque dcob in diceobj)
		{
			dcob.acc=accelerate;
			sum+=dcob.upside;
		}
		if (Input.GetKeyDown(KeyCode.Escape)) { 
			Application.LoadLevel("StartScene"); }
	}

	void OnGUI() {
		LargeFont.normal.textColor = Color.white;
		LargeFont.font = MyFont;
		TitleFont.normal.textColor = Color.white;
		TitleFont.font = MyFont;
		GUI.Label (new Rect (Screen.width * 0.775f, Screen.height *0.06f, 100, 20),"Dice Roll",TitleFont);
		int n = 0;
		foreach (MouseTorque dcob in diceobj)
		{   if(n==0)
			GUI.Label (new Rect (Screen.width * 0.82f, Screen.height *0.25f, 100, 20),dcob.upside.ToString(),LargeFont);
			else
				GUI.Label (new Rect (Screen.width * 0.8f, Screen.height *(0.25f+n*0.08f), 100, 20),"+"+dcob.upside.ToString(),LargeFont);
			n++;
		}
		GUI.Label (new Rect (Screen.width * 0.8f, Screen.height *(0.25f+n*0.08f), 100, 20),":"+sum.ToString(),LargeFont);
	} 
}
