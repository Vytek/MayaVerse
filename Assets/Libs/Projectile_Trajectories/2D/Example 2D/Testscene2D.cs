using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testscene2D : MonoBehaviour {

    public Transform LaunchPosition;
    private float LaunchSpeed = 0;
    private float LaunchAngle = 0;
    public float SpeedPerUnit = 10;

    public GameObject TwoDBullet;

    private Vector3 InitialPosition;

    private bool Dragging = false;

	void Start () {
        InitialPosition = LaunchPosition.transform.position;
	}
	
	void Update () {
        //Launch speed is calculated based on distance from intitial position
        LaunchSpeed = Vector3.Distance(InitialPosition, LaunchPosition.position) * SpeedPerUnit;
        //Displacement vector
        Vector3 d = InitialPosition - LaunchPosition.position;

        //Only enable drawing if we are dragging the object
        LaunchPosition.GetComponent<TwoDTrajectory>().Draw = Dragging;

        //Check if it's non zero , otherwise errors may occur
        if (d != Vector3.zero)
        {
            //Since we're calculating d(x,y)'s angle with a horizontal line , we can calculate the cosine by using the dot product , (1,0) dot (x,y) = x = length(d)*cos(theta) therefore cos(theta) = x/length(d)
            float cosine = (d.x) / d.magnitude;
            //Using determinants of 2d vectors , sine(theta) = d.y/length(d) , The sign of the angle is the same as the sign of the Sine. since length(d) is always positive , the sign of d.y is enough
            LaunchAngle = Mathf.Sign(d.y) * Mathf.Acos(cosine) * (180 / Mathf.PI);
            //Set rotation to the calculated angle
            LaunchPosition.rotation = Quaternion.Euler(LaunchPosition.rotation.eulerAngles.x, LaunchPosition.rotation.eulerAngles.y, LaunchAngle);
            //Set Launch Speed
            LaunchPosition.GetComponent<TwoDTrajectory>().LaunchSpeed = LaunchSpeed;
        }
        //Get mouse position
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Sets mouse position to the launcher's plane
        mouse.z = LaunchPosition.transform.position.z;
        //Check if mouse is on top of the launcher
        if (LaunchPosition.GetComponent<SpriteRenderer>().bounds.Contains(mouse))
        {
            //If we click for the first time , set to drag mode
            if (Input.GetMouseButton(0) && !Dragging)
            {
                Dragging = true;
            }
        }
        if (Dragging == true)
        {
            //If we're dragging , launcher follows mouse position
            LaunchPosition.position = mouse;
            //If we stop holding the mouse button
            if (!Input.GetMouseButton(0))
            {
                //Reset launcher
                LaunchPosition.position = InitialPosition;
                Dragging = false;
                //Spawn a projectile , with initial velocity facing the same direction as the launcher
                Rigidbody2D tD = Instantiate(TwoDBullet, mouse, Quaternion.identity).GetComponent<Rigidbody2D>();
                tD.velocity = LaunchSpeed * LaunchPosition.right;
            }
        }

    }
}
