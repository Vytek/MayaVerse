using UnityEngine;
using System;
using System.IO;

public class SerializableManager : MonoBehaviour {
    public static SerializableManager current;
    public const string DefaultSaveName = "Level1.game";

    public String DefaultFileName = "Level1.game";

    public bool AutoSave, TryToLoadDefaultOnStart, OnExitSave, Encrypt;

    [HideInInspector]
    public float TimeBetweenSavedSeconds = 60 * 5, CurrentTimeToSave;
    [HideInInspector]
    public String password = "my password";

    private bool Saved, Loaded;

    SerializableManager() {
        if (!IsCurrentValid()) return;
        current = this;
    }

    void Awake() {
        if (!IsCurrentValid()) return;
        current = this;

        if (TryToLoadDefaultOnStart) {
            LoadAll();
        }
    }

    void Update() {
        if (AutoSave) {
            CurrentTimeToSave += Time.deltaTime;
            if(CurrentTimeToSave > TimeBetweenSavedSeconds) {
                CurrentTimeToSave = 0;
                SaveAll();
            }
        }
    }

    void OnApplicationQuit() {
        if(OnExitSave) SaveAll();
    }

    bool IsCurrentValid() {
        if (current != null && current != this) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// To save the game, with the default file name
    /// </summary>
    public static void SaveAll() {
        SaveAll(current.DefaultFileName);
    }

    /// <summary>
    /// To save the game, with the specified file name
    /// </summary>
    /// <param name="FileName">The file name without path</param>
    public static void SaveAll(string FileName) {
        SerializableGame.SaveAll(FileName);
        current.Saved = true;
    }

    /// <summary>
    /// To load the game, with the default file name
    /// </summary>
    public static void LoadAll() {
        LoadAll(current.DefaultFileName);
    }

    /// <summary>
    /// To load the game, with the specified file name
    /// </summary>
    /// <param name="FileName">The file name without path</param>
    public static void LoadAll(string FileName) {
        SerializableGame.Load(FileName);
        current.Loaded = true;
    }

    /// <summary>
    /// To see if the level is loaded
    /// </summary>
    /// <returns>If the level is loaded</returns>
    public static bool IsLoaded() {
        return current.Loaded;
    }

    /// <summary>
    /// To see if the level is saved
    /// </summary>
    /// <returns>If the level is saved</returns>
    public static bool IsSaved() {
        return current.Saved;
    }

    /// <summary>
    /// To see if the file already exists
    /// </summary>
    /// <param name="filename">The file name without path</param>
    /// <returns>Status if the file exists</returns>
    public static bool FileExists(String filename) {
        return File.Exists(Application.persistentDataPath + "/" + filename);
    }

    /// <summary>
    /// To instantiate a prefab in the right way
    /// </summary>
    /// <param name="prefab">The name of you prefab you want to instantiate</param>
    /// <returns>The new object that was created</returns>
    public static GameObject PrefabInstantiate(String prefabName) {
		GameObject toLoad = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
		if(toLoad == null){
			Debug.LogError("The prefab name is no valid or is not in a resource folder.");
			return null;
		}
		GameObject creating = Instantiate(toLoad) as GameObject;
		SerializableGame.GameObjectPrefabName.Add(creating, prefabName);
		return creating;
    }

	/// <summary>
	/// To instantiate a prefab in the right way
	/// </summary>
	/// <param name="prefab">The GameObject you want to instantiate</param>
	/// <returns>The new object that was created</returns>
	public static GameObject PrefabInstantiate(GameObject prefab) {
		return PrefabInstantiate(prefab.name);
	}
}