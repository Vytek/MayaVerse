using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[ExecuteInEditMode]
public class SerializableComponent : MonoBehaviour {

    [HideInInspector]
    public List<MonoBehaviour> ValidToLinkComponents = new List<MonoBehaviour>();

    void Start() {
        this.DetectComponents();
        if (Application.isPlaying && ValidToLinkComponents.Count > 0) {
            SerializableGame.ToSaveGameObjects.Add(gameObject);
        }
    }

    void OnDestroy() {
        if (Application.isPlaying) {
            SerializableGame.ToSaveGameObjects.Remove(gameObject);
        }
    }

    /// <summary>
    /// Check if you have to add a new component to save system
    /// </summary>
    public void DetectComponents() {
        ValidToLinkComponents.Clear();

        foreach (MonoBehaviour ToCheck in gameObject.GetComponents<MonoBehaviour>()) {
            if (SerializableFiles.NeedLink(ToCheck)) {
                ValidToLinkComponents.Add(ToCheck);
            }
        }

    }
}