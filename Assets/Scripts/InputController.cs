using System;
using UnityEngine;
using Valve.VR; 
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    public UnityEvent OnWakeUp;
    
    ulong actionSetHandle = 0;
    ulong actionHandle = 0;
    private void Start()
    {
        OpenVRUtil.System.InitOpenVR();
        
        var error = OpenVR.Input.SetActionManifestPath(Application.streamingAssetsPath + "/SteamVR/actions.json");
        if (error != EVRInputError.None)
        {
            Debug.LogError("Failed to set action manifest path: " + error);
        }
        
        error = OpenVR.Input.GetActionSetHandle("/actions/Watch", ref actionSetHandle);
        if (error != EVRInputError.None)
        {
            Debug.LogError("Failed to get action set handle: " + error);
        }
        
        error = OpenVR.Input.GetActionHandle($"/actions/Watch/in/WakeUp", ref actionHandle);
        if (error != EVRInputError.None)
        {
            Debug.LogError("Failed to get action handle: " + error);
        }
    }

    private void Destroy()
    {
        OpenVRUtil.System.ShutdownOpenVR();
    }

    private void Update()
    {
        var actionSetList = new VRActiveActionSet_t[]
        {
            new VRActiveActionSet_t()
            {
                ulActionSet = actionSetHandle,
                ulRestrictedToDevice = OpenVR.k_ulInvalidInputValueHandle
            }
        };
        
        var activeActionSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRActiveActionSet_t));
        var error = OpenVR.Input.UpdateActionState(actionSetList, activeActionSize);
        if (error != EVRInputError.None)
        {
            Debug.LogError("Failed to update action state: " + error);
        }
        
        var result = new InputDigitalActionData_t();
        var digitalActionSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(InputDigitalActionData_t));
        error = OpenVR.Input.GetDigitalActionData(actionHandle, ref result, digitalActionSize, OpenVR.k_ulInvalidInputValueHandle);
        if (error != EVRInputError.None)
        {
            Debug.LogError("Failed to get digital action data: " + error);
        }

        if (result.bState && result.bChanged)
        {
            OnWakeUp.Invoke();
        }
    }
}