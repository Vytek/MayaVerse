
// # This script _must_ be attached to the main camera which renders 3D scene
// # Default coordinate system is left handed. If your project is using different coordinate system please configuration data in Start method accordingly
// # Use property Ansel.IsAvailable to adjust UI and other items which allow user to interact with Ansel
// # Use property Ansel.IsSessionActive to adjust game logic (game should be paused and camera parameters (position, orientation, FOV, view/projection matrices etc) _must_ not be changed elsewhere in script)
// # Use property Ansel.IsCaptureActive to disable effects (e.g. motion blur) which can cause Ansel not to work correctly during capture
// # Use Ansel.ConfigureSession to enable/disable Ansel sessions and features (only when session is not active)
// # Key parameters are exposed as properties so they can be edited directly in the editor. Other parameters should be changed only in rare scenarios.

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using System.Runtime.InteropServices;

namespace NVIDIA
{
  public class Ansel : MonoBehaviour
  {
    [StructLayout(LayoutKind.Sequential)]
    public struct ConfigData
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] forward;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] up;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] right;

      // The speed at which camera moves in the world
      public float translationalSpeedInWorldUnitsPerSecond;
      // The speed at which camera rotates 
      public float rotationalSpeedInDegreesPerSecond;
      // How many frames it takes for camera update to be reflected in a rendered frame
      public uint captureLatency;
      // How many frames we must wait for a new frame to settle - i.e. temporal AA and similar
      // effects to stabilize after the camera has been adjusted
      public uint captureSettleLatency;
      // Game scale, the size of a world unit measured in meters
      public float metersInWorldUnit;
      // Integration will support Camera::screenOriginXOffset/screenOriginYOffset
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraOffcenteredProjectionSupported;
      // Integration will support Camera::position
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraTranslationSupported;
      // Integration will support Camera::rotation
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraRotationSupported;
      // Integration will support Camera::horizontalFov
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraFovSupported;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraData
    {
      public float fov; // degrees
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] projectionOffset;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] position;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public float[] rotation;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct SessionData
    {
      [MarshalAs(UnmanagedType.I1)]
      public bool isAnselAllowed; // if set to false none of the below parameters is relevant
      [MarshalAs(UnmanagedType.I1)]
      public bool is360MonoAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool is360StereoAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isFovChangeAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isHighresAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isPauseAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isRotationAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isTranslationAllowed;
    };

    // Buffer hints for Ansel
    public enum HintBufferType
    {
      kBufferTypeHDR = 0,
      kBufferTypeDepth,
      kBufferTypeHUDless,
      kBufferTypeCount
    };

    // User control status
    public enum UserControlStatus
    {
        kUserControlOk = 0,
        kUserControlIdAlreadyExists,
        kUserControlInvalidId,
        kUserControlInvalidType,
        kUserControlInvalidLabel,
        kUserControlNameTooLong,
        kUserControlInvalidValue,
        kUserControlInvalidLocale,
        kUserControlInvalidCallback
    };    

    // The speed at which camera moves in the world
    [SerializeField]
    public float TranslationalSpeedInWorldUnitsPerSecond = 5.0f;
    // The speed at which camera rotates 
    [SerializeField]
    public float RotationalSpeedInDegreesPerSecond = 45.0f;
    // How many frames it takes for camera update to be reflected in a rendered frame
    [SerializeField]
    public uint CaptureLatency = 0;
    // How many frames we must wait for a new frame to settle - i.e. temporal AA and similar
    // effects to stabilize after the camera has been adjusted
    [SerializeField]
    public uint CaptureSettleLatency = 10;
    // Game scale, the size of a world unit measured in meters
    [SerializeField]
    public float MetersInWorldUnit = 1.0f;

    public static bool IsSessionActive
    {
      get
      {
        return sessionActive;
      }
    }

    public static bool IsCaptureActive
    {
      get
      {
        return captureActive;
      }
    }

    public static bool IsAvailable
    {
      get
      {
        return anselIsAvailable();
      }
    }

    // --------------------------------------------------------------------------------
    private void Awake()
    {
      hintBufferPreBindCBs = new CommandBuffer[(int)HintBufferType.kBufferTypeCount];
      hintBufferPostBindCBs = new CommandBuffer[(int)HintBufferType.kBufferTypeCount];
      for (int i = 0; i < (int)HintBufferType.kBufferTypeCount; i++)
      {
        hintBufferPreBindCBs[i] = new CommandBuffer();
        hintBufferPreBindCBs[i].IssuePluginEvent(GetMarkBufferPreBindRenderEventFunc(), i);
        hintBufferPostBindCBs[i] = new CommandBuffer();
        hintBufferPostBindCBs[i].IssuePluginEvent(GetMarkBufferPostBindRenderEventFunc(), i);
      }
    }

    public void Start()
    {
      if (!IsAvailable)
      {
        Debug.LogError("Ansel is not available or enabled on this platform. Did you forget to whitelist your executable?");
        return;
      }

      // Get our camera (this script _must_ be attached to the main camera which renders the 3D scene)
      mainCam = GetComponent<UnityEngine.Camera>();

      // Hint example, call this right before setting HDR render target active. This will notify Ansel that HDR buffer is about to bind.
      // Use that if Ansel is incorrectly determining HDR buffer to be used. (by default it is not the case). 
      // This is only the example, consider trying different CameraEvents in the rendering pipeline.
      /*
      mainCam.AddCommandBuffer(CameraEvent.BeforeImageEffects, hintBufferPreBindCBs[(int)HintBufferType.kBufferTypeHDR]);
      */

      anselConfig = new ConfigData();

      // Default coordinate system is left handed.
      // If your project is using different coordinate system please adjust accordingly      
      anselConfig.right = new float[3] { 1, 0, 0 };
      anselConfig.up = new float[3] { 0, 1, 0 };
      anselConfig.forward = new float[3] { 0, 0, 1 };
      // Can be set by user from the editor
      anselConfig.translationalSpeedInWorldUnitsPerSecond = TranslationalSpeedInWorldUnitsPerSecond;
      anselConfig.rotationalSpeedInDegreesPerSecond = RotationalSpeedInDegreesPerSecond;
      anselConfig.captureLatency = CaptureLatency;
      anselConfig.captureSettleLatency = CaptureSettleLatency;
      anselConfig.metersInWorldUnit = MetersInWorldUnit;
      // These should always be true unless there is some special scenario
      anselConfig.isCameraOffcenteredProjectionSupported = true;
      anselConfig.isCameraRotationSupported = true;
      anselConfig.isCameraTranslationSupported = true;
      anselConfig.isCameraFovSupported = true;
      anselInit(ref anselConfig);

      // Ansel will return camera parameters here
      anselCam = new CameraData();

      // Default session configuration which allows everything.
      // Game can reconfigure session anytime session is not active by calling ConfigureSession.
      SessionData ses = new SessionData();
      ses.isAnselAllowed = true; // if false none of the below parameters is relevant
      ses.isFovChangeAllowed = true;
      ses.isHighresAllowed = true;
      ses.isPauseAllowed = true;
      ses.isRotationAllowed = true;
      ses.isTranslationAllowed = true;
      ses.is360StereoAllowed = true;
      ses.is360MonoAllowed = true;
      anselConfigureSession(ref ses);

      // Custom Control example
      // Add your own slider or boolean toggle into the Ansel UI. You can then poll values at any time, but
      // it makes sense to do it only when anselIsSessionOn() is true. It is a good way to add the control 
      // over game specific settings like for instance day/night switch or bloom intensity.
      /*
      {
        anselAddUserControlSlider(1, "My Custom Slider", 0.2f));
        float myValue = anselGetUserControlSliderValue(2)
      }
      */

      if (!IsAvailable)
      {
        Debug.LogError("Ansel failed to configure session. Please check Ansel log for more details.");        
      }
      else
      {
        print("Ansel is initialized and ready to use");
      }
    }

    // --------------------------------------------------------------------------------
    public void UpdateCoordinateSystem(Vector3 right, Vector3 up, Vector3 forward)
    {
      if (anselIsSessionOn())
      {
        Debug.LogError("Ansel coordinate system cannot be configured while session is active");
        return;
      }      
      anselConfig.right = new float[3] { right.x, right.y, right.z };
      anselConfig.up = new float[3] { up.x, up.y, up.z };
      anselConfig.forward = new float[3] { forward.x, forward.y, forward.z };
      anselUpdateConfiguration(ref anselConfig);
    }

    // --------------------------------------------------------------------------------
    public void ConfigureSession(SessionData ses)
    {
      if (!IsAvailable)
      {
        Debug.LogError("Ansel is not available or enabled on this platform. Did you forget to whitelist your executable?");
        return;
      }

      if (anselIsSessionOn())
      {
        Debug.LogError("Ansel session cannot be configured while session is active");
        return;
      }
      anselConfigureSession(ref ses);
    }

    // --------------------------------------------------------------------------------
    public void OnPreRender()
    {
      if (anselIsSessionOn())
      {
        // Ansel session is active (user pressed Alt+F2)
        if (!sessionActive)
        {
          sessionActive = true;
          // On first update after session is activated we need to store
          // camera and other parameters so they can be restored later on
          SaveState();
          print("Started Ansel session");
        }

        Animator anim = mainCam.GetComponent<Animator>();
        if (anim)
        {
          anim.enabled = false;
        }

        // Check if capture is active
        captureActive = anselIsCaptureOn();

        Transform trans = mainCam.transform;

        anselCam.fov = mainCam.fieldOfView;
        anselCam.projectionOffset = new float[2] { 0, 0 };
        anselCam.position = new float[3] { trans.position.x, trans.position.y, trans.position.z };
        anselCam.rotation = new float[4] { trans.rotation.x, trans.rotation.y, trans.rotation.z, trans.rotation.w };
        anselUpdateCamera(ref anselCam);

        // Reset projection matrix so that potential FOV changes below can take effect
        mainCam.ResetProjectionMatrix();
        mainCam.transform.position = new Vector3(anselCam.position[0], anselCam.position[1], anselCam.position[2]);
        mainCam.transform.rotation = new Quaternion(anselCam.rotation[0], anselCam.rotation[1], anselCam.rotation[2], anselCam.rotation[3]);
        mainCam.fieldOfView = anselCam.fov;
        if (anselCam.projectionOffset[0] != 0 || anselCam.projectionOffset[1] != 0)
        {
          // Hi-res screen shots require projection matrix adjustment
          projectionMatrix = mainCam.projectionMatrix;
          float l = -1.0f + anselCam.projectionOffset[0];
          float r = l + 2.0f;
          float b = -1.0f + anselCam.projectionOffset[1];
          float t = b + 2.0f;
          projectionMatrix[0, 2] = (l + r) / (r - l);
          projectionMatrix[1, 2] = (t + b) / (t - b);
          mainCam.projectionMatrix = projectionMatrix;
        }
      }
      else
      {
        // Ansel session is no longer active
        if (sessionActive)
        {
          sessionActive = false;
          captureActive = false;
          RestoreState();
          print("Stopped Ansel session");
          Animator anim = mainCam.GetComponent<Animator>();
          if (anim)
          {
            anim.enabled = true;
          }
        }
      }
    }

    // --------------------------------------------------------------------------------
    private void SaveState()
    {
      Transform trans = mainCam.transform;

      cameraPos = trans.position;
      cameraRotation = trans.rotation;
      cameraFOV = mainCam.fieldOfView;
      cursorVisible = Cursor.visible;

      // Stop time counting to effectively pause the game
      Time.timeScale = 0.0f;
      // Disable user input
      Input.ResetInputAxes();

      // TODO: Hide GUI/HUD and disable anything else that might cause bad user experience with Ansel
      //GameObject.Find("NGUICamera").GetComponent<Camera>().enabled = false;

      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    // --------------------------------------------------------------------------------
    private void RestoreState()
    {
      // Reset time scale back to 1.0f (or whatever value game is using as default)
      Time.timeScale = 1.0f;

      // Restore camera parameters to the original values        
      mainCam.ResetProjectionMatrix();
      mainCam.transform.position = cameraPos;
      mainCam.transform.rotation = cameraRotation;
      mainCam.fieldOfView = cameraFOV;
      Cursor.visible = cursorVisible;
      Cursor.lockState = CursorLockMode.None;

      // TODO: show any hidden GUI/HUD elements and re-enable all items which were disabled when session started
      //GameObject.Find("NGUICamera").GetComponent<Camera>().enabled = true;
    }

#if (UNITY_64 || UNITY_EDITOR_64 || PLATFORM_ARCH_64)
    const string PLUGIN_DLL = "AnselPlugin64";
#else
    const string PLUGIN_DLL = "AnselPlugin32";
#endif

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void anselInit(ref ConfigData conf);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void anselUpdateConfiguration(ref ConfigData conf);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void anselUpdateCamera(ref CameraData cam);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void anselConfigureSession(ref SessionData ses);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool anselIsSessionOn();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool anselIsCaptureOn();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool anselIsAvailable();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void anselStartSession();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void anselStopSession();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern System.IntPtr GetMarkBufferPreBindRenderEventFunc();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern System.IntPtr GetMarkBufferPostBindRenderEventFunc();

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern UserControlStatus anselAddUserControlSlider(uint userControlId, string labelUtf8, float value);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern UserControlStatus anselSetUserControlSliderValue(uint userControlId, float value);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern float anselGetUserControlSliderValue(uint userControlId);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern UserControlStatus anselAddUserControlBoolean(uint userControlId, string labelUtf8, bool value);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern UserControlStatus anselSetUserControlBooleanValue(uint userControlId, bool value);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool anselGetUserControlBooleanValue(uint userControlId);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern UserControlStatus anselSetUserControlLabelLocalization(uint userControlId, string lang, string labelUtf8);

    [DllImport(PLUGIN_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern UserControlStatus anselRemoveUserControl(uint userControlId);


    private static bool sessionActive = false;
    private static bool captureActive = false;

    private bool cursorVisible = false;
    private Vector3 cameraPos;
    private Quaternion cameraRotation;
    private float cameraFOV;

    private ConfigData anselConfig;
    private CameraData anselCam;
    private Matrix4x4 projectionMatrix;
    private Camera mainCam;

    private CommandBuffer[] hintBufferPreBindCBs;
    private CommandBuffer[] hintBufferPostBindCBs;
  };
}