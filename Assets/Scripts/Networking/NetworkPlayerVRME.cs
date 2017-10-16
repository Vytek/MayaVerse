using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NetworkPlayerVRME : MonoBehaviour {

	//The UID of Player
	public string UID = String.Empty; //NOT WORKING?
	[SerializeField]
	public bool DEBUG = false;
	[SerializeField]
	public bool LOCAL = true;

	Vector3 lastPosition = Vector3.zero;
	Vector3 nextPosition = Vector3.zero;
	Quaternion lastRotation = Quaternion.identity;
	Quaternion nextRotation = Quaternion.identity;
	Vector3 lastScale;

	// Use this for initialization
	void Start () 
	{
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
        //SendMessage NOT HERE!
        //NetworkManager.instance.SendMessage(NetworkManager.SendType.SENDTOOTHER, NetworkManager.PacketId.PLAYER_JOIN, 0, this.UID, true, lastPosition, lastRotation);
    }
	
	// Update is called once per frame
	void Update () 
	{
		if (LOCAL == true) {
			if ((Vector3.Distance (transform.localPosition, lastPosition) > 0.05) || (Quaternion.Angle (transform.localRotation, lastRotation) > 0.3)) {
				if (DEBUG) {
					Debug.Log ("LOCAL");
					Debug.Log ("LastPos :" + lastPosition.x.ToString () + ", " + lastPosition.y.ToString () + ", " + lastPosition.z.ToString ());
					Debug.Log ("LastRot :" + lastRotation.x.ToString () + ", " + lastRotation.y.ToString () + ", " + lastRotation.z.ToString () + ", " + lastRotation.w.ToString ());
				}
				NetworkManager.instance.SendMessage(NetworkManager.SendType.SENDTOOTHER, NetworkManager.PacketId.PLAYER_MOVE, 0, this.UID, true, transform.localPosition, transform.localRotation);
				lastPosition = transform.localPosition;
				lastRotation = transform.localRotation;
			}
		} 
		else 
		{
			if ((Vector3.Distance(transform.position, lastPosition) > 0.05) || (Quaternion.Angle(transform.rotation, lastRotation) > 0.3))
			{
				if (DEBUG) {	
					Debug.Log ("GLOBAL");
					Debug.Log ("LastPos :" + lastPosition.x.ToString () + ", " + lastPosition.y.ToString () + ", " + lastPosition.z.ToString ());
					Debug.Log ("LastRot :" + lastRotation.x.ToString () + ", " + lastRotation.y.ToString () + ", " + lastRotation.z.ToString () + ", " + lastRotation.w.ToString ());
				}
				NetworkManager.instance.SendMessage(NetworkManager.SendType.SENDTOOTHER, NetworkManager.PacketId.PLAYER_MOVE, 0, this.UID, true, transform.position, transform.rotation);
				//Update stuff
				lastPosition = transform.position;
				lastRotation = transform.rotation;
			}
		}
	}
}