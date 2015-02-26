using UnityEngine;
using System.Collections;

public class DispMsg : MonoBehaviour {


	private GUIStyle GuiFont;
	private float  StartTime;
	private float  GameTime;
	private GUIStyle GuiGoalFont;

	// Use this for initialization
	void Start()
	{
		GuiFont = new GUIStyle ();
		GuiFont.fontSize = 32;
		GuiFont.normal.textColor = Color.green;

		GuiGoalFont = new GUIStyle ();
		GuiGoalFont.fontSize = 100;
		GuiGoalFont.normal.textColor = Color.yellow;

		StartTime = Time.time;
	}

	// Update is called once per frame
	void Update()
	{
		GameTime = Time.time - StartTime;
	}
	
	void OnGUI ()
	{
//		DispStartMessage ();
	}

	void DispStartMessage()
	{
		GUI.Label (new Rect (0, Screen.height - 40, 200, 40), "Arena Time  = " + GameTime, GuiFont);
	}
}
