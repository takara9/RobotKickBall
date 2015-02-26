using UnityEngine;
using System.Collections;

/**
 * 
 * 
 * */

public class HitGoal : MonoBehaviour
{

	/*
	 * これはボールとスクリプトを共有しているために、入っている。
	 * 
	 */
	private AudioSource sound01;	
	private GUIStyle GuiFont;
	private float  StartTime;
	private float  GameTime;
	private GUIStyle GuiGoalFont;
	private float  MsgTimeGoal;
	private float  MsgTimeGoalStart;
	private ParticleSystem ps;

	public float GoalMsgPriod = 3;
	public string RestServerUrl = "http://192.168.1.3/rest";

	void Start()
	{
		sound01 = GetComponent<AudioSource>();

		GuiGoalFont = new GUIStyle ();
		GuiGoalFont.fontSize = 100;
		GuiGoalFont.normal.textColor = Color.yellow;

		StartTime = Time.time;
		MsgTimeGoal = 0f;
		MsgTimeGoalStart = 0f;
	}

	void Update()
	{
		GameTime = Time.time - StartTime;
		if (MsgTimeGoal > 0f) {
			MsgTimeGoal = GoalMsgPriod - (Time.time - MsgTimeGoalStart);
			if (MsgTimeGoal < 0f) {
				MsgTimeGoal = 0f;
			}
		}
	}
	
	void OnGUI ()
	{
		if (MsgTimeGoal > 0f) {
			DispGoal();
		}
	}

	void DispGoal()
	{
		GUI.Label (new Rect (Screen.width/2 - 150 , Screen.height/2 - 40, 200, 40), "GOAL!", GuiGoalFont);
	}

	[RPC]
	void GoalHitAction()
	{
		MsgTimeGoalStart = Time.time;
		MsgTimeGoal = GoalMsgPriod;
		sound01.PlayOneShot (sound01.clip);
		ps = GetComponent<ParticleSystem> ();
		ps.Emit (20);
		BallGoal ("Sphere(Clone)");
	}

	void BallGoal (string name) 
	{
		GameObject obj = GameObject.Find (name);
		obj.rigidbody.velocity = new Vector3 (0, 0, 0);
		obj.transform.localPosition = new Vector3 (0, 10, 0);
		MsgTimeGoalStart = Time.time;
		MsgTimeGoal = GoalMsgPriod;
	}


	void OnCollisionEnter(Collision col)
	{
		string query;
		if (Network.isServer) {
			if (name == "GOAL_GREEN" || name == "GOAL_RED") {
				if (MsgTimeGoal == 0) {
					networkView.RPC ("GoalHitAction", RPCMode.All);
					DataManager.Instance.score = DataManager.Instance.score + 10;
					query = RestServerUrl + "?user=" + DataManager.Instance.username + "&key=" + DataManager.Instance.hashpw + "&=score=" + DataManager.Instance.score;
					GET (query);
				}
			}
		}

	}

	/*
	 * こちらも同じく、ボールが接触いｓた時のもの
	 */
	
	public WWW GET(string url) {
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www));
		return www;
	}
	
	private IEnumerator WaitForRequest(WWW www) {
		yield return www;
		// check for errors
		if (www.error == null) {
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
}