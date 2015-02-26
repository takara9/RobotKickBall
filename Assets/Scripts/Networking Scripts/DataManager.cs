using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private static DataManager instance;

	public int hoge;
	public string username;
	public string hashpw;
	public int score;
	public string owner;

	public static DataManager Instance{
		get{
			if( null == instance ){
				instance = (DataManager)FindObjectOfType(typeof(DataManager));
				if( null == instance ){
					Debug.Log(" DataManager Instance Error ");
				}
			}
			return instance;
		}
	}
	
	void Awake(){
		GameObject[] obj = GameObject.FindGameObjectsWithTag("DataManager");
		if( 1 < obj.Length ){
			// 既に存在しているなら削除
			Destroy( gameObject );
		}else{
			// シーン遷移では破棄させない
			DontDestroyOnLoad( gameObject );
		}
	}


}
