using UnityEngine;
using Valve.VR;
using System;
using OpenVRUtil;

public class WatchOverlay : MonoBehaviour
{
    public Camera camera;
    public RenderTexture renderTexture;
    public ETrackedControllerRole targetHand = ETrackedControllerRole.LeftHand;
    private ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
    private bool isVisible = false;

    [Range(0, 0.5f)] public float size = 0.5f;
    [Range(-0.2f, 0.2f)] public float x;
    [Range(-0.2f, 0.2f)] public float y;
    [Range(-0.2f, 0.2f)] public float z;
    [Range(0, 360)] public int rotationX;
    [Range(0, 360)] public int rotationY;
    [Range(0, 360)] public int rotationZ;

    private void Start()
    {
        OpenVRUtil.System.InitOpenVR();
        overlayHandle = Overlay.CreateOverlay("WatchOverlayKey", "WatchOverlay");
        
        Overlay.FlipOverlayVertical(overlayHandle);
        Overlay.SetOverlaySize(overlayHandle, size);
    }

    private void OnDestroy()
    {
        OpenVRUtil.System.ShutdownOpenVR();
    }

    private void OnApplicationQuit()
    {
        Overlay.DestroyOverlay(overlayHandle);
    }
    
    

    private void Update()
    {
        var position = new Vector3(x, y, z);
        var rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        var controllerIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole(targetHand);
        if (controllerIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
        {
            Overlay.SetOverlayTransformRelative(overlayHandle, controllerIndex, position, rotation);
        }

        if (!renderTexture.IsCreated())
        {
            return;
        }

        Overlay.SetOverlayRenderTexture(overlayHandle, renderTexture);
    }

    public void OnWakeUp()
    {
        if (isVisible)
        {
            Overlay.HideOverlay(overlayHandle);
            isVisible = false;
        }
        else
        {
            Overlay.ShowOverlay(overlayHandle);
            isVisible = true;
        }
    }
}
