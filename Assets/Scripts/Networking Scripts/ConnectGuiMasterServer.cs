using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;

public class ConnectGuiMasterServer : MonoBehaviour
{

	private int state = 0;

	public string gameTypeName = "ゲームタイプ";
	public string gameName = "ゲーム名";
	public string comment = "コメント";
	private int serverPort = 25002;

	public string ipAddress = "108.168.231.170";
	public int port = 23466;
	private float lastHostListRequest = -1000.0f;
	private float hostListRefreshTimeout = 10.0f;
	private bool filterNATHosts = false;
	private bool useNat = false;

	
	private Rect windowRect;
	private Rect serverListRect;


	public String username = "user_x";
	public String password = "password";
	public String hashpw;

	public Transform playerPrefab;

	private GUIStyle smallFont;
	private GUIStyle largeFont;
	private float StartTime;


	void OnFailedToConnectToMasterServer (NetworkConnectionError info)
	{
		Debug.Log ("OnFailedToConnectToMasterServer : "+info);
	}

	void OnFailedToConnect (NetworkConnectionError info)
	{
		Debug.Log ("OnFailedToConnect : "+info);
	}

	void OnGUI ()
	{
		if (state == 0 || state == 1) 
		{
			windowRect = new Rect (Screen.width - 220, 0, 200, 50);
			windowRect = GUILayout.Window (0, windowRect, MakeWindow, "ゲームアリーナ");
			serverListRect = new Rect (0, 70, Screen.width, 100);
			if (Network.peerType == NetworkPeerType.Disconnected && MasterServer.PollHostList ().Length != 0)
				serverListRect = GUILayout.Window (1, serverListRect, MakeClientWindow, "ゲームサーバーリスト");
		}
		if (state == 1) 
		{

			username = GUILayout.TextField (username);
			password = GUILayout.PasswordField (password, "*" [0], 10);
			if (GUILayout.Button ("Enter")) {
				Debug.Log ("username = " + username);
				Debug.Log ("password = " + password);
				string up = username + password;
				SHA1 sha = new SHA1CryptoServiceProvider ();
				UTF8Encoding ue = new UTF8Encoding ();
				byte[] planeBytes = ue.GetBytes (up);
				byte[] hashBytes = sha.ComputeHash (planeBytes);
				string hashStr = "";
				foreach (byte b in hashBytes) {
					hashStr += string.Format ("{0,0:x2}", b);
				}
				hashpw = hashStr;
				Debug.Log ("Hash data = " + hashpw);
				DataManager.Instance.username = username;
				DataManager.Instance.hashpw = hashStr;
				state = 0;
			}
		}
	}

	void Awake ()
	{
		MasterServer.ipAddress = ipAddress;
		MasterServer.port = port;
		MasterServer.dedicatedServer = true;  // サーバーを立てたプレイヤーを人数に加えたくない場合trueにしてください
		DontDestroyOnLoad (this);

		//所有しているIPアドレスを確認します
		if (Network.HavePublicAddress ())
			Debug.Log ("パブリックIPアドレスを所有しています");
		else
			Debug.Log ("プライベートIPアドレスを所有しています");

		DataManager.Instance.hoge = 0;

	}
	void MakeWindow (int id)
	{
		GUILayout.Space (10);
		if (Network.peerType == NetworkPeerType.Disconnected) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space (10);
			
			if (GUILayout.Button ("サーバー起動")) {
				Network.InitializeServer (32, serverPort, useNat);
				MasterServer.RegisterHost (gameTypeName, gameName, comment);
			}
			
			if (GUILayout.Button ("リフレッシュ") || Time.realtimeSinceStartup > lastHostListRequest + hostListRefreshTimeout) {
				MasterServer.RequestHostList (gameTypeName);
				lastHostListRequest = Time.realtimeSinceStartup;
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			state = 0;
		} else {
			if (GUILayout.Button ("切断")) {
				Network.Disconnect ();
				MasterServer.UnregisterHost ();
			}
			GUILayout.FlexibleSpace ();
		}
		GUI.DragWindow (new Rect (0, 0, 1000, 1000));
	}

	void MakeClientWindow (int id)
	{
		GUILayout.Space (5);
		
		HostData[] data = MasterServer.PollHostList ();
		foreach (HostData element in data) {
			if (!(filterNATHosts && element.useNat)) {
				GUILayout.BeginHorizontal ();
				var connections = element.connectedPlayers + "/" + element.playerLimit;
				GUILayout.Label (element.gameName);
				GUILayout.Space (5);
				GUILayout.Label (connections);
				GUILayout.Space (5);
				var hostInfo = "";
				
				//全てのIPアドレスが表示されます。
				//マスターサーバーと内部LANで接続されている場合には複数IPアドレスが表示される場合があります。
				//Unityではこの全てのIPアドレスをチェックするようになっていて最初に有効だと判断したものに接続します。
				foreach (string host in element.ip)
					hostInfo = hostInfo + host + ":" + element.port + " ";
				

				GUILayout.Space (5);
				GUILayout.Label (element.comment);
				GUILayout.Space (5);
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("アリーナ接続")) {
					Network.Connect (element);
					state = 1;
				}
				GUILayout.EndHorizontal ();
				GUILayout.Label (hostInfo);
			}
		}
	}
}
