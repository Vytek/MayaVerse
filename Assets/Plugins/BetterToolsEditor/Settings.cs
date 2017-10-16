using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("Settings")]
public class SettingsSerializer {

    public ToolSettings moveTool   = new ToolSettings();
    public ToolSettings rotateTool = new ToolSettings();
    public ToolSettings scaleTool  = new ToolSettings();

    public void Save() {
        var serializer = new XmlSerializer(typeof(SettingsSerializer));
        using (var stream = new FileStream(Path.Combine(Application.dataPath + "/Plugins/BetterToolsEditor/Settings", "settings.xml"), FileMode.Create)) {
            serializer.Serialize(stream, this);
        }
    }

    public static SettingsSerializer Load() {
        var serializer = new XmlSerializer(typeof(SettingsSerializer));
        using (var stream = new FileStream(Path.Combine(Application.dataPath + "/Plugins/BetterToolsEditor/Settings", "settings.xml"), FileMode.Open)) {
            return serializer.Deserialize(stream) as SettingsSerializer;
        }
    }
}

public class ToolSettings {
    
    [XmlElement("Ruler")]
    public bool ruler = true;

    [XmlElement("Label")]
    public bool label = true;

    [XmlElement("ColorX")]
    public Color x = Color.red;

    [XmlElement("ColorY")]
    public Color y = Color.red;

    [XmlElement("ColorZ")]
    public Color z = Color.red;
}
