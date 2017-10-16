using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public static class SerializableFiles {
    internal static bool LastLoadIsEncrypt;

    public enum ValidsTypes { Valid, Error, Encrypted, ERROR_Enc }

    public static void Save(List<GameObjectValue> ToSave, String filename) {
        String FilePath = Application.persistentDataPath + "/" + filename;
        if (SerializableManager.current.Encrypt) {
            Encryption.Encryptor(FilePath + Encryption.FileType, ToSave);            
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        } else {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(FilePath);
            bf.Serialize(file, ToSave);
            file.Close();
            if (File.Exists(FilePath + Encryption.FileType))
                File.Delete(FilePath + Encryption.FileType);
        }
    }

    public static object Load(string filename) {
        string FilePath = Application.persistentDataPath + "/" + filename;
        LastLoadIsEncrypt = false;
        if (File.Exists(FilePath + Encryption.FileType)) {
            if (!SerializableManager.current.Encrypt) {
                return "You cannot decrypt a save with the encryption turned off";
            }
            LastLoadIsEncrypt = true;
            return Encryption.Decryptor(FilePath + Encryption.FileType);
        }

        if (!File.Exists(FilePath)) {
            FilePath = Application.persistentDataPath + filename;

            if (!File.Exists(FilePath)) {
                return (string.Format("The file you are looking for does not exist!. File name: {0}", filename));
            }
        }
        try {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FilePath, FileMode.Open);
            object load = bf.Deserialize(file);
            file.Close();

            if (load is List<GameObjectValue>) {
                return (List<GameObjectValue>)load;
            }
            return "Error loading level, the level may be an old version";
        } catch {
            return null;
        }
    }

    public static ValidsTypes IsValid(string path) {
        if (path.Contains(Encryption.FileType)) {
            if (Encryption.Decryptor(path) is List<GameObjectValue>) {
                return ValidsTypes.Encrypted;
            }
            return ValidsTypes.ERROR_Enc;
        }

        BinaryFormatter bf;
        FileStream file = null;
        try {
            if (File.Exists(path.ToString())) {
                bf = new BinaryFormatter();
                file = File.Open(path, FileMode.Open);
                object load = bf.Deserialize(file);
                file.Close();
                if (load is List<GameObjectValue>)
                    return ValidsTypes.Valid;
            }
        } catch {
            if (file != null)
                file.Close();
        }
        return ValidsTypes.Error;
    }

    public static SaveAttribute IsMemberSaveAttribute(FieldInfo member) {
        foreach (object attribute in member.GetCustomAttributes(true))
            if (attribute is SaveAttribute)
                return attribute as SaveAttribute;
        return null;
    }

    public static bool NeedLink(MonoBehaviour check) {
        if (check.GetType().GetCustomAttributes(typeof(SaveAttribute), true).FirstOrDefault() is SaveAttribute) {
            return true;
        }
        foreach (FieldInfo field in check.GetType().GetFields()) {
            SaveAttribute saveAttribute = SerializableFiles.IsMemberSaveAttribute(field);
            if (saveAttribute != null) {
                return true;
            }
        }
        return false;
    }

}


[Serializable]
public class GameObjectValue {
    public String Searchkey;
    public String PrefabPath;
    public List<MonoBehaviourValues> Scripts;
    public GameObjectValue(string Searchkey, string PrefabPath, List<MonoBehaviourValues> Scripts) {
        this.Searchkey = Searchkey;
        this.PrefabPath = PrefabPath;
        this.Scripts = Scripts;
    }
}

[Serializable]
public class MonoBehaviourValues {
    public string ClassTypeName;
    public bool Enable;
    public List<FieldValue> ObjectsData;
    public MonoBehaviourValues(Type type, bool Enable, List<FieldValue> ObjectData) {
        this.ClassTypeName = type.AssemblyQualifiedName;
        this.ObjectsData = ObjectData;
        this.Enable = Enable;
    }
}

[Serializable]
public class FieldValue {
    public string field;
    public object value;
    public FieldValue(string field, object value) {
        this.field = field;
        this.value = value;
    }
}