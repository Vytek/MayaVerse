using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour {

	//The UID of Player
	public string UID;
	public bool DEBUG = true;
	bool LOCAL = true;

	Vector3 lastPosition = Vector3.zero;
	Vector3 nextPosition = Vector3.zero;
	Quaternion lastRotation = Quaternion.identity;
	Quaternion nextRotation = Quaternion.identity;
	Vector3 lastScale;

	// Use this for initialization
	void Start () {
		NetworkManager.OnReceiveMessageFromGameObjectUpdate += NetworkManager_OnReceiveMessageFromGameObjectUpdate;
		NetworkManager.OnDisconnectedClientUpdate += NetworkManager_OnDisconnectedClientUpdate;
		//Initialize
		if (LOCAL) 
		{
			lastPosition = transform.localPosition;
			lastRotation = transform.localRotation;
		} 
		else 
		{
			lastPosition = transform.position;
			lastRotation = transform.rotation;
		}
	}

	void NetworkManager_OnDisconnectedClientUpdate (NetworkManager.DisconnectedClientToDestroyPlayerGameObject newMessage)
	{
		if (newMessage.PlayerGameObjectUID == this.UID)
		{
			UnityMainThreadDispatcher.Instance().Enqueue(DestroyGameObjectOnTheMainThread());
			Debug.Log ("Raise event in Player GameObject");
		}
	}

    /// <summary>
    /// Execute Update In Main Thread
    /// </summary>
    /// <returns></returns>
	public IEnumerator ThisWillBeExecutedOnTheMainThread()
	{
		//Debug.Log("This is executed from the main thread"); //DEBUG
		//transform.position = new Vector3(newMessage.GameObjectPos.x, newMessage.GameObjectPos.y, newMessage.GameObjectPos.z);
		lastPosition = nextPosition;
		lastRotation = nextRotation;
		if (LOCAL) 
		{
			transform.localPosition = nextPosition;
			transform.localRotation = nextRotation;
		}
		else
		{
			transform.position = nextPosition;
			transform.rotation = nextRotation;
		}
		yield return null;
	}

    /// <summary>
    /// Destry GameObject In MainThread
    /// </summary>
    /// <returns></returns>
	public IEnumerator DestroyGameObjectOnTheMainThread()
	{
		DestroyImmediate (gameObject);
		Debug.Log ("Destroy Player GameObject");
		yield return null;
	}

	void NetworkManager_OnReceiveMessageFromGameObjectUpdate (NetworkManager.ReceiveMessageFromGameObject newMessage)
	{
		if (DEBUG) 
		{
			Debug.Log ("Raise event in Player GameObject");
			Debug.Log (newMessage.MessageType);
			Debug.Log (newMessage.GamePlayerObjectOwner);
			Debug.Log (newMessage.GameObjectPos);
			Debug.Log (newMessage.GameObjectRot);
		}
		//Update pos and rot
		if (newMessage.GamePlayerObjectOwner == UID)
		{
			nextPosition = new Vector3(newMessage.GameObjectPos.x, newMessage.GameObjectPos.y, newMessage.GameObjectPos.z);
			nextRotation = new Quaternion(newMessage.GameObjectRot.x, newMessage.GameObjectRot.y, newMessage.GameObjectRot.z, newMessage.GameObjectRot.w);
			UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread());
		}
	}
}
