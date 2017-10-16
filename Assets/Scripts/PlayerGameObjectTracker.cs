using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameObjectTracker : MonoBehaviour {

    public static List<GameObject> AllPlayerGameObjects { get; private set; }

    private void OnEnable()
    {

        if (AllPlayerGameObjects == null)
        {
            AllPlayerGameObjects = new List<GameObject>();
        }
        AllPlayerGameObjects.Add(this.gameObject);
    }

    private void OnDisable()
    {

        AllPlayerGameObjects.Remove(this.gameObject);

    }

    public static GameObject GetObjectsWithIndex(int Index)
    {
        return AllPlayerGameObjects[Index];
    }
}
