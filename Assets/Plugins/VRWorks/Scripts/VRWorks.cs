using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Runtime.InteropServices;
#if UNITY_EDITOR_64 || UNITY_EDITOR_32
using UnityEditor;
using UnityEditor.Callbacks;
#endif

// VRWorks script used to enable MRS/LMS/SPS features on NVIDIA hardware
// Please contact UnitySupport@nvidia.com if you have any questions.

namespace NVIDIA
{
#if UNITY_EDITOR_64 || UNITY_EDITOR_32
  public class VRWorksBuildPostprocessor
  {
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
      string name = System.IO.Path.GetFileNameWithoutExtension(pathToBuiltProject);
      string path = System.IO.Path.GetDirectoryName(pathToBuiltProject);
      FileUtil.CopyFileOrDirectory("Library/nv_cache.asset", path + "/" + name + "_Data/" + "nv_cache.asset");
    }
  }
#endif

  [RequireComponent(typeof(Camera))]
  public class VRWorks : MonoBehaviour
  {
    // PUBLIC API    

    [Header("Multi-Resolution Shading")]
    [Range(0.0f, 0.4f)]
    [Tooltip("Horizontal split from the left as percentage of the viewport width (mirrored at the right)")]
    public float m_SplitX = 0.25f;
    [Range(0.0f, 0.4f)]
    [Tooltip("Vertical split from the top as percentage of the viewport height (mirrored at the bottom)")]
    public float m_SplitY = 0.25f;
    [Range(0.05f, 0.95f)]
    [Tooltip("Pixel density at the outskirts as a percentage (middle viewport always at 100%)")]
    public float m_PixelDensity = 0.7f;

    public enum Feature
    {
      // Default rendering
      None = 0,
      // MRS
      MultiResolution = 1,
      // SPS
      SinglePassStereo = 4,
      // LMS
      LensMatchedShading = 8,
    };

    public void Awake()
    {
      m_CommandBuffer = new UnityEngine.Rendering.CommandBuffer[(int)CB.kCB_Count];
      for (int i = 0; i < (int)CB.kCB_Count; i++)
      {
        m_CommandBuffer[i] = new UnityEngine.Rendering.CommandBuffer();        
      }
    }

    public void Start()
    {
      string features = "";
      uint renderFeatures = UnityGetRenderFeatureState();
      if ((renderFeatures & kPluginExtFeatureMRSAvailable) != 0)
      {
        // Maxwell+
        features += "MRS ";
      }
      if ((renderFeatures & kPluginExtFeatureSLIActive) != 0)
      {
        // SLI configuration
        features += "SLI ";
      }
      else  if ((renderFeatures & kPluginExtFeatureSPSAvailable) != 0)
      {
        // Pascal+ in non SLI configuration
        features += "SPS LMS";
      }      

      print("VRWorks: " + (features.Length > 0 ? features + " detected" : "Not supported or plugin was not initialized properly. If you just started a new project please restart Unity."));
      m_TargetSize = 0;
      m_RenderFeatures = 0;
      m_MRSParameters = new MRSParameters();      
      Camera cam = GetComponent<Camera>();
      if(cam == null)
      {
        // We should be attached to a camera            
        Debug.LogError("VRWorks not attached to camera");
      }
      else
      {
        AddCamera(cam);
      }      
    }

    public void OnApplicationQuit()
    {
      RenderTexture.ReleaseTemporary(m_TmpUpscaleDepthRT);
      RenderTexture.ReleaseTemporary(m_TmpUVRemapToLinearRT);
      RenderTexture.ReleaseTemporary(m_TmpUVRemapFromLinearRT);
      SetActiveFeature(Feature.None);
    }

    public void AddCamera(Camera cam)
    {
      if (!cam || cam == m_Camera)
      {
        return;
      }

      m_Camera = cam;

      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeDepthTexture, m_CommandBuffer[(int)CB.kCB_BeforeDepth]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeDepthNormalsTexture, m_CommandBuffer[(int)CB.kCB_BeforeDepth]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterDepthTexture, m_CommandBuffer[(int)CB.kCB_AfterDepth]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterDepthNormalsTexture, m_CommandBuffer[(int)CB.kCB_AfterDepthNormals]);

      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeForwardOpaque, m_CommandBuffer[(int)CB.kCB_BeforeForward]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeForwardAlpha, m_CommandBuffer[(int)CB.kCB_BeforeForward]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterForwardOpaque, m_CommandBuffer[(int)CB.kCB_AfterForward]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterForwardAlpha, m_CommandBuffer[(int)CB.kCB_AfterForward]);

      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeSkybox, m_CommandBuffer[(int)CB.kCB_BeforeSky]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterSkybox, m_CommandBuffer[(int)CB.kCB_AfterSky]);

      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeHaloAndLensFlares, m_CommandBuffer[(int)CB.kCB_BeforeSky]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterHaloAndLensFlares, m_CommandBuffer[(int)CB.kCB_AfterSky]);

      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeImageEffects, m_CommandBuffer[(int)CB.kCB_BeforeFX]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeImageEffectsOpaque, m_CommandBuffer[(int)CB.kCB_BeforeFX]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterImageEffects, m_CommandBuffer[(int)CB.kCB_AfterFX]);
      m_Camera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterImageEffectsOpaque, m_CommandBuffer[(int)CB.kCB_AfterFX]);     
    }

    public void OnPreRender()
    {
      uint targetSize = 0;
      if (m_Camera.stereoEnabled)
      {
        // target size needs to be twice for single pass stereo
        targetSize = (uint)((UnityEngine.VR.VRSettings.eyeTextureWidth + 0 )<< 17) | (uint)m_Camera.pixelHeight;
      }
      else
      {
        targetSize = (uint)(m_Camera.pixelWidth << 16) | (uint)m_Camera.pixelHeight;
      }
      uint renderFeatures = UnityGetRenderFeatureState();
      if (m_TargetSize != targetSize || m_RenderFeatures != renderFeatures)
      {
        m_TargetSize = targetSize;
        m_RenderFeatures = renderFeatures;
        UpdateCommandBuffers();
      }
      
      if((renderFeatures & kPluginExtFeatureMRSActive) != 0)
      {
        if(!m_Camera.stereoEnabled)
        {
          // Allow UV clamping in desktop MRS to avoid artifacts along the edges in image effects.
          // In VR these potential artifacts affect only peripheral vision so they are unlikely to be noticed.
          FeatureData data = GetActiveFeatureData();
          Shader.SetGlobalVector("_PluginUVClamp", new Vector4(data.mrsLeft.MultiResUVClamp[0], data.mrsLeft.MultiResUVClamp[1], data.mrsLeft.MultiResUVClamp[2], data.mrsLeft.MultiResUVClamp[3]));
        }

        if(!NearlyEqual(m_MRSParameters.splitX,m_SplitX,0.01f) ||
           !NearlyEqual(m_MRSParameters.splitY,m_SplitY,0.01f) ||
           !NearlyEqual(m_MRSParameters.pixelDensity,m_PixelDensity,0.01f))
        {
          GL.Flush();
          m_MRSParameters.splitX = m_SplitX;
          m_MRSParameters.splitY = m_SplitY;
          m_MRSParameters.pixelDensity = m_PixelDensity;
          UnitySetMRSParameters(ref m_MRSParameters);
        }        
      }
      else
      {
        Shader.SetGlobalVector("_PluginUVClamp", new Vector4(0,0,1,1));    
      }

      if ((renderFeatures & kPluginExtFeatureSPSActive) != 0 || (renderFeatures & kPluginExtFeatureLMSActive) != 0)
      {
        // Right eye matrix
        Matrix4x4 clipRightX = m_Camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right) * m_Camera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
        Shader.SetGlobalMatrix("_WorldToClipXRight", clipRightX);
      }

      Graphics.ExecuteCommandBuffer(m_CommandBuffer[(int)CB.kCB_BeforeEverything]);
    }

    public bool IsFeatureAvailable(Feature value)
    {
      return IsFeatureAvailableInternal(value);
    }

    public Feature GetActiveFeature()
    {
      return GetActiveFeatureInternal();      
    }    

    public bool SetActiveFeature(Feature value)
    {
        if (!IsFeatureAvailableInternal(value))
        {
            Debug.LogErrorFormat("VRWorks feature " + GetFeatureName(value) + " is not supported on this hardware");
            return false;
        }
        // These are _all_ currently implemented and valid feature combinations.
        // Trying to modify these flags will result in unpredictable behavior.
        uint renderFeatures = 0;
        if (value == Feature.MultiResolution)
        {            
            renderFeatures |= kPluginExtFeatureMRSCons;
        }        
        else if (value == Feature.SinglePassStereo)
        {
            renderFeatures |= kPluginExtFeatureSPSActive;
        }
        else if (value == Feature.LensMatchedShading)
        {
            renderFeatures |= kPluginExtFeatureSPSActive | kPluginExtFeatureLMSActive;
        }        
        UnitySetRenderFeatureState(renderFeatures);

        UnityEngine.Rendering.CommandBuffer buf = new UnityEngine.Rendering.CommandBuffer();
        InsertMarker(buf, PluginExtMarker.kPluginExtMarkerSetRenderFeatures, renderFeatures);            
        Graphics.ExecuteCommandBuffer(buf);
        GL.Flush();
        return true;
    }


    private static void InsertMarker(UnityEngine.Rendering.CommandBuffer buf, PluginExtMarker marker, UInt32 data)
    {
      buf.IssuePluginEventAndData(PluginExtGetIssueMarkerCallback(), (int)marker, (IntPtr)data);
    }

    private static void InsertCommand(UnityEngine.Rendering.CommandBuffer buf, PluginExtCommands command, UnityEngine.Rendering.RenderTargetIdentifier source, UnityEngine.Rendering.RenderTargetIdentifier dest, UInt32 commandParam, UInt32 commandFlags)
    {
      buf.IssuePluginCustomBlit(PluginExtGetIssueEventCallback(), (UInt32)command, source, dest, commandParam, commandFlags);
    }

    public void StartDisableFeature(Feature value)
    {
      UnityEngine.Rendering.CommandBuffer buf = new UnityEngine.Rendering.CommandBuffer();
      uint renderFeatures = UnityGetRenderFeatureState();
      if (value == Feature.MultiResolution)
      {
        renderFeatures &= ~kPluginExtFeatureMRSCons;
      }
      else if (value == Feature.SinglePassStereo)
      {
        renderFeatures &= ~kPluginExtFeatureSPSActive;
      }
      else if (value == Feature.LensMatchedShading)
      {
        renderFeatures &= ~kPluginExtFeatureLMSActive;
      }
      InsertMarker(buf, PluginExtMarker.kPluginExtMarkerSetRenderFeatures, renderFeatures);
      Graphics.ExecuteCommandBuffer(buf);
    }

    public void EndDisableFeature()
    {
      UnityEngine.Rendering.CommandBuffer buf = new UnityEngine.Rendering.CommandBuffer();
      uint renderFeatures = UnityGetRenderFeatureState();
      InsertMarker(buf, PluginExtMarker.kPluginExtMarkerSetRenderFeatures, renderFeatures);
      Graphics.ExecuteCommandBuffer(buf);
    }

    public static void SetKeywords(Material mat)
    {
      if (Application.isPlaying)
      {
        NVIDIA.VRWorks.Feature feature = GetActiveFeatureInternal();
        if (feature == NVIDIA.VRWorks.Feature.MultiResolution)
        {
          mat.EnableKeyword("VRWORKS_MRS");
          mat.DisableKeyword("VRWORKS_LMS");
          mat.DisableKeyword("VRWORKS_NONE");
        }
        else if (feature == NVIDIA.VRWorks.Feature.LensMatchedShading)
        {
          mat.EnableKeyword("VRWORKS_LMS");
          mat.DisableKeyword("VRWORKS_MRS");
          mat.DisableKeyword("VRWORKS_NONE");
        }
        else
        {
          mat.EnableKeyword("VRWORKS_NONE");
          mat.DisableKeyword("VRWORKS_MRS");
          mat.DisableKeyword("VRWORKS_LMS");
        }
      }
      else
      {
        mat.DisableKeyword("VRWORKS_MRS");
        mat.DisableKeyword("VRWORKS_LMS");
        mat.EnableKeyword("VRWORKS_NONE");
      }
    }

    private void UpdateCommandBuffers()
    {
      {
        // In double wide mode pixel width reports only half the actual size
        int pixelWidth = m_Camera.stereoEnabled ? (m_Camera.pixelWidth) * 2 : m_Camera.pixelWidth;
        
        m_TmpUpscaleDepthRT = RenderTexture.GetTemporary(pixelWidth, m_Camera.pixelHeight, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Default);
        m_TmpUpscaleDepthRT.filterMode = FilterMode.Point;
        m_TmpUpscaleDepthRT.Create();

        m_TmpUVRemapFromLinearRT = RenderTexture.GetTemporary(pixelWidth, m_Camera.pixelHeight, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Default);
        m_TmpUVRemapFromLinearRT.filterMode = FilterMode.Point;
        m_TmpUVRemapFromLinearRT.Create();

        m_TmpUVRemapToLinearRT = RenderTexture.GetTemporary(pixelWidth, m_Camera.pixelHeight, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Default);
        m_TmpUVRemapToLinearRT.filterMode = FilterMode.Point;
        m_TmpUVRemapToLinearRT.Create();

        // Update command buffers        
        for (int i = 0; i < (int)CB.kCB_Count; i++)
        {
          m_CommandBuffer[i].Clear();
          m_CommandBuffer[i].name = "VRWorks";
          if ((m_RenderFeatures & kPluginExtFeatureSPSActive) != 0)
          {
            m_CommandBuffer[i].name += " SPS";
          }
          if ((m_RenderFeatures & kPluginExtFeatureLMSActive) != 0)
          {
            m_CommandBuffer[i].name += " LMS";
          }
          if ((m_RenderFeatures & kPluginExtFeatureMRSActive) != 0)
          {
            m_CommandBuffer[i].name += " MRS";
          }
          //+ ((m_RenderFeatures & kPluginExtFeatureMRSActive) != 0) + " LMS" + ((m_RenderFeatures & kPluginExtFeatureLMSActive) != 0));
        }

        print("UpdateCommandBuffer " + m_CommandBuffer[0].name);

        string pluginKeywords = "PLUGIN_STEREOX";

        // Before everything
        uint textureFormat = 1; // kUnityVREyeTextureLayoutSeparate = 1 << 0
        uint vrState = UnityEngine.VR.VRSettings.enabled ? (uint)PluginExtVRState.kPluginExtVRStateActive : 0;
        if (m_Camera.stereoEnabled)
        {
          textureFormat = 1 << 1;// kUnityVREyeTextureLayoutDoubleWide = 1 << 1,                        
          vrState |= (uint)PluginExtVRState.kPluginExtVRStateUsed;
        }

        // Before everything
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeEverything], PluginExtMarker.kPluginExtMarkerEyeTextureLayout, textureFormat);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeEverything], PluginExtMarker.kPluginExtMarkerSetEyeTargetSize, m_TargetSize);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeEverything], PluginExtMarker.kPluginExtMarkerSetVRState, vrState);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeEverything], PluginExtMarker.kPluginExtMarkerSetRenderFeatures, m_RenderFeatures);
        // Remove this if we start remapping UV in image effects by computing remap instead of fetching UV from texture
        InsertCommand(m_CommandBuffer[(int)CB.kCB_BeforeEverything], PluginExtCommands.kPluginExtCommandMapUVFromLinear, BuiltinRenderTextureType.None, m_TmpUVRemapFromLinearRT, 0, 0);
        InsertCommand(m_CommandBuffer[(int)CB.kCB_BeforeEverything], PluginExtCommands.kPluginExtCommandMapUVToLinear, BuiltinRenderTextureType.None, m_TmpUVRemapToLinearRT, 0, 0);
        m_CommandBuffer[(int)CB.kCB_BeforeEverything].SetGlobalTexture("_PluginUVRemapFromLinearTexture", m_TmpUVRemapFromLinearRT);
        m_CommandBuffer[(int)CB.kCB_BeforeEverything].SetGlobalTexture("_PluginUVRemapToLinearTexture", m_TmpUVRemapToLinearRT);

        // Depth
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeDepth], PluginExtMarker.kPluginExtMarkerSetRenderFeatures, m_RenderFeatures & ~kPluginExtFeatureLMSActive);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeDepth], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageBeginCustomSection);
        if ((m_RenderFeatures & kPluginExtFeatureSPSActive) != 0)
        {
          m_CommandBuffer[(int)CB.kCB_BeforeDepth].EnableShaderKeyword(pluginKeywords);
          m_CommandBuffer[(int)CB.kCB_AfterDepth].DisableShaderKeyword(pluginKeywords);
          m_CommandBuffer[(int)CB.kCB_AfterDepthNormals].DisableShaderKeyword(pluginKeywords);
        }
        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterDepthNormals], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageEndCustomSection);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterDepthNormals], PluginExtMarker.kPluginExtMarkerSetRenderFeatures, m_RenderFeatures);

        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterDepth], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageEndCustomSection);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterDepth], PluginExtMarker.kPluginExtMarkerSetRenderFeatures, m_RenderFeatures);
        if ((m_RenderFeatures & kPluginExtFeatureMRSActive) != 0)
        {
          InsertCommand(m_CommandBuffer[(int)CB.kCB_AfterDepth], PluginExtCommands.kPluginExtCommandGrabScreen, BuiltinRenderTextureType.Depth, m_TmpUpscaleDepthRT, 0, 0);
          m_CommandBuffer[(int)CB.kCB_AfterDepth].SetGlobalTexture("_CameraDepthTexture", m_TmpUpscaleDepthRT);
          m_CommandBuffer[(int)CB.kCB_AfterDepth].SetGlobalTexture("_PluginDepthTexture", BuiltinRenderTextureType.Depth);
        }

        // Forward
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeForward], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageBeginCustomSection);
        if ((m_RenderFeatures & kPluginExtFeatureSPSActive) != 0)
        {
          m_CommandBuffer[(int)CB.kCB_BeforeForward].EnableShaderKeyword(pluginKeywords);
          m_CommandBuffer[(int)CB.kCB_AfterForward].DisableShaderKeyword(pluginKeywords);
        }
        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterForward], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageEndCustomSection);

        // Sky
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeSky], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageBeginCustomSection);
        if ((m_RenderFeatures & kPluginExtFeatureSPSActive) != 0)
        {
          m_CommandBuffer[(int)CB.kCB_BeforeSky].EnableShaderKeyword(pluginKeywords);
          m_CommandBuffer[(int)CB.kCB_AfterSky].DisableShaderKeyword(pluginKeywords);
        }
        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterSky], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageEndCustomSection);

        // FX
        InsertMarker(m_CommandBuffer[(int)CB.kCB_BeforeFX], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageBeginPostProcess);
        InsertMarker(m_CommandBuffer[(int)CB.kCB_AfterFX], PluginExtMarker.kPluginExtMarkerSetRenderStage, (int)PluginExtRenderStage.kPluginExtRenderStageEndPostProcess);

      }
    }

    public FeatureData GetActiveFeatureData()
    {
      UnityGetFeatureData(ref m_FeatureData);
      return m_FeatureData;
    }

    public void SetInternalFlags(uint flags)
    {
      UnitySetInternalFlags(flags);
    }

    
    [StructLayout(LayoutKind.Sequential)]
    public struct MRSParameters
    {      
      public float splitX;
      public float splitY;
      public float pixelDensity;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MRSData
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResSplitsX;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResSplitsY;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResX0;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResX1;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResX2;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResY0;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResY1;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] LinearToMultiResY2;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public float[] MultiResUVScaleBias;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public float[] MultiResUVClamp;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public float[] MultiResScaleOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FeatureData
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public float[] boundingRect;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] splits;
      public MRSData mrsLeft;
      public MRSData mrsRight;
    };

    private enum CB
    {
      kCB_BeforeEverything,
      kCB_BeforeDepth,
      kCB_BeforeForward,
      kCB_BeforeSky,
      kCB_BeforeFX,
      kCB_AfterDepth,
      kCB_AfterDepthNormals,
      kCB_AfterForward,
      kCB_AfterSky,
      kCB_AfterFX,
      kCB_Count
    }
    private UnityEngine.Rendering.CommandBuffer[] m_CommandBuffer;
    private uint m_RenderFeatures = 0;
    private uint m_TargetSize = 0;
    private Camera m_Camera;
    private RenderTexture m_TmpUpscaleDepthRT;
    private RenderTexture m_TmpUVRemapToLinearRT;
    private RenderTexture m_TmpUVRemapFromLinearRT;

#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
    [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern void UnityGetFeatureData(ref FeatureData data);

#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
    [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern void UnitySetMRSParameters(ref MRSParameters mrsParams);

#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
    [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern UInt32 UnityRenderingExtGetEventIDOffset();


#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
    [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern IntPtr PluginExtGetIssueMarkerCallback();


#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
    [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern IntPtr PluginExtGetIssueEventCallback();


#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
    [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern void UnitySetRenderFeatureState(uint features);

#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
  [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern uint UnityGetRenderFeatureState();

#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
  [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
    private static extern void UnitySetInternalFlags(uint flags);

    static uint kPluginExtFeatureNone = 0;
    static uint kPluginExtFeatureMRSAvailable = 1 << 0;
    static uint kPluginExtFeatureSPSAvailable = 1 << 1;
    static uint kPluginExtFeatureMRSCons = 1 << 2;
    static uint kPluginExtFeatureMRSAggr = 1 << 3;
    static uint kPluginExtFeatureSPSActive = 1 << 4;
    static uint kPluginExtFeatureLMSActive = 1 << 5; // always available if SPS is available
    static uint kPluginExtFeatureSLIActive = 1 << 6;
    static uint kPluginExtFeatureMRSActive = kPluginExtFeatureMRSCons | kPluginExtFeatureMRSAggr;
    static uint kPluginExtFeatureShaderVariantActive = kPluginExtFeatureSPSActive;
    static uint kPluginExtFeatureAllActive = kPluginExtFeatureSPSActive | kPluginExtFeatureMRSActive | kPluginExtFeatureLMSActive;

    public enum PluginExtRenderStage
    {
      kPluginExtRenderStageNone = 0,
      kPluginExtRenderStageBeginCustomSection = 1 << 0,
      kPluginExtRenderStageEndCustomSection = 1 << 1,
      kPluginExtRenderStageBeginPostProcess = 1 << 4,
      kPluginExtRenderStageEndPostProcess = 1 << 5,
    };

    public enum PluginExtVRState
    {
      kPluginExtVRStateActive = 1 << 0, // VR device is present and active
      kPluginExtVRStateUsed = 1 << 1,   // Current rendering is in VR (stereo) mode
    };

    public enum PluginExtMarker
    {
      kPluginExtMarkerSetEyeTargetSize,
      kPluginExtMarkerSetRenderStage,
      kPluginExtMarkerEyeTextureLayout,
      kPluginExtMarkerDrawOcclusionMesh,
      kPluginExtMarkerSetRenderFeatures,
      kPluginExtMarkerSetVRState,
    };

    public enum PluginExtCommands
    {
      kPluginExtCommandFlush,
      kPluginExtCommandDownscale,
      kPluginExtCommandUpscale,
      kPluginExtCommandMapUVToLinear,
      kPluginExtCommandMapUVFromLinear,      
      kPluginExtCommandGrabScreen
    };

    private static System.String GetFeatureName(Feature value)
    {
      switch (value)
      {        
        case Feature.MultiResolution: return "Multi-Resolution Shading";
        case Feature.SinglePassStereo: return "Single Pass Stereo";
        case Feature.LensMatchedShading: return "Lens Matched Shading";
      }
      return "None";
    }

    private static bool IsFeatureAvailableInternal(Feature value)
    {
      uint renderFeatures = UnityGetRenderFeatureState();
      if (value == Feature.MultiResolution)
      {
        return (renderFeatures & kPluginExtFeatureMRSAvailable) != 0;
      }
      else if (value == Feature.SinglePassStereo || value == Feature.LensMatchedShading)
      {
        // SPS/LMS available only in non SLI configuration
        return (renderFeatures & kPluginExtFeatureSPSAvailable) != 0 &&
          (renderFeatures & kPluginExtFeatureSLIActive) == 0;
      }
      return true;
    }

    private static Feature GetActiveFeatureInternal()
    {
      uint renderFeatures = UnityGetRenderFeatureState();
      if ((renderFeatures & kPluginExtFeatureMRSCons) != 0)
      {
        return Feature.MultiResolution;
      }
      else if ((renderFeatures & kPluginExtFeatureLMSActive) != 0)
      {
        return Feature.LensMatchedShading;
      }
      else if ((renderFeatures & kPluginExtFeatureSPSActive) != 0)
      {
        return Feature.SinglePassStereo;
      }
      return Feature.None;
    }

    private static bool NearlyEqual(float a, float b, float epsilon)
    {
      float absA = Math.Abs(a);
      float absB = Math.Abs(b);
      float diff = Math.Abs(a - b);

      if (a == b)
      { 
        return true;
      }
      else if (a == 0 || b == 0 || diff < Single.Epsilon)
      {
        return diff < (epsilon * Single.Epsilon);
      }
      else
      { 
        return diff / (absA + absB) < epsilon;
      }
    }    

    private static FeatureData m_FeatureData;
    private MRSParameters m_MRSParameters;
  };

} // namespace NVIDIA