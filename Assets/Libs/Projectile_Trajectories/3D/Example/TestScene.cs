using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScene : MonoBehaviour {



    public GameObject sphere;
    public Transform launchPosition;

    public Slider AngleSlider;
    public Slider LaunchSlider;

	void Start () {
	}

    public void Fire()
    {
        //Spawns the Ball with initial velocity in the same direciton as the canon's forward.
        GameObject r = Instantiate(sphere, launchPosition.position, Quaternion.identity);
        Quaternion a = Quaternion.Euler(0, launchPosition.rotation.eulerAngles.y, 0);
        r.GetComponent<Rigidbody>().velocity = launchPosition.forward * LaunchSlider.value;
    }
	
	void Update () {
        //Rotates canon using value from angle slider
        transform.rotation = Quaternion.Euler(AngleSlider.value, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        //Sets launch speed
        launchPosition.GetComponent<ThreeDTrajectory>().LaunchSpeed = LaunchSlider.value;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Fire(); 
        }


    }
}
