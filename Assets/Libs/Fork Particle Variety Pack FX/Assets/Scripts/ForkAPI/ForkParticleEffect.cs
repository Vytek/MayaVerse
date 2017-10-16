/******************************************************************************
 Disclaimer Notice:
 This file is provided as is with no warranties of any kind and is
 provided without any obligation on Fork Particle, Inc. to assist in 
 its use or modification. Fork Particle, Inc. will not, under any
 circumstances, be liable for any lost revenue or other damages arising 
 from the use of this file.
 
 (c) Copyright 2017 Fork Particle, Inc. All rights reserved.
******************************************************************************/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ForkParticleEffect : MonoBehaviour
{	
	private delegate void frkSetTextureNameCB(string sTextureName);

    private bool bValidEffect = false, bEffectLoaded = false;
	private System.IntPtr pFrkEffect;

	public PSBAsset PSBFile;

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
    private static extern System.IntPtr _frkParticlePluginCreateEffect(byte[] data, byte[] sName, int nSize);

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
	private static extern void _frkParticlePluginGetEffectTexturePointer (int nTextureID);

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
	private static extern void _frkParticlePluginSetEffectMatrix (System.IntPtr pEffect, Matrix4x4 effectMatTR);

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
	private static extern void _frkParticlePluginSetEffectColor (System.IntPtr pEffect, float r, float g, float b, float a);

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
	private static extern void _frkParticlePluginSetEffectScale(System.IntPtr pEffect, float fUniformScale);

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
	private static extern void _frkParticlePluginClearEffect (System.IntPtr pEffect);

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
	private static extern int _frkParticlePluginEffectIsAlive (System.IntPtr pEffect);

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
	private static extern void _frkParticlePluginEffectFreeze (System.IntPtr pEffect);

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
	private static extern void _frkParticlePluginPlayEffect (System.IntPtr pEffect);

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
	private static extern void RegisterSetTextureNameCB(frkSetTextureNameCB callback);

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
    private static extern void _frkParticlePluginSetEffectEnable(bool bState, System.IntPtr pEffect);

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
	private static extern int _frkParticlePluginGetEffectRenderParamsNumIndices(System.IntPtr pEffect, int nEmitterIdx);

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
	private static extern int _frkParticlePluginGetEffectRenderParamsNumVertices(System.IntPtr pEffect, int nEmitterIdx);

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
    private static extern void _frkParticlePluginGetEffectRenderParams(System.IntPtr pEffect, int nEmitterIdx, int[] triangles, int[] nTextureID, int nIndexOffset, int[] nBlendMode);

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
	private static extern int _frkParticlePluginGetEffectNumEmitters(System.IntPtr pEffect);


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
	private static extern int _frkParticlePluginEffectGetGeometry(System.IntPtr pEffect, Vector3[] Vertices, Vector2[] UV, Color32[] mColor);

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
    private static extern void _frkParticlePluginEffectRestart(System.IntPtr pEffect);

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
    private static extern int _frkParticlePluginEffectPrepareGeometry(System.IntPtr pEffect);
    		
    //******************************************************************************************************//

    const int frkPSYSTEM_EMITTERBLENDMODE_NORMAL = 0;
    const int frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE = 1;
    const int frkPSYSTEM_EMITTERBLENDMODE_SUBTRACTIVE = 2;
    const int frkPSYSTEM_EMITTERBLENDMODE_DARKMAP = 3;
    const int frkPSYSTEM_EMITTERBLENDMODE_LIGHTMAP = 4;
    const int frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE2 = 5;
    const int frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE3 = 6;
    const int frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE4 = 7;

    const int NUMSUBMESHES = 16;

	Vector3[] Vertices = new Vector3[1];
    Vector2[] UV = new Vector2[1];
    Color32[] mColor = new Color32[1];

    Vector3 translation;
    Vector3 scale = new Vector3 (1, 1, 1);
    Quaternion rotation;

    int[] newTriangles;
    int[] nTex = new int[1];
    int[] nBlendModeID = new int[1];
    int nIndices = 0;
    int nEmitters = 0;

    Mesh mesh = null;
	Material mat;
    Renderer rend;
    Shader sh = null;

    static Texture2D[] g_tex = new Texture2D[32];
	static int g_ntex = 0;

	private static void GetTextureCB(string message)
	{
		PassTexturePointerToFork (message);
	}

    void Start()
	{
        // Wait for the Fork SDK to be initialized
        if (ForkParticlePlugin.Instance.bForkSDKInit == true)
            ForkEffectCreate();

        translation = transform.position;
        rotation = transform.rotation;

		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
        rend = GetComponent<Renderer>();
        mesh.subMeshCount = NUMSUBMESHES;

        mesh.MarkDynamic();
 
        sh = Shader.Find("Unlit/ForkParticle");
        mat = new Material(sh);
        rend.material = mat;
	}

    void OnDisable ()
    {
        if (!bValidEffect)
            return;

        if (pFrkEffect != IntPtr.Zero)
        {
            _frkParticlePluginClearEffect(pFrkEffect);
            _frkParticlePluginSetEffectEnable(false, pFrkEffect);
            mesh.Clear();
            mesh.subMeshCount = 0;
        }
    }

    void OnEnable()
    {
        if (pFrkEffect != IntPtr.Zero)
        {
            _frkParticlePluginClearEffect(pFrkEffect);
        }
    }

	void Update() {
        // Wait for the Fork SDK to be initialized
        if (ForkParticlePlugin.Instance.bForkSDKInit == false)
            return;

        if (bEffectLoaded == false)
        {
            ForkEffectCreate();
        }
        
        if (bEffectLoaded == false || bValidEffect == false || pFrkEffect == IntPtr.Zero)
			return;
           
        translation = transform.position;
        scale = new Vector3 (1, 1, 1);
        rotation = transform.rotation;

        Matrix4x4 matTR = Matrix4x4.TRS (translation, rotation, scale);

        // Transform and Rotation Matrix
        // Fork API does not support the TRS in a single
        // matrix, due to which a seperate API is used
        // to update the scale of the effects
        _frkParticlePluginSetEffectMatrix (pFrkEffect, matTR);

        // Fork uses uniform scale, the value transform.lossyScale.x
        // will be applied to x, y and z components of the effects
        _frkParticlePluginSetEffectScale(pFrkEffect, transform.localScale.x);

        mesh.Clear();
        mesh.subMeshCount = 0;
        
        int nNumVertices = _frkParticlePluginEffectPrepareGeometry(pFrkEffect);
        if (nNumVertices == 0)
        {
            return;
        }
        
        Vertices = new Vector3[nNumVertices];
        UV = new Vector2[nNumVertices];
        mColor = new Color32[nNumVertices];
        
        nEmitters = _frkParticlePluginEffectGetGeometry(pFrkEffect, Vertices, UV, mColor);
        
        if (nEmitters == 0)
        {
            mesh.Clear();
            mesh.subMeshCount = 0;
            return;
        }

        mesh.Clear();
        mesh.subMeshCount = nEmitters;
		mesh.vertices = Vertices;
		mesh.uv = UV;
		mesh.colors32 = mColor;
        	
        Material[] meshMat = new Material[nEmitters];
        for (int i = 0; i < nEmitters; i++)
        {
            meshMat[i] = mat;
        }
        rend.materials = meshMat;

        int nIndexOffset = 0;
        
		for (int i = 0; i < nEmitters; i++) {
			nIndices = _frkParticlePluginGetEffectRenderParamsNumIndices (pFrkEffect, i);
            if (nIndices == 0)
                continue;

            newTriangles = new int[nIndices];
			_frkParticlePluginGetEffectRenderParams(pFrkEffect, i, newTriangles, nTex, nIndexOffset, nBlendModeID);
            
            mesh.SetTriangles(newTriangles, i, false);

			Texture mainTex = rend.materials [i].GetTexture ("_MainTex");
			if (mainTex != g_tex [nTex [0]])
				rend.materials[i].SetTexture ("_MainTex", g_tex [nTex [0]]);

            SetBlendMode(nBlendModeID[0], i);

            nIndexOffset += _frkParticlePluginGetEffectRenderParamsNumVertices(pFrkEffect, i);
            
    	}

        mesh.bounds = new Bounds(Vector3.zero, Vector3.zero);
	}

    private void ForkEffectCreate() {

        PSBFile.Load(PSBFile.sPSBPath);
        RegisterSetTextureNameCB(new frkSetTextureNameCB(GetTextureCB));
		
		// Fork SDK has been initialized, we can create our effect now
		if (PSBFile != null) {
			// Effect must pass the PSB data to dll for the effect to be created
            byte[] sName = System.Text.Encoding.UTF8.GetBytes(PSBFile.sPSBName);

            pFrkEffect = _frkParticlePluginCreateEffect(PSBFile.Data, sName, PSBFile.nDataSize);
            if (pFrkEffect == IntPtr.Zero) {
                Debug.LogError("Fork Particle: Failed to create effect.");
            }
            else
            {
                ForkParticlePlugin.Instance.AddEffect(this.gameObject);
                bEffectLoaded = true;
                bValidEffect = true;
                _frkParticlePluginSetEffectEnable(false, pFrkEffect);
            }
		}
	}

	private static void PassTexturePointerToFork (string sName) {
		// Load the texture from unity Resource folder
		// and pass its pointer to the dll, so that
		// it can be attached to the effect

        sName = sName.Remove(sName.IndexOf('.'));
        int nTextureID = CheckLoadedTexture(sName);

        if (nTextureID != -1)
        {
            _frkParticlePluginGetEffectTexturePointer(nTextureID);
            return;
        }

        sName = ForkParticlePlugin.Instance.TexturePath + sName;
        Texture2D tex = Resources.Load(sName, typeof(Texture2D)) as Texture2D;

		if (tex == null) {
            Debug.Log("Fork Particle: Failed to load texture");
            tex = Resources.Load("null", typeof(Texture2D)) as Texture2D;

            if (tex == null)
                return;
		}

        tex.filterMode = FilterMode.Point;
		g_tex [g_ntex] = tex;
		
		_frkParticlePluginGetEffectTexturePointer (g_ntex);
		g_ntex++;
	}

    static int CheckLoadedTexture (string sTextureName) {
        int nTexID = -1;
        
        for (int i = 0; i < g_ntex; i++)
        {
            if (g_tex[i].name == sTextureName)
            {
                nTexID = i;
                break;
            }
        }

        return nTexID;
    }

    public static void DestroyTextures () {
        for (int i = 0; i < g_ntex; i++)
        {
            g_tex[i] = null;
        }

        g_ntex = 0;
    }

	void OnDestroy()
	{
		ForkParticlePlugin.Instance.RemoveEffect (this.gameObject);
		gameObject.SetActive (false);
		if (bValidEffect)
			_frkParticlePluginDestroyEffect (pFrkEffect);
	}

	public void InvalidateEffect()
	{
		bValidEffect = false;
	}

    void SetBlendMode (int nBlendMode, int nMatID)
    {
        int i = nMatID;

        switch (nBlendMode)
        {
            case frkPSYSTEM_EMITTERBLENDMODE_NORMAL:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_SUBTRACTIVE:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_DARKMAP:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_LIGHTMAP:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE2:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE3:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;

            case frkPSYSTEM_EMITTERBLENDMODE_EMISSIVE4:
                rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;
        }
    }

    public void EnableEffect(bool bState)
    {
        if (pFrkEffect != IntPtr.Zero)
            _frkParticlePluginSetEffectEnable(bState, pFrkEffect);
    }

    /**********************************************************/
    // Kills all the particles instantly
    /*********************************************************/
	public void ClearForkEffect () 
	{
        if (pFrkEffect != IntPtr.Zero)
    		_frkParticlePluginClearEffect (pFrkEffect);
	}

    /**********************************************************/
    // Returns 1 if the effect is still playing, 0 otherwise
    /*********************************************************/
	public int IsEffectAlive ()
	{
        int nIsAlive = -1;
        if (pFrkEffect != IntPtr.Zero)
		    nIsAlive = _frkParticlePluginEffectIsAlive (pFrkEffect);
		
        return nIsAlive;
	}

    /********************************************/
    // Freezes the effect in its current state
    /********************************************/
	public void EffectFreeze ()
	{
        if (pFrkEffect != IntPtr.Zero)
    		_frkParticlePluginEffectFreeze(pFrkEffect);
	}

    /********************************************/
    // Play the effect, Object must be enabled
    /********************************************/
	public void PlayEffect ()
	{
        if (gameObject.activeSelf == false)
            return;

        if (pFrkEffect != IntPtr.Zero)
        {
            _frkParticlePluginSetEffectEnable(true, pFrkEffect);
            _frkParticlePluginPlayEffect(pFrkEffect);
        }
	}

    /*******************************************************/
    // Change the color of the effect in R, G, B, A format
    /*******************************************************/
    public void SetEffectColor (float fRed, float fGreen, float fBlue, float fAlpha)
    {
        if (pFrkEffect != IntPtr.Zero)
            _frkParticlePluginSetEffectColor(pFrkEffect, fRed, fGreen, fBlue, fAlpha);
    }

    /**********************************************************/
    // Kills all the particles instantly and plays the effect
    /*********************************************************/
    public void RestartEffect()
    {
        if (pFrkEffect != IntPtr.Zero)
            _frkParticlePluginEffectRestart(pFrkEffect);
    }
}