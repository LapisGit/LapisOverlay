using UnityEngine;
using Valve.VR;

public class WatchSettingController : MonoBehaviour
{
    bool mediaToggleState = true;
    
    [SerializeField] private WatchOverlay watchOverlay;

    public void OnLeftHandButtonClick()
    {
        watchOverlay.targetHand = ETrackedControllerRole.LeftHand;
    }
    
    public void OnRightHandButtonClick()
    {
        watchOverlay.targetHand = ETrackedControllerRole.RightHand;
    }

    public void OnMediaToggleClick()
    {
        if (mediaToggleState == false)
        {
            mediaToggleState = true;
            GameObject.Find("Watch/Canvas/Media").SetActive(true);
        } 
        else
        {
            mediaToggleState = false;
            GameObject.Find("Watch/Canvas/Media").SetActive(false);
        }
    }
}