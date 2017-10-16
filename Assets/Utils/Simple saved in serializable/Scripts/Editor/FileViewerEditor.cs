using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class FileViewerEditor : EditorWindow {

    //cache
    string lastpassword = null;
    //        File name, Type
    Dictionary<String, SerializableFiles.ValidsTypes> CacheEncrytion = new Dictionary<String, SerializableFiles.ValidsTypes>();

    Vector2 ScrollViewPos;
    bool FileViewActive = false;
    volatile string FilesPath = null;
    string CurrentLoadPath;
    Dictionary<string, bool> FoldoutStatus = new Dictionary<string,bool>();

    //All the GUIStyles!
    private static GUIStyle Tab1, Tab2, Tab3, TextButton, ValidFile, NotValidFile, TextButtonRight, FileName;
    
    private object GameLoad = null;

    [MenuItem("Window/Simple saved in serializable/File Viewer")]
    static void Init() {
        EditorWindow view = EditorWindow.GetWindow(typeof(FileViewerEditor));
        view.titleContent.text = "File Viewer";
        view.titleContent.image = EditorGUIUtility.FindTexture("Project"); 
        view.Show();
    }

    void OnInspectorUpdate() {
        Repaint();
    }

	void OnGUI() {
		if(FilesPath == null)
			FilesPath = Application.persistentDataPath;
		
        if (Tab1 == null)
            Tabs();

        EditorGUILayout.BeginHorizontal();

        GUI.enabled = this.FileViewActive;
        if (GUILayout.Button("Files Explorer", GUILayout.Width(100), GUILayout.Height(50))) {
            this.FileViewActive = false;
        }
        GUI.enabled = !this.FileViewActive && this.GameLoad != null;
        if (GUILayout.Button("File View", GUILayout.Width(100), GUILayout.Height(50))) {
            this.FileViewActive = true;
        }
        GUI.enabled = true;
        if (GUILayout.Button("Refresh", GUILayout.Width(100), GUILayout.Height(50))) {
            if(this.FileViewActive)
                this.LoadGame(this.CurrentLoadPath);
            else{
                lastpassword = null;
                CacheEncrytion.Clear();
            }
        }
        GUI.enabled = true;                

        if (this.FileViewActive && CurrentLoadPath != null) {
            EditorGUILayout.LabelField(CurrentLoadPath + (CurrentLoadPath.Contains(Encryption.FileType) ? "   (encrypted)" : ""), FileName,
                GUILayout.Height(50));
        } else {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Files path", EditorStyles.boldLabel, GUILayout.Height(20));
            FilesPath = EditorGUILayout.TextField(FilesPath);//Files path
            EditorGUILayout.EndVertical();
        }       

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (!this.FileViewActive && Application.persistentDataPath != FilesPath) {
            Color defaultCo = GUI.color;
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Are not in the folder that is used to load! which it is: '" + Application.persistentDataPath + "'", MessageType.Warning);
            if (GUILayout.Button("FIX", GUILayout.Width(70), GUILayout.Height(40))) {
                FilesPath = Application.persistentDataPath;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = defaultCo;
        }

        EditorGUILayout.BeginVertical("BOX");
        ScrollViewPos = EditorGUILayout.BeginScrollView(ScrollViewPos);
        
        

        if (this.FileViewActive)           
            this.FileView();   
        else
            this.FilesExplorer();        

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void FileView() {
        //not valid load
        if (GameLoad == null) {
            //i have to load?
            if (this.CurrentLoadPath == null || this.CurrentLoadPath.Length < 5) {
                //not have, is SerializableManager valid?
                if (SerializableManager.current != null) {
                    SerializableFiles.ValidsTypes valid = SerializableFiles.IsValid(SerializableManager.current.DefaultFileName);
                    if (valid == SerializableFiles.ValidsTypes.Valid || valid == SerializableFiles.ValidsTypes.Encrypted) {
                        LoadGame(SerializableManager.current.DefaultFileName);
                        return;
                    }
                }
                this.FileViewActive = false;//change to File Explorer
                return;
            } else {
                SerializableFiles.ValidsTypes valid = SerializableFiles.IsValid(this.CurrentLoadPath);
                if (valid == SerializableFiles.ValidsTypes.Valid || valid == SerializableFiles.ValidsTypes.Encrypted) {
                    LoadGame(this.CurrentLoadPath);
                } else {
                    this.FileViewActive = false;//change to File Explorer
                    return;
                }
            }
        }
        if (GameLoad is String) {
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox((String)GameLoad, MessageType.Error);
            return;
        }

        List<GameObjectValue> GameObject = (List<GameObjectValue>)GameLoad;

        for (int IDGameObject = 0; IDGameObject < GameObject.Count; IDGameObject++)
            if (Foldout(-1, IDGameObject, GameObjectName(GameObject[IDGameObject].Searchkey, GameObject[IDGameObject].PrefabPath, GameObject[IDGameObject].Scripts), Tab1))
                for (int IDscript = 0; IDscript < GameObject[IDGameObject].Scripts.Count; IDscript++) {

                    MonoBehaviourValues script = GameObject[IDGameObject].Scripts[IDscript];

                    if (Foldout(IDGameObject, IDscript, ScriptName(script.ClassTypeName) + "(Script)   " + (script.Enable ? "Enable" : "Disabled"), Tab2)) {
                        EditorGUILayout.LabelField("Field = (Value) as (Type)", Tab3);
                        foreach (FieldValue FieldValue in script.ObjectsData) {
                            EditorGUILayout.LabelField(AdapterField(FieldValue.field) + " = (" + FieldValue.value.ToString() + ")  as  (" + FieldValue.value.GetType().FullName + ")", Tab3);
                        }
                    }
                }
    }

    private void FilesExplorer() {

        if (SerializableManager.current.Encrypt && SerializableManager.current.password != lastpassword) {
            this.CacheEncrytion.Clear();//old
        }

        //Back...
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("[FOLDER]", EditorStyles.boldLabel, GUILayout.Width(90));
        EditorGUILayout.LabelField("[BACK]", GUILayout.MinWidth(20));
        if (GUILayout.Button("Open", GUILayout.Width(110))) {
            GUIUtility.keyboardControl = 0;
            FilesPath = Path.GetFullPath(Path.Combine(FilesPath, @"..\"));
        }
        //to aling
        EditorGUILayout.LabelField("", EditorStyles.label, GUILayout.Width(110 * 2 + 4));
        EditorGUILayout.EndHorizontal();

        try {
            foreach (string DirectoryPath in Directory.GetDirectories(FilesPath)) {
                string FolderName = Path.GetFileName(DirectoryPath);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("[FOLDER]", EditorStyles.boldLabel, GUILayout.Width(90));
                EditorGUILayout.LabelField(FolderName, GUILayout.MinWidth(20));

                if (GUILayout.Button("Open", GUILayout.Width(110))) {
                    GUIUtility.keyboardControl = 0;
                    FilesPath = DirectoryPath;
                }
                //to aling
                EditorGUILayout.LabelField("", EditorStyles.label, GUILayout.Width(110 * 2 + 4));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            foreach (string FilePath in Directory.GetFiles(FilesPath)) {
                string FileName = Path.GetFileName(FilePath);
                if (FileName == "Thumbs.db") continue;

                if (lastpassword == null || SerializableManager.current.password != lastpassword || !CacheEncrytion.ContainsKey(FileName)) {
                    //create cache
                    CacheEncrytion.Add(FileName, SerializableFiles.IsValid(FilePath));
                }

                SerializableFiles.ValidsTypes ValidSave = SerializableFiles.ValidsTypes.Error;  //SerializableFiles.IsValid(FilePath);
                CacheEncrytion.TryGetValue(FileName, out ValidSave);

                EditorGUILayout.BeginHorizontal();
                bool valid = ValidSave != SerializableFiles.ValidsTypes.Error && ValidSave != SerializableFiles.ValidsTypes.ERROR_Enc;
                //File status
                EditorGUILayout.LabelField("[" + ValidSave.ToString().Replace("_", " ").ToUpper() + "]", valid ? ValidFile : NotValidFile, GUILayout.Width(90));
                //File name
                EditorGUILayout.LabelField(FileName, GUILayout.MinWidth(200));
                //File size
                EditorGUILayout.LabelField(FileSize(FilePath), TextButtonRight, GUILayout.Width(100));
                //shows buttons?
                if (valid) {

                    //View Button
                    if (GUILayout.Button("View", GUILayout.Width(110))) {
                        this.LoadGame(FileName);
                    }
                    //Load Button
                    GUI.enabled = SerializableManager.current != null && SerializableManager.current.enabled && Application.isPlaying;
                    if (GUILayout.Button("Load", GUILayout.Width(110))) {
                        SerializableManager.LoadAll(FileName);
                    }
                    //Default Button
                    GUI.enabled = SerializableManager.current != null;
                    if (GUILayout.Button((SerializableManager.current.DefaultFileName == FileName ? "Remove Default" : "Set Default"), GUILayout.Width(110))) {
                        if (SerializableManager.current.DefaultFileName == FileName) {
                            FileName = SerializableManager.DefaultSaveName;
                        }
                        SerializableManager.current.DefaultFileName = FileName;
                    }
                } else {
                    if (ValidSave == SerializableFiles.ValidsTypes.Error) {
                        EditorGUILayout.LabelField("This is not a valid saved", EditorStyles.label, GUILayout.Width(110 * 3 + 8));
                    } else if (ValidSave == SerializableFiles.ValidsTypes.ERROR_Enc) {
                        EditorGUILayout.LabelField("The saved could not be decrypted, bad password?", EditorStyles.label, GUILayout.Width(110 * 3 + 8));
                    } else {
                        EditorGUILayout.LabelField("INTERAL ERROR (A161)", EditorStyles.label, GUILayout.Width(110 * 3 + 8));
                    }

                }
                //Restart GUI Status
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
        } catch (FileNotFoundException e) {
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox("The route " + e.FileName + " is not a folder!", MessageType.Error);
        } catch (Exception e) {
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox("Error: " + e.Message, MessageType.Error);
        }

        lastpassword = SerializableManager.current.password;
    }

    static void Tabs() {
        //Tab
        Tab1 = new GUIStyle(EditorStyles.foldout);
        Tab1.margin = new RectOffset(0, 0, 0, 0);
        Tab1.fontStyle = FontStyle.Bold;
        Tab1.fontSize = 12;
        //Tab2
        Tab2 = new GUIStyle(EditorStyles.foldout);
        Tab2.margin = new RectOffset(20, 0, 0, 0);
        Tab2.fontStyle = FontStyle.Bold;
        //Tab3
        Tab3 = new GUIStyle(EditorStyles.label);
        Tab3.padding = new RectOffset(40, 0, 0, 0);
        //TextButton
        TextButton = new GUIStyle(EditorStyles.boldLabel);
        TextButton.alignment = TextAnchor.MiddleLeft;
        //TextButtonRight
        TextButtonRight = new GUIStyle(EditorStyles.boldLabel);
        TextButtonRight.alignment = TextAnchor.MiddleRight;
        //ValidFile
        ValidFile = new GUIStyle(EditorStyles.boldLabel);
        ValidFile.normal.textColor = new Color(0, 0.65f, 0);
        //NotValidFile
        NotValidFile = new GUIStyle(EditorStyles.boldLabel);
        NotValidFile.normal.textColor = Color.red;
        //Title
        FileName = new GUIStyle(EditorStyles.boldLabel);
        FileName.alignment = TextAnchor.MiddleLeft;
        FileName.fontSize = 12;
    }

    void LoadGame(string FileName) {
        if (FileName == null || FileName.Length < 5) return;
        this.FileViewActive = true;
        GameLoad = SerializableFiles.Load(FileName.Replace(Encryption.FileType, ""));
        FoldoutStatus.Clear();
        this.CurrentLoadPath = FileName;
    }

    private bool Foldout(int block1, int block2, string name, GUIStyle guiStyle) {
        bool status = false;
        FoldoutStatus.TryGetValue(block1 + "." + block2,out status);
        status = EditorGUILayout.Foldout(status, name, guiStyle);
        FoldoutStatus.Remove(block1 + "." + block2);
        FoldoutStatus.Add(block1 + "." + block2, status);
        return status;
    }

    private string AdapterField(string name) {
        if (name == "1") {
            return "transform.position";
        }else if(name == "2") {
            return "transform.rotation";
        }else if(name == "3") {
            return "transform.localScale";
        }
        return name;
    }

    private string GameObjectName(string key, string prefab, List<MonoBehaviourValues> KeyData) {
        if(key != "") {
            Type loadType = Type.GetType(KeyData[0].ClassTypeName);
            if (loadType == null) return "Null ERROR " + KeyData[0].ClassTypeName;
            SaveAttribute atribute = SerializableFiles.IsMemberSaveAttribute(loadType.GetField(KeyData[0].ObjectsData[0].field, SerializableGame.Flags));

            return "The key: " + key + " is for " + (loadType.GetField(atribute.ReferentValue).Name + " object");
        }else if(prefab != "") {
            if(prefab.IndexOf("/") > 0)
                return prefab.Substring(prefab.LastIndexOf("/")+1);
            return prefab;
        }

        return "Key and Prefab ERROR!!";
    }

    private string ScriptName(string name) {
        Type loadType = Type.GetType(name);
        if (loadType == null) return "Null ERROR " + name;
        return loadType.FullName;
    }

    string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB" };
    private string FileSize(string filename) {
        double len = new FileInfo(filename).Length;
        int order = 0;
        while (len >= 1024 && order + 1 < sizes.Length) {
            order++;
            len = len / 1024;
        }
        if (order == 0) {
            if(len != 0)
                len = 1;
            order = 1;
        }
        return String.Format("{0:0} {1}", Math.Round(len), sizes[order]);
    }
}