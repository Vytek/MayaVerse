using UnityEngine;

[RequireComponent(typeof(SerializableComponent))]
public class exampleCubeControl : MonoBehaviour {

    public GameObject Prefab;

    public int ID = 1;

    [Save("ID")]
    public float time;
    	
	// Update is called once per frame
	void Update () {
	    if(Random.Range(1, 70) == 1) {
			GameObject newObject = SerializableManager.PrefabInstantiate(Prefab);
            newObject.transform.position = transform.position + new Vector3(Random.Range(-10, 10),0,0);
        }

        time += Time.deltaTime;
	}


    void OnGUI() {
        if (GUI.Button(new Rect(10, 10, 150, 20), "Save Game")) {
            SerializableManager.SaveAll();
        }
        if (GUI.Button(new Rect(10, 50, 150, 20), "Load Game")) {
            //to erase those now!
            foreach (exampleCube cubes in GameObject.FindObjectsOfType<exampleCube>()) {
                Destroy(cubes.gameObject);
            }
            SerializableManager.LoadAll();
        }

        GUI.Label(new Rect(10, 300, 200, 20), "Time: " + (int)time);
    }
}
