using UnityEngine;
using DevConsole;

public class Example : MonoBehaviour {
    
    [Command]
    static void TimeScale(float value) {
        Time.timeScale = value;
        Console.Log("Change successful", Color.green);
    }

    [Command]
    static void ShowTime() {
        Console.Log(Time.time.ToString());
    }

    [Command]
    static void SetGravity(Vector3 value) {
        Physics.gravity = value;
    }    
}