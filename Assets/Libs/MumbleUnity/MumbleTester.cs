/*
 * This is the front facing script to control how MumbleUnity works.
 * It's expected that, to fit in properly with your application,
 * You'll want to change this class (and possible SendMumbleAudio)
 * in order to work the way you want it to
 */
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using Mumble;

public class MumbleTester : MonoBehaviour {

    public GameObject MyMumbleAudioPlayerPrefab;
	public GameObject PlayerME;
    public MumbleMicrophone MyMumbleMic;
    public DebugValues DebuggingVariables;

    private MumbleClient _mumbleClient;
    public string HostName = "1.2.3.4";
    public int Port = 64738;
    public string Username = "ExampleUser";
    public string Password = "1passwordHere!";
    public string ChannelToJoin = "";

	void Start () {

		if (HostName == "1.2.3.4") {
			Debug.LogError ("Please set the mumble host name to your mumble server");
			return;
		} 
		else 
		{
			INIParser ini = new INIParser();
			// Open the save file. If the save file does not exist, INIParser automatically create
			// one
			ini.Open(Application.dataPath + "/MayaVerseLowPoly.ini");
            if (ini.IsKeyExists("NetworkConfig", "ServerVoiceIP"))
            {
                HostName = ini.ReadValue("NetworkConfig", "ServerVoiceIP", "127.0.0.1");
                Debug.Log("VoiceServerIP: " + HostName);
            }
            else
            {
                ini.WriteValue("NetworkConfig", "ServerVoiceIP", "127.0.0.1");
                Debug.Log("VoiceServerIP: " + HostName);
            }
            if (ini.IsKeyExists("NetworkConfig", "ServerVoicePort"))
            {
                Port = ini.ReadValue("NetworkConfig", "ServerVoicePort", 64738);
                Debug.Log("VoiceServerPort: " + Port.ToString());
            }
            else
            {
                ini.WriteValue("NetworkConfig", "ServerVoicePort", 64738);
                Debug.Log("VoiceServerPort: " + Port.ToString());
            }
            if (ini.IsKeyExists("VoiceAccount", "Login"))
            {
                Username = ini.ReadValue("VoiceAccount", "Login", "Vytek");
                Debug.Log("AvatarName: " + this.Username);
            }
            else
            {
                ini.WriteValue("VoiceAccount", "Login", "Vytek");
                Debug.Log("AvatarName: " + this.Username);
            }
            if (ini.IsKeyExists("VoiceAccount", "Password"))
            {
                Password = ini.ReadValue("VoiceAccount", "Password", "Vytek");
            }
            else
            {
                ini.WriteValue("VoiceAccount", "Password", "Vytek");
            }
			//Close file
			ini.Close();		
		}
        Application.runInBackground = true;
        _mumbleClient = new MumbleClient(HostName, Port, CreateMumbleAudioPlayerFromPrefab, DestroyMumbleAudioPlayer, DebuggingVariables);

        if (DebuggingVariables.UseRandomUsername)
            Username += UnityEngine.Random.Range(0, 100f);
        _mumbleClient.Connect(Username, Password);

        if(MyMumbleMic != null)
            _mumbleClient.AddMumbleMic(MyMumbleMic);

#if UNITY_EDITOR
        if (DebuggingVariables.EnableEditorIOGraph)
        {
            EditorGraph editorGraph = EditorWindow.GetWindow<EditorGraph>();
            editorGraph.Show();
            StartCoroutine(UpdateEditorGraph());
        }
#endif
    }
    private MumbleAudioPlayer CreateMumbleAudioPlayerFromPrefab()
    {
        // Depending on your use case, you might want to add the prefab to an existing object (like someone's head)
        // If you have users entering and leaving frequently, you might want to implement an object pool
        GameObject newObj = GameObject.Instantiate(MyMumbleAudioPlayerPrefab);
        if (PlayerGameObjectTracker.AllPlayerGameObjects.Count==1)
        {
            
            //http://answers.unity3d.com/questions/572176/how-can-i-instantiate-a-gameobject-directly-into-a-1.html
            Debug.Log("CreateMumbleAudioPlayerFromPrefab()");
            Debug.Log(this.PlayerME.transform.Find("Head").gameObject.transform);
            newObj.transform.parent = this.PlayerME.transform.Find("Head").gameObject.transform; //??    
        } else
        {
            foreach (GameObject PGO in PlayerGameObjectTracker.AllPlayerGameObjects)
            {
                if (newObj.gameObject.GetComponentInChildren<MumbleAudioPlayer>().MumbleUserName == PGO.gameObject.GetComponentInChildren<NetworkPlayerParent>().AvatarName)
                {
                    Debug.Log("GameObject Name To Attach MumbleAudioPlayer: " + PGO.name);
                    newObj.transform.parent = PGO.transform.Find("Head").gameObject.transform;
                }
            }
        }
        MumbleAudioPlayer newPlayer = newObj.GetComponent<MumbleAudioPlayer>();
        return newPlayer;
    }
    private void DestroyMumbleAudioPlayer(MumbleAudioPlayer playerToDestroy)
    {
        UnityEngine.GameObject.Destroy(playerToDestroy.gameObject);
    }
    void OnApplicationQuit()
    {
        Debug.LogWarning("Shutting down connections");
        if(_mumbleClient != null)
            _mumbleClient.Close();
    }
    IEnumerator UpdateEditorGraph()
    {
        long numPacketsReceived = 0;
        long numPacketsSent = 0;
        long numPacketsLost = 0;

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            long numSentThisSample = _mumbleClient.NumUDPPacketsSent - numPacketsSent;
            long numRecvThisSample = _mumbleClient.NumUDPPacketsReceieved - numPacketsReceived;
            long numLostThisSample = _mumbleClient.NumUDPPacketsLost - numPacketsLost;

            Graph.channel[0].Feed(-numSentThisSample);//gray
            Graph.channel[1].Feed(-numRecvThisSample);//blue
            Graph.channel[2].Feed(-numLostThisSample);//red

            numPacketsSent += numSentThisSample;
            numPacketsReceived += numRecvThisSample;
            numPacketsLost += numLostThisSample;
        }
    }
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _mumbleClient.SendTextMessage("This is an example message from Unity");
            print("Sent mumble message");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            print("Will attempt to join channel " + ChannelToJoin);
            _mumbleClient.JoinChannel(ChannelToJoin);
        }
	}
}
