using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDTrajectory : MonoBehaviour {


    [Header("Launch parameters")]
    //Indicate whether or not the Launch angle should depend on the LaunchPosition's rotation.
    public bool FixedLaunchAngle = false;
    //Indicates whether or not the angle should be inverted.
    public bool InvertLaunchAngle = false;
    //Launch angle
    public float LaunchAngle = 0;
    //Initial launch velocity
    public float LaunchSpeed = 25;
    //A transform that gives information about position and rotation of launch
    public Transform LaunchPosition;
    public bool Draw = true;
    [Header("Trajectory Preview parameters")]
    //Number of lines to be drawn to represent the trajectory
    public int LineCount = 5;
    //Size of each line
    public float LineLength = 1.0f;
    //Distance that will be covered by the preview.
    public float MaxDistance = 50;
    //Lines materials
    public Material PreviewMaterial;
    //Line scrolling speed
    public float ScrollSpeed = 1.5f;

    //A list to store all the spawned line renderers
    private List<GameObject> LineRenderers = new List<GameObject>();
    //Scroll interpolant , this will go from 0 to 1 then reset to 0
    private float Interpolant = 0;
    //To check if the linecount changed and initiate a reset.

    private GameObject Lineparent;


    //Calculates the y coordinate of the projectile given its x coordinate (assuming projectile starts from origin and is on the (Z,Y) plane)
    public float TrajectoryFunction(float x)
    {
        //The tangent function is not defined for x = pi/2 , we can cheat a little by changing it to value close to 90 degrees and get approximately a vertical line.
        if (LaunchAngle == 90) LaunchAngle = 89.99f; 

        float alpha = LaunchAngle * (float)Mathf.PI / 180; //transform to radians 
        float c = Mathf.Cos(alpha);
        float g = Physics.gravity.y;
        float x0 = LaunchPosition.position.x;
        float y0 = LaunchPosition.position.y;
        return (g * x * x) / (2 * LaunchSpeed * LaunchSpeed * c * c) + Mathf.Tan(alpha) * x;
    }

    private void Awake()
    {
        Lineparent = new GameObject("Trajectory LRenderers");
    }

    void Start () {
        if (!LaunchPosition) LaunchPosition = transform;

        ResetRenderers();
    }


    //Respawn the Line renderers
    public void ResetRenderers()
    {
        //Destroy old renderers
        for (int i = 0; i < LineRenderers.Count; i++) Destroy(LineRenderers[i]);
        LineRenderers.Clear();
        //Spawn new renderers
        for (int i = 0; i < LineCount;i++)
        {
            GameObject newLine = new GameObject("Line " + i.ToString());
            newLine.AddComponent(typeof(LineRenderer));
            newLine.GetComponent<LineRenderer>().material = PreviewMaterial;
            newLine.transform.parent = Lineparent.transform;
            LineRenderers.Add(newLine);
        }
    }

    public void SetLinePositions()
    {
        if (Draw)
        {
            //Check if we are shooting right or left based on the angle.
            float sign = Mathf.Sign(Mathf.Cos(LaunchAngle * (Mathf.PI / (float)180)));
            float step = MaxDistance / (LineRenderers.Count) * sign;
            for (int i = 0; i < LineRenderers.Count; i++)
            {
                //Reenable renderers :
                LineRenderers[i].GetComponent<LineRenderer>().enabled = true;
                float x = (float)i * step + Interpolant * step;
                Quaternion a = LaunchPosition.transform.rotation;
                float XRotation = a.eulerAngles.x;
                if (InvertLaunchAngle) XRotation = -XRotation;
                //Since we assumed we're shooting in the (Z,Y) plane , a rotation is required. Therefore , we take the y rotation of the Launch transform , and apply that to our positions.
                a = Quaternion.Euler(0, a.eulerAngles.y, 0);
                if (!FixedLaunchAngle)
                {
                    LaunchAngle = XRotation;
                }
                //Since we assumed at the TrajectoryFunction that we're starting from the origin. We offset to the launch position , then we rotate the trajectory around the Y axis.
                Vector3 Pos1 = a * new Vector3(0, (float)TrajectoryFunction(x), x) + LaunchPosition.position;
                Vector3 Pos2 = a * new Vector3(0, TrajectoryFunction(x + LineLength), x + LineLength) + LaunchPosition.position;

                //Inverse direction based on the angle (So images wont appear facing the wrong way)


                if (sign >= 0)
                {
                    LineRenderers[i].GetComponent<LineRenderer>().SetPosition(0, Pos1);
                    LineRenderers[i].GetComponent<LineRenderer>().SetPosition(1, Pos2);
                }
                else
                {
                    LineRenderers[i].GetComponent<LineRenderer>().SetPosition(1, Pos1);
                    LineRenderers[i].GetComponent<LineRenderer>().SetPosition(0, Pos2);
                }
            }
        }
        else
        {
            //if we don't want to draw, disable all renderers : 
            for (int i = 0;i < LineRenderers.Count;i++) LineRenderers[i].GetComponent<LineRenderer>().enabled = false;
        }
    }
	
	void Update () {
       
        Interpolant += Time.deltaTime * ScrollSpeed;
        if (Interpolant >= 1) Interpolant = 0;

        //Check if line count changed.
        if (LineCount != LineRenderers.Count) ResetRenderers();

        SetLinePositions();
   
	}
}
