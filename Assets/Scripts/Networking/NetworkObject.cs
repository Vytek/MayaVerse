using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class NetworkObject : MonoBehaviour {

	//The ID this object (so we can check if it's us updating)
	public ushort objectID;
	public bool DEBUG = true;
	public float InterRate = 9;

	Vector3 lastPosition = Vector3.zero;
	Vector3 nextPosition = Vector3.zero;
	Quaternion lastRotation = Quaternion.identity;
	Quaternion nextRotation = Quaternion.identity;
	Vector3 lastScale;

	bool isKinematic = false;
	Rigidbody rb;
	VRTK_InteractableObject io;

	void Awake()
	{
		io = GetComponent<VRTK_InteractableObject> ();
		rb = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {
		NetworkManager.OnReceiveMessageFromGameObjectUpdate += NetworkManager_OnReceiveMessageFromGameObjectUpdate;

		if (GetComponent<VRTK_InteractableObject> () == null) {
			Debug.LogError ("This component requires the VRTK_InteractableObject script attached to the parent.");
			return;
		} else {
			GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
			GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(ObjectUngrabbed);
		}

		//Initialize
		lastPosition = transform.position;
		lastRotation = transform.rotation;
	}

	/// <summary>
	/// Thises the will be executed on the main thread.
	/// </summary>
	/// <returns>The will be executed on the main thread.</returns>
	public IEnumerator ThisWillBeExecutedOnTheMainThread(bool isKine)
	{
		Debug.Log("Update pos and ros execute in MainThread");
		//transform.position = new Vector3(newMessage.GameObjectPos.x, newMessage.GameObjectPos.y, newMessage.GameObjectPos.z);
		rb.isKinematic = isKine;
		this.isKinematic = isKine;
		lastPosition = nextPosition;
		lastRotation = nextRotation;
		transform.position = nextPosition;
		transform.rotation = nextRotation;
		yield return null;
	}

	/// <summary>
	/// Networks the manager on receive message from game object update.
	/// </summary>
	/// <param name="newMessage">New message.</param>
	void NetworkManager_OnReceiveMessageFromGameObjectUpdate (NetworkManager.ReceiveMessageFromGameObject newMessage)
	{
		if (DEBUG) {
			Debug.Log ("Raise event in GameObject");
			Debug.Log (newMessage.MessageType);
			Debug.Log (newMessage.GameObjectID);
			Debug.Log (newMessage.GameObjectPos);
			Debug.Log (newMessage.GameObjectRot);
			Debug.Log (newMessage.isKinematic);
		}

		//Update pos and rot
		if (newMessage.GameObjectID == objectID)
		{
			nextPosition = new Vector3(newMessage.GameObjectPos.x, newMessage.GameObjectPos.y, newMessage.GameObjectPos.z);
			nextRotation = new Quaternion(newMessage.GameObjectRot.x, newMessage.GameObjectRot.y, newMessage.GameObjectRot.z, newMessage.GameObjectRot.w);
			UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(newMessage.isKinematic));
		}
	}

	/// <summary>
	/// Objects the grabbed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
	{
		if (DEBUG) {
			Debug.Log ("I'm grabbed!");
		}
		this.isKinematic = true;
	}

	/// <summary>
	/// Objects the ungrabbed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void ObjectUngrabbed(object sender, InteractableObjectEventArgs e)
	{
		if (DEBUG) {
			Debug.Log ("I'm ungrabbed!");
		}
		this.isKinematic = false;
	}

	// Update is called once per frame
	void Update () {
		if ((Vector3.Distance(transform.position, lastPosition) > 0.05) || (Quaternion.Angle(transform.rotation, lastRotation) > 0.3))
		{
			//If is a client object not grabbed as isKinematic, the owner object grabbed and isKinematic = false
			if (!rb.isKinematic) 
			{	
				//SerialisePosRot (Vector3.Lerp (transform.position, lastPosition, Time.deltaTime * InterRate), Quaternion.Lerp (transform.rotation, lastRotation, Time.deltaTime * InterRate), ObjectID, isKinematic);
				NetworkManager.instance.SendMessage (NetworkManager.SendType.SENDTOOTHER, NetworkManager.PacketId.OBJECT_MOVE, this.objectID, String.Empty, this.isKinematic, transform.position, transform.rotation);
				//Update stuff
				lastPosition = transform.position;
				lastRotation = transform.rotation;
				//this.isKinematic = rb.isKinematic; NO!
			}
		}
	}
}
