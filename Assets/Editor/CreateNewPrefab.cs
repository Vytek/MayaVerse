// Creates a prefab at the given path.
// If a prefab already exists it asks if you want to replace it

using UnityEngine;
using UnityEditor;

public class CreateNewPrefab : EditorWindow
{
	[MenuItem("Prefab/Create New Prefab")]
	static void CreatePrefab()
	{
		GameObject[] objs = Selection.gameObjects;

		foreach (GameObject go in objs)
		{
			string localPath = "Assets/" + go.name + ".prefab";
			if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
			{
				if (EditorUtility.DisplayDialog("Are you sure?",
					"The prefab already exists. Do you want to overwrite it?",
					"Yes",
					"No"))
				{
					CreateNew(go, localPath);
				}
			}
			else
			{
				Debug.Log(go.name + " Prefab Created");
				CreateNew(go, localPath);
			}
		}
	}

	// Disable the menu item if no selection is in place
	[MenuItem("Prefab/Create New Prefab", true)]
	static bool ValidateCreatePrefab()
	{
		return Selection.activeGameObject != null;
	}

	static void CreateNew(GameObject obj, string localPath)
	{
		Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
		PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
	}
}