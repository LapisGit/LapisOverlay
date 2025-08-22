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
    
    [Range(-0.2f, 0.2f)] public float leftX;
    [Range(-0.2f, 0.2f)] public float leftY;
    [Range(-0.2f, 0.2f)] public float leftZ;
    [Range(0, 360)] public int leftRotationX;
    [Range(0, 360)] public int leftRotationY;
    [Range(0, 360)] public int leftRotationZ;
    
    [Range(-0.2f, 0.2f)] public float rightX;
    [Range(-0.2f, 0.2f)] public float rightY;
    [Range(-0.2f, 0.2f)] public float rightZ;
    [Range(0, 360)] public int rightRotationX;
    [Range(0, 360)] public int rightRotationY;
    [Range(0, 360)] public int rightRotationZ;
    

    private void Start()
    {
        OpenVRUtil.System.InitOpenVR();

        if (Overlay.IsOpenVRReady())
        {
            try
            {
                overlayHandle = Overlay.CreateOverlay("LPS-Watch", "LapisOverlayWatch");
                
                Overlay.FlipOverlayVertical(overlayHandle);
                Overlay.SetOverlaySize(overlayHandle, size);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to create watch overlay: " + e.Message);
                overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
            }
        }
        else
        {
            Debug.LogError("OpenVR not ready for overlay creation");
        }
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
        if (!Overlay.IsValidHandle(overlayHandle) || !Overlay.IsOpenVRReady())
        {
            return;
        }

        Vector3 position;
        Quaternion rotation;
        
        if (targetHand == ETrackedControllerRole.LeftHand)
        {
            position = new Vector3(leftX, leftY, leftZ);
            rotation = Quaternion.Euler(leftRotationX, leftRotationY, leftRotationZ);
        }
        else
        {
            position = new Vector3(rightX, rightY, rightZ);
            rotation = Quaternion.Euler(rightRotationX, rightRotationY, rightRotationZ);
        }
        
        var controllerIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole(targetHand);
        
        if (controllerIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
        {
            try
            {
                Overlay.SetOverlayTransformRelative(overlayHandle, controllerIndex, position, rotation);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to set overlay transform: " + e.Message);
                overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
                return;
            }
        }

        if (!renderTexture.IsCreated())
        {
            return;
        }

        try
        {
            Overlay.SetOverlayRenderTexture(overlayHandle, renderTexture);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set overlay render texture: " + e.Message);
        }
    }

    public void OnWakeUp()
    {
        if (!Overlay.IsValidHandle(overlayHandle))
        {
            Debug.LogWarning("Cannot toggle overlay visibility: Invalid overlay handle");
            return;
        }

        try
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
        catch (Exception e)
        {
            Debug.LogError("Failed to toggle overlay visibility: " + e.Message);
        }
    }
    

}
