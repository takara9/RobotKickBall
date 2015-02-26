using UnityEngine;
using System.Collections;

/**
 * 
 * ネットワークのマスターサーバーに繋がった時に生まれるプレハブ
 * 
 * 
 * */

public class SpawnPrefab : MonoBehaviour
{
	public Transform playerPrefab;

	/*
	 * ネットワークに接続した時に、この関数がコールバックされる
	 * クライアントであれば、ネットワーク・インスタンスを起動する
	 */
	void OnNetworkLoadedLevel ()
	{
		if (!Network.isServer) {
			Network.Instantiate (playerPrefab, transform.position, transform.rotation, 0);
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