/******************************************************************************
 Disclaimer Notice:
 This file is provided as is with no warranties of any kind and is
 provided without any obligation on Fork Particle, Inc. to assist in 
 its use or modification. Fork Particle, Inc. will not, under any
 circumstances, be liable for any lost revenue or other damages arising 
 from the use of this file.
 
 (c) Copyright 2017 Fork Particle, Inc. All rights reserved.
******************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ForkParticlePlugin : MonoBehaviour {

	public bool bForkSDKInit	= false; 

	private Coroutine RenderRoutine;
	private ArrayList effectsList = new ArrayList(); 
	private bool bForkSDKShutdown = false;

	#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#elif UNITY_EDITOR_WIN
	[DllImport ("forkParticlePluginX64")]
	#elif UNITY_EDITOR_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_WIN
	[DllImport ("forkParticlePluginX86")]
	#endif
	private static extern bool _frkParticlePluginSDKInit();

	#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#elif UNITY_EDITOR_WIN
	[DllImport ("forkParticlePluginX64")]
	#elif UNITY_EDITOR_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_WIN
	[DllImport ("forkParticlePluginX86")]
	#endif
	private static extern bool  _frkParticlePluginSDKShutdown();

	#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#elif UNITY_EDITOR_WIN
	[DllImport ("forkParticlePluginX64")]
	#elif UNITY_EDITOR_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_WIN
	[DllImport ("forkParticlePluginX86")]
	#endif
	private static extern IntPtr GetRenderEventFunc();

	#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#elif UNITY_EDITOR_WIN
	[DllImport ("forkParticlePluginX64")]
	#elif UNITY_EDITOR_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_WIN
	[DllImport ("forkParticlePluginX86")]
	#endif
	private static extern void _frkParticlePluginSetEffectView(Matrix4x4 viewMat, Matrix4x4 projMat, Vector3 Up, Vector3 Right, Vector3 Forward, Vector3 Position);

	#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#elif UNITY_EDITOR_WIN
	[DllImport ("forkParticlePluginX64")]
	#elif UNITY_EDITOR_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_WIN
	[DllImport ("forkParticlePluginX86")]
	#endif
	private static extern void _frkParticlePluginDestroyEffect (System.IntPtr pEffect);

	#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#elif UNITY_EDITOR_WIN
	[DllImport ("forkParticlePluginX64")]
	#elif UNITY_EDITOR_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_OSX
	[DllImport ("PluginMAC")]
	#elif UNITY_STANDALONE_WIN
	[DllImport ("forkParticlePluginX86")]
	#endif
	private static extern bool _frkParticlePluginSDKProcess (float fFrameTDelta);

	/***********************************************************************/

	private static ForkParticlePlugin _instance = null;
    public string TexturePath = "ForkFX/";

	public static ForkParticlePlugin Instance {
		get {
			return _instance;
		}
	}

	void Awake() {
		if (_instance) {
			GameObject.Destroy (_instance.gameObject);
			_instance = null;
		}

		_instance = this;
	}

	// Use this for initialization
	void Start () {		
		// Initializes the Fork SDK. 
		// The SDK must be initialised 
		// before creating any effects. 
		bForkSDKInit = _frkParticlePluginSDKInit ();
	}

	void Update()
	{
		Camera cam = Camera.main;
		_frkParticlePluginSetEffectView (cam.worldToCameraMatrix, cam.projectionMatrix, cam.transform.up, cam.transform.right, cam.transform.forward, cam.transform.position);
		_frkParticlePluginSDKProcess (1.0f / 60.0f);
	}

	public void AddEffect(GameObject obj)
	{
		effectsList.Add (obj);
	}

	public void RemoveEffect(GameObject obj)
	{
		effectsList.Remove (obj);
	}

	public bool ShutDownForkSDK()
	{
		if (bForkSDKShutdown)
			return true;
		
		InvalidateObjects ();
        // release textures
        ForkParticleEffect.DestroyTextures();

		// Shuts down the Fork SDK and destroy all the effects
		// references in the dll.
        bForkSDKShutdown = _frkParticlePluginSDKShutdown ();

		return bForkSDKShutdown;
	}

	void InvalidateObjects()
	{
		for (int i = 0; i < effectsList.Count; i++) {
			GameObject obj = (GameObject)effectsList [i];
			obj.GetComponent<ForkParticleEffect> ().InvalidateEffect();
		}
	}

	public void OnApplicationQuit ()
	{
		#if (!UNITY_EDITOR)
		if (bForkSDKShutdown == false) {
			Debug.Log("OnApplicationQuit ");
			ShutDownForkSDK ();
		}
		#endif
	}

    public void Test() {
        for (int i = 0; i < effectsList.Count; i++) {
            GameObject obj = (GameObject)effectsList [i];
            obj.GetComponent<ForkParticleEffect>().PlayEffect();
        }
    }
}
