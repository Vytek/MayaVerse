using UnityEngine;
using VRTK;

public sealed class AvatarCameraRigSync : MonoBehaviour {
    public GameObject AvatarHead;
    public GameObject LeftHand;
    public GameObject RightHand;

    private void OnEnable() {
		Invoke("ShimBypassForTime", 2); //DEBUG
    }

	private void ShimBypassForTime() {
		SetUpTransformFollow(AvatarHead, VRTK_DeviceFinder.Devices.Headset);
		SetUpTransformFollow(LeftHand, VRTK_DeviceFinder.Devices.LeftController);
		SetUpTransformFollow(RightHand, VRTK_DeviceFinder.Devices.RightController);
	}

    private static void SetUpTransformFollow(GameObject avatarComponent, VRTK_DeviceFinder.Devices device) {

        var transformFollow = avatarComponent.AddComponent<VRTK_TransformFollow>();
        Debug.Log(VRTK_DeviceFinder.DeviceTransform(device).gameObject);
        transformFollow.gameObjectToFollow = VRTK_DeviceFinder.DeviceTransform(device).gameObject;
        transformFollow.followsScale = false;

    }
}
