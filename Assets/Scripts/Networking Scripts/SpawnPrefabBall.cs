using UnityEngine;
using System.Collections;

/**
 * 
 * ネットワークのマスターサーバーに繋がった時に生まれるプレハブ
 * 
 * 
 * 
 * */

public class SpawnPrefabBall : MonoBehaviour
{

	public Transform playerPrefab;

	/*
	 * ネットワークに接続した時に、この関数がコールバックされる
	 * クライアントであれば、ネットワーク・インスタンスを起動する
	 */
	void OnNetworkLoadedLevel ()
	{
		if (Network.isServer) {
			Debug.Log ("OnNetworkLoadLevel is Server");
			Network.Instantiate (playerPrefab, transform.position, transform.rotation, 0);
		} else {
			Debug.Log ("OnNetworkLoadLevel is Client");
			Debug.Log ("name = " + name);	
		}

	}
	
	/*
	 * プレイヤーが切れたらプレイヤーを削除する
	 * 
	 */
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		Debug.Log ("Server destroying player");
		Network.RemoveRPCs (player, 0);
		Network.DestroyPlayerObjects (player);
	}

}
