using UnityEngine;
using System.Collections;

public class SphereNetworkInit : MonoBehaviour {

	void OnNetworkInstantiate (NetworkMessageInfo msg)
	{
		// This is our own player
		if (networkView.isMine) {
			GetComponent<NetworkRigidbody>().enabled = false;
		} else {
			name += "Remote";
			GetComponent<NetworkRigidbody>().enabled = true;
		}
	}
}