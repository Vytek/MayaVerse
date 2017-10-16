using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

	public GameObject ThisGameObject;

	// Use this for initialization
	void Start () {
		//Count
		Debug.Log ("Mumbers GameObjects: " + GameObjectTracker.AllGameObjects.Count);
		//Index a GameObject in List
		Debug.Log ("GameObject Name: " + GameObjectTracker.AllGameObjects [1].gameObject.name);
		//Search with foreach
		foreach (GameObject GO in GameObjectTracker.AllGameObjects)
		{
			Debug.Log ("GameObject Name: " + GO.name);
		}
		//Search with IndexOf??
	}
}
