Tutorial on how to use "Simple saved in serializable"


#### In any use ####

Whenever you have a class that uses some kind of attribute that you wish to save on disk, simply add the following tag at the beginning of the class:
[RequireComponent (typeof (SerializableComponent))]
This makes unity add the "Serializable component" Component to your GameObject automatically

It is also necessary to have a "Serializable Manager" in a GameObject somewhere in the scene as the api is in that class

#### Warning ####

Never use Destroy (Object obj, float time) with a time parameter on an object that you want to save.
This will cause Unity to remove the object before it can be saved when the game is closed.

#### Events ####

Call SerializableManager.SaveAll(); to save all properties and SerializableManager.LoadAll(); to load them.

Whenever an object of any type is loaded it will call this function:
void OnLoad() {}

#### Warning ####

Awake is called before the API is loaded, use Start.

#### Supported objects ####

Any object that is a Serializable type can be stored
https://msdn.microsoft.com/en-us/library/ms973893.aspx

The following Unity types are also supported:
Vector2, Vector3, Vector4, Quaternion and Color

Simply include the SerializableObjects class in your script (using SerializableObjects;) and use the custom types SVector2, SVector3, SVector4, etc...

######## Saving static objects (simple) ########

To save a static property a unique ID or type string or int must be used. For example:
public int ID = 1;

[Save("ID")]
public float time;

The ID in this case is the int property called "ID", the info that will be saved and loaded is the float "time".
Use this ID to save/load a field, ie. [Save("ID")]

######## Saving an object dynamically (not so simple) ########

You can save and load objects created during runtime.
Objects that you want to save during runtime must have their prefab placed in a Resources folder somewhere in your project.
Then, to instantiate them use:

	"SerializableManager.PrefabInstantiate (GameObject prefab)" this will return the GameObject's instance

######## Saving Transform properties ########

If you want to save the properties of a transform (position, rotation, scale) put the following tag on top of the class:
[Save (true, true, true)]
The first paramenter is for saving the posision, the second rotation and the third scale.
All fields tagged with [Save] will be saved and loaded as expected.

######## contact ########
For any questions or problems do not hesitate to contact me by my mail: zedgeincorporation@gmail.com