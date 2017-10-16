using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerParent : MonoBehaviour {

    //The UID of Player
    public string UID;
    public string AvatarName;
    public bool DEBUG = true;

    // Use this for initialization
    void Start () {
        NetworkManager.OnDisconnectedClientUpdate += NetworkManager_OnDisconnectedClientUpdate;
    }

    void NetworkManager_OnDisconnectedClientUpdate(NetworkManager.DisconnectedClientToDestroyPlayerGameObject newMessage)
    {
        if (newMessage.PlayerGameObjectUID == this.UID)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(DestroyGameObjectOnTheMainThread());
            Debug.Log("Raise event in Player GameObject");
        }
    }

    /// <summary>
    /// Destry GameObject In MainThread
    /// </summary>
    /// <returns></returns>
	public IEnumerator DestroyGameObjectOnTheMainThread()
    {
        DestroyImmediate(gameObject);
        Debug.Log("Destroy Player GameObject");
        yield return null;
    }
}
