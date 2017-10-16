using System;

public class SaveAttribute : Attribute {

    public string ReferentValue;
    public bool SavePosition, SaveRotation, SaveScale;
    public enum InstanceStatus { Instance, NotInstance, Undefined };
    public InstanceStatus InstanceNewOnLoad = InstanceStatus.Undefined;
    /// <summary>
    /// When you want to load it will create a new object and put all the saved info
    /// </summary>
    public SaveAttribute() {
        InstanceNewOnLoad = InstanceStatus.Instance;
    }

    /// <summary>
    /// You will find an object that contains the same Reference Value that was used to save and to load all its stored information
    /// </summary>
    /// <param name="ReferentValue">The field used as a reference MUST BE A SINGLE VALUE BETWEEN OBJECTS OF THE SAME TYPE!</param>
    public SaveAttribute(string ReferentValue) {
        this.ReferentValue = ReferentValue;
        InstanceNewOnLoad = InstanceStatus.NotInstance;
    }

    /// <summary>
    /// To see what properties should save transform
    /// </summary>
    /// <param name="SavePosition">To save the position</param>
    /// <param name="SaveRotation">To save rotation</param>
    /// <param name="SaveScale">To save the scale</param>
    public SaveAttribute(bool SavePosition, bool SaveRotation, bool SaveScale) {
        this.SavePosition = SavePosition;
        this.SaveRotation = SaveRotation;
        this.SaveScale = SaveScale;
        InstanceNewOnLoad = InstanceStatus.Instance;
    }

}