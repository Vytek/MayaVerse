using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(SerializableComponent))]
public class SerializableComponentEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SerializableComponent serializableComponent = (SerializableComponent)target;

        string Stored = DetectStoredFormat();
        if(Stored == "ERROR") {
            EditorGUILayout.HelpBox("Only you can save with a type of stored, static or dynamic mode", MessageType.Error);
        }else if (Stored == "Internal ERROR") {
            EditorGUILayout.HelpBox("It could not be detected in the way that is going to save", MessageType.Error);
        }

        GUILayout.Label(string.Format("This gameobject is linked with the system stored in {0}", Stored));

        foreach (MonoBehaviour link in serializableComponent.ValidToLinkComponents.ToArray()) {
            if (link == null) {
                serializableComponent.ValidToLinkComponents.Remove(link);
            } else {
                GUILayout.Label(string.Format("The {0} file is valid and will be saved", link.GetType().Name));
            }
        }

        if (GUILayout.Button("Search again")) {
            serializableComponent.DetectComponents();
        }

    }

    private string DetectStoredFormat() {
        bool staticMode = false, DynamicMode = false;

        //script level
        foreach (MonoBehaviour mono in ((SerializableComponent)target).GetComponents<MonoBehaviour>()) {
            if (!SerializableFiles.NeedLink(mono)) continue;
            //dynamic posistion,etc.. save..
            SaveAttribute FirstOrDefault = (mono.GetType().GetCustomAttributes(typeof(SaveAttribute), true).FirstOrDefault() as SaveAttribute);
            if (FirstOrDefault != null && FirstOrDefault.InstanceNewOnLoad == SaveAttribute.InstanceStatus.Instance) {
                DynamicMode = true;
            }

            foreach (FieldInfo field in mono.GetType().GetFields(SerializableGame.Flags)) {

                SaveAttribute saveAttribute = SerializableFiles.IsMemberSaveAttribute(field);
                if (saveAttribute != null) {
                    if (saveAttribute.InstanceNewOnLoad == SaveAttribute.InstanceStatus.NotInstance) {
                        //static
                        staticMode = true;
                    } else if(saveAttribute.InstanceNewOnLoad == SaveAttribute.InstanceStatus.Instance) {
                        //dynamic
                        DynamicMode = true;
                    }
                }

            }
        }
        if(staticMode && DynamicMode) {
            return "ERROR";//error box
        }

        if (staticMode) {
            return "static mode";
        }

        if (DynamicMode) {
            return "dynamic mode";
        }

        return "Internal ERROR";//error box

    }

}