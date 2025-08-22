using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DashboardOverlay : MonoBehaviour
{
    private ulong dashboardHandle = OpenVR.k_ulOverlayHandleInvalid;
    private ulong thumbnailHandle = OpenVR.k_ulOverlayHandleInvalid;
    public Camera camera;
    public RenderTexture renderTexture; 
    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;
    

    private void Start()
    {
        OpenVRUtil.System.InitOpenVR();

        (dashboardHandle, thumbnailHandle) = OpenVRUtil.Overlay.CreateDashboardOverlay("LPS-Dashboard", "LapisOverlay");
        
        var filePath = Application.streamingAssetsPath + "/lapispfp.png";
        
        OpenVRUtil.Overlay.FlipOverlayVertical(dashboardHandle);
        OpenVRUtil.Overlay.SetOverlaySize(dashboardHandle, 2.5f);
        OpenVRUtil.Overlay.SetOverlayFromFile(thumbnailHandle, filePath);

        var mouseScalingFactor = new HmdVector2_t()
        {
            v0 = renderTexture.width,
            v1 = renderTexture.height
        };
        var error = OpenVR.Overlay.SetOverlayMouseScale(dashboardHandle, ref mouseScalingFactor);
        if (error != EVROverlayError.None)
        {
            Debug.LogError("Failed to set overlay mouse scale: " + error);
        }
    }
    private void OnApplicationQuit()
    {
        OpenVRUtil.Overlay.DestroyOverlay(dashboardHandle);
    }

    private void OnDestroy()
    {
        OpenVRUtil.System.ShutdownOpenVR();
    }

    private void Update()
    {
        OpenVRUtil.Overlay.SetOverlayRenderTexture(dashboardHandle, renderTexture);
        ProccessOverlayEvents();
    }

    private Button GetButtonByPosition(Vector2 position)
    {
        var pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = position;
        
        var raycastResultList = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, raycastResultList);
        var raycastResult = raycastResultList.Find(element => element.gameObject.GetComponent<Button>() != null);
        if (raycastResult.gameObject != null)
        {
            return raycastResult.gameObject.GetComponent<Button>();
        }
        Debug.LogWarning("No button found at position: " + position);
        return null;
    }

    private void ProccessOverlayEvents()
    {
        var vrEvent = new VREvent_t();
        var uncbVREvent = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));
        while (OpenVR.Overlay.PollNextOverlayEvent(dashboardHandle, ref vrEvent, uncbVREvent))
        {
            switch ((EVREventType)vrEvent.eventType)
            {
                case EVREventType.VREvent_MouseButtonDown:
                    vrEvent.data.mouse.y = renderTexture.height - vrEvent.data.mouse.y;
                    var button = GetButtonByPosition(new Vector2(vrEvent.data.mouse.x, vrEvent.data.mouse.y));
                    if (button != null)
                    {
                        button.onClick.Invoke();
                    }
                    break;
            }
        }
    }
}