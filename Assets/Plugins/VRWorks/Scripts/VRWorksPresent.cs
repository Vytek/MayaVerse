using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NVIDIA
{
  [ExecuteInEditMode]
  [RequireComponent(typeof(Camera))]
  public class VRWorksPresent : MonoBehaviour
  {
    #if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
      [DllImport("GfxPluginVRWorks64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    #else
      [DllImport("GfxPluginVRWorks32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    #endif
    private static extern IntPtr PluginExtGetIssueEventCallback();

    const int kPluginExtCommandUpscale = 5;

    [Tooltip("Flip final image vertically?")]
    public bool VerticalFlip = false; // change this as appropriate for your project    

    private NVIDIA.VRWorks m_VRWorks;

    public void Start()
    {
      Camera cam = GetComponent<Camera>();
      if (cam == null)
      {
        // We should be attached to a camera            
        Debug.LogError("VRWorksPresent not attached to camera");
      }
      else
      {
        m_VRWorks = cam.GetComponent<NVIDIA.VRWorks>();
      }
    }

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      NVIDIA.VRWorks.Feature feature = m_VRWorks.GetActiveFeature();
      if (feature == NVIDIA.VRWorks.Feature.LensMatchedShading || feature == NVIDIA.VRWorks.Feature.MultiResolution)
      {
        UnityEngine.Rendering.CommandBuffer buf = new UnityEngine.Rendering.CommandBuffer();
        buf.SetRenderTarget(destination);
        buf.IssuePluginCustomBlit(PluginExtGetIssueEventCallback(), kPluginExtCommandUpscale, source, destination, 0, VerticalFlip ? 1U : 0U);
        Graphics.ExecuteCommandBuffer(buf);
      }
      else
      {
        Graphics.Blit(source, destination);
      }
    }
  }
}
