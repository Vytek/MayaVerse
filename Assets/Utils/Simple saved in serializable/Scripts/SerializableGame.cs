using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using SerializableObjects;
/// <summary>
/// Autor @thalacDev
///////////////////////////////////////////////////
/// MODIFY THIS CLASS CAN CAUSE FATAL ERRORS!  ///
//////////////////////////////////////////////////
/// </summary>
public static class SerializableGame {
	public static Dictionary<GameObject, String> GameObjectPrefabName = new Dictionary<GameObject, String>();
    public static List<GameObject> ToSaveGameObjects = new List<GameObject>();
    public static bool DebugSave = false;
    public const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
    //field value 1 = position
    //field value 2 = rotation
    //field value 3 = scale

    public static void SaveAll(string filename) {
        List<GameObjectValue> GameObjectsValues = new List<GameObjectValue>();

        //GameObject level
        foreach (GameObject go in ToSaveGameObjects) {

            //GameObject variables
            String Searchkey = "";
            //To avoid invalid use of keys
            string ReferentValueKey = "";
            String PrefabPath = "";
            List<MonoBehaviourValues> ScriptsList = new List<MonoBehaviourValues>();

            //script level
            foreach (MonoBehaviour mono in go.GetComponents<MonoBehaviour>()) {
                if (!SerializableFiles.NeedLink(mono)) continue;

                //script variables
                List<FieldValue> Field_Values = new List<FieldValue>();

                SaveAttribute LastsaveAttribute = null;

                //extras saves
                SaveAttribute ExtraSave = mono.GetType().GetCustomAttributes(typeof(SaveAttribute), true).FirstOrDefault() as SaveAttribute;
                if (ExtraSave != null) {
                    LastsaveAttribute = ExtraSave;

                    if (ExtraSave.SavePosition)
                        Field_Values.Add(new FieldValue("1", new SVector3(mono.transform.position)));

                    if (ExtraSave.SaveRotation)
                        Field_Values.Add(new FieldValue("2", new SQuaternion(mono.transform.rotation)));

                    if (ExtraSave.SaveScale)
                        Field_Values.Add(new FieldValue("3", new SVector3(mono.transform.localScale)));
                }                

                foreach (FieldInfo field in mono.GetType().GetFields(Flags)) {

                    SaveAttribute saveAttribute = SerializableFiles.IsMemberSaveAttribute(field);
                    if (saveAttribute != null) {
                        //To convert non-serializable objects to serializable objects
                        object FieldValue = SerializableObjects.Adapter.ToSerializableObject(field.GetValue(mono));
                        Field_Values.Add(new FieldValue(field.Name, FieldValue));
                        //To avoid invalid use of keys
                        if (ReferentValueKey != "" && ReferentValueKey != saveAttribute.ReferentValue) {
                            Debug.LogError("Only supports the use of a single key(" + ReferentValueKey + " != "+ saveAttribute.ReferentValue + ")");
                            return;
                        }
                        ReferentValueKey = saveAttribute.ReferentValue;                        
                        LastsaveAttribute = saveAttribute;
                    }

                }

                if (LastsaveAttribute != null && LastsaveAttribute.InstanceNewOnLoad.Equals(SaveAttribute.InstanceStatus.NotInstance)) {
                    //set by key!
                    if (PrefabPath.Length > 0) {
                        Debug.LogError("Only you can save with a type of stored, static or dynamic mode");
                        return;
                    }
                    
                    Searchkey = mono.GetType().GetField(LastsaveAttribute.ReferentValue, Flags).GetValue(mono).ToString();

                } else if(LastsaveAttribute != null && LastsaveAttribute.InstanceNewOnLoad.Equals(SaveAttribute.InstanceStatus.Instance)) {
                    //set by prefab
                    if (Searchkey.Length > 0) {
                        Debug.LogError("Only you can save with a type of stored, static or dynamic mode");
                        return;
                    }

					if (!GameObjectPrefabName.ContainsKey(mono.gameObject)) {
						Debug.LogError(string.Format("You can only save PrefabInstance!, {0} was not created by SerializableManager.PrefabInstantiate(GameObject prefab)",
							mono.gameObject.name));
                        return;
                    }

					GameObjectPrefabName.TryGetValue(mono.gameObject, out PrefabPath);
                } else {
                    Debug.LogError("You need save with a type of stored, static or dynamic mode");
                    return;
                }

                ScriptsList.Add(new MonoBehaviourValues(mono.GetType(), mono.enabled, Field_Values));
            }//script level

            GameObjectsValues.Add(new GameObjectValue(Searchkey, PrefabPath, ScriptsList));

        }//GameObject level

        //to show debug
        if (DebugSave) {
            foreach (GameObjectValue debu in GameObjectsValues) {
                Debug.Log(debu.ToString());
            }
        }

        SerializableFiles.Save(GameObjectsValues, filename);

        Debug.Log("Successfully save file: " + filename + (SerializableManager.current.Encrypt? " (Encrypted)" : ""));
    }

    public static void Load(string filename) {
        object loadobj = SerializableFiles.Load(filename);
        if(loadobj is String) {
            Debug.LogError(loadobj);
            return;
        }

        if(loadobj == null || (loadobj is List<GameObjectValue>) == false) {
            Debug.LogError("Could not load the file: " + filename);
            return;
        }
        List<GameObjectValue> LoadList = (List<GameObjectValue>)loadobj;

        //to show debug
        if (DebugSave) {
            foreach (GameObjectValue debu in LoadList) {
                Debug.Log(debu.ToString());
            }
        }
		
        foreach(GameObjectValue GameObjectToLoad in LoadList) {

            GameObject Load = null;

            if (GameObjectToLoad.PrefabPath.Length > 0) {
                //dynamic mode
				GameObject LoadPrefab = Resources.Load(GameObjectToLoad.PrefabPath, typeof(GameObject)) as GameObject;
				Load = GameObject.Instantiate(LoadPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				SerializableGame.GameObjectPrefabName.Add(Load, GameObjectToLoad.PrefabPath);
            }

            Dictionary<Type, int> repeated = new Dictionary<Type, int>();

                foreach (MonoBehaviourValues script in GameObjectToLoad.Scripts) {

                Type loadType = Type.GetType(script.ClassTypeName);

                if(GameObjectToLoad.PrefabPath.Length > 0) {
                    //dynamic mode

                    Component ToSet = null;
                    Component[] components = Load.GetComponents(loadType);
                    if(components.Length > 1) {

                        int value = 1;
                        if(repeated.TryGetValue(loadType, out value)) {
                            repeated.Remove(loadType);
                            repeated.Add(loadType, value + 1);
                            value++;
                        } else {
                            repeated.Add(loadType, value);
                        }
                        ToSet = components[value];

                    } else {
                        ToSet = components[0];
                    }

                    foreach (FieldValue Field_Values in script.ObjectsData) {

                        String Field = Field_Values.field;

                        if (Field == "1") {
                            Load.transform.position = (Field_Values.value as SVector3).getVector();
                        } else if (Field == "2") {
                            Load.transform.rotation = (Field_Values.value as SQuaternion).getQuaternion();
                        } else if (Field == "3") {
                            Load.transform.localScale = (Field_Values.value as SVector3).getVector();
                        } else {
                            loadType.GetField(Field, Flags).SetValue(ToSet, Adapter.FormSerializableObject(Field_Values.value));
                        }
                    }

                    ((MonoBehaviour)ToSet).enabled = script.Enable;
                    Load.SendMessage("OnLoad", SendMessageOptions.DontRequireReceiver);

				} else {
                    //static mode
                    foreach (object gameobject in UnityEngine.Object.FindObjectsOfType(loadType)) {

                        SaveAttribute atribute = SerializableFiles.IsMemberSaveAttribute(gameobject.GetType().GetField(script.ObjectsData[0].field, Flags));

						if ((loadType.GetField(atribute.ReferentValue).GetValue(gameobject).ToString()) == GameObjectToLoad.Searchkey) {

							foreach (FieldValue Field_Values in script.ObjectsData) {
                                gameobject.GetType().GetField(Field_Values.field, Flags).SetValue(gameobject, Field_Values.value);
                            }

                            ((MonoBehaviour)gameobject).enabled = script.Enable;
                            ((MonoBehaviour)gameobject).SendMessage("OnLoad", SendMessageOptions.DontRequireReceiver);
                        }

                    }

                }

            }

        }
        Debug.Log("Successfully loaded file: " + filename + (SerializableFiles.LastLoadIsEncrypt? " (Encrypted)" : ""));
    }
}