using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTracker : MonoBehaviour {

	public static List<GameObject> AllGameObjects { get; private set;}

	private void OnEnable() {

		if (AllGameObjects == null) {
			AllGameObjects = new List<GameObject> ();
		}
		AllGameObjects.Add (this.gameObject);
	}

	private void OnDisable() {
	
		AllGameObjects.Remove (this.gameObject);

	}

	public static GameObject GetObjectsWithIndex( int Index )
	{
		return AllGameObjects [Index];
	}

	//EOF
}
