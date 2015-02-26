using UnityEngine;
using System.Collections;

[AddComponentMenu("Ball Player/Ball Controller")]
public class BallController : MonoBehaviour {

	private float RestartTimer;
	private float GoalTime;


	void Start () {
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}


	void Update () {
		if (transform.localPosition.y < -200f) {
			GameObject obj = GameObject.Find(name);
			obj.rigidbody.velocity = new Vector3(0,0,0);
			transform.localPosition = new Vector3(0f,10f,0f);
		}
	}


	[RPC]
	void BallAction( string name, float x, float z, float pushPower, string owner) 
	{
		Vector3 pushDir = new Vector3 (x, 0.14f, z);
		GameObject obj = GameObject.Find (name);
		obj.rigidbody.velocity = pushDir * pushPower;
	}

	//キャラクターコントローラーがRigidbodyのものを押すためのもの
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		float pushPower;
		Rigidbody body = hit.collider.attachedRigidbody;

		if (body == null || body.isKinematic)
			return;
		if (hit.moveDirection.y < -0.3f)
			return;

		//===================================================
		ThirdPersonController marioController = GetComponent<ThirdPersonController> ();
		float speed = marioController.GetSpeed ();
		pushPower = speed - 0.3f;
		if (marioController.IsKicking ()) {
			pushPower = 20.0f;
		}
		//===================================================

		string owner = DataManager.Instance.username;
		networkView.RPC ("BallAction", RPCMode.Server, hit.gameObject.name, hit.moveDirection.x, hit.moveDirection.z, pushPower, owner);
	}
}
