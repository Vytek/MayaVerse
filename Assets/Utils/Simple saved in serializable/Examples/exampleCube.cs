using UnityEngine;

[RequireComponent(typeof(SerializableComponent))]
[Save(true, true, true)]
public class exampleCube : MonoBehaviour {

    [Save]
    public float Speed;

    [Save]
    private Color color;
    
	void Awake() {
        Speed = Random.Range(.5f, 5f);
	}
	
	void Update () {

        if (Random.Range(0, 500) == 1) {
            color = Color.blue;
        }
        if (Random.Range(0, 500) == 1) {
            color = Color.green;
        }
        if (Random.Range(0, 500) == 1) {
            color = Color.red;
        }
        if (Random.Range(0, 500) == 1) {
            color = Color.white;
        }

        gameObject.GetComponent<Renderer>().material.color = color;

        transform.Translate(Vector3.down * Time.deltaTime * Speed, Space.World);
        if (transform.position.y < 0) {
            Destroy(gameObject);
        }

        transform.Rotate(new Vector3(1, 1, 0) * Time.deltaTime * 10);
	}

    void OnLoad() {
        //Debug.Log("On Load event!");
    }
}