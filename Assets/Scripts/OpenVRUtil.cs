using UnityEngine;
using Valve.VR;
using System;

namespace OpenVRUtil
{
    public static class System
    {
        public static void InitOpenVR()
        {
            if (OpenVR.System != null) return;

            var error = EVRInitError.None;
            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);
            if (error != EVRInitError.None)
            {
                Debug.LogError("Failed to initialize OpenVR: " + error);
                return;
            }
        }
        
        public static void ShutdownOpenVR()
        {
            if (OpenVR.System != null)
            {
                OpenVR.Shutdown();
            }
        }
    }

    public static class Overlay
    {
        public static ulong CreateOverlay(string key, string name)
        {
            var handle = OpenVR.k_ulOverlayHandleInvalid;
            var error = OpenVR.Overlay.CreateOverlay(key, name, ref handle);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to create overlay: " + error);
            }

            return handle;
        }
        
        public static void SetOverlayRenderTexture(ulong handle, RenderTexture renderTexture)
        {
            if (!renderTexture.IsCreated()) return;
        
            var nativeTexturePtr = renderTexture.GetNativeTexturePtr();
            var texture = new Texture_t
            {
                handle = nativeTexturePtr,
                eType = ETextureType.DirectX,
                eColorSpace = EColorSpace.Auto
            };
            var error = OpenVR.Overlay.SetOverlayTexture(handle, ref texture);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to set overlay texture: " + error);

            }
        }
        
        public static void DestroyOverlay(ulong handle)
        {
            if (handle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var error = OpenVR.Overlay.DestroyOverlay(handle);
                if (error != EVROverlayError.None)
                {
                    throw new Exception("Failed to dispose overlay " + error);
                }
            }
        }
        
        public static void FlipOverlayVertical(ulong handle)
        {
            var bounds = new VRTextureBounds_t
            {
                uMin = 0f,
                vMin = 1f,
                uMax = 1f,
                vMax = 0f
            };
            var error = OpenVR.Overlay.SetOverlayTextureBounds(handle, ref bounds);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to flip overlay vertically: " + error);
            }
        }
        
        public static void SetOverlayTransformRelative(ulong handle, uint trackedDeviceIndex, Vector3 position, Quaternion rotation)
        {
            var rigidTransform = new SteamVR_Utils.RigidTransform(position, rotation);
            var matrix = rigidTransform.ToHmdMatrix34();
            var error = OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(handle, trackedDeviceIndex, ref matrix);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to set overlay transform relative to tracked device: " + error);
            }
        }
        
        public static void SetOverlayFromFile(ulong handle, string path)
        {
            var error = OpenVR.Overlay.SetOverlayFromFile(handle, path);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to set overlay from file: " + error);
            }
        }

        public static void ShowOverlay(ulong handle)
        {
            var error = OpenVR.Overlay.ShowOverlay(handle);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to show overlay: " + error);
            }
        }
        
        public static void HideOverlay(ulong handle)
        {
            var error = OpenVR.Overlay.HideOverlay(handle);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to hide overlay: " + error);
            }
        }
        
        public static void SetOverlaySize(ulong handle, float size)
        {
            var error = OpenVR.Overlay.SetOverlayWidthInMeters(handle, size);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to set overlay size: " + error);
            }
        }

        public static void SetOverlayTransformAbsolute(ulong handle, Vector3 position, Quaternion rotation)
        {
            var rigidTransform = new SteamVR_Utils.RigidTransform(position, rotation);
            var matrix = rigidTransform.ToHmdMatrix34();
            var error = OpenVR.Overlay.SetOverlayTransformAbsolute(handle, ETrackingUniverseOrigin.TrackingUniverseStanding,
                ref matrix);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to set overlay transform: " + error);
            }
        }
        
        public static (ulong, ulong) CreateDashboardOverlay(string key, string name)
        {
            ulong dashboardHandle = 0;
            ulong thumbnailHandle = 0;
            var error = OpenVR.Overlay.CreateDashboardOverlay(key, name, ref dashboardHandle, ref thumbnailHandle);
            if (error != EVROverlayError.None)
            {
                throw new Exception("Failed to create dashboard overlay: " + error);
            }
            
            return (dashboardHandle, thumbnailHandle);
        }

        public static bool IsValidHandle(ulong handle)
        {
            return handle != OpenVR.k_ulOverlayHandleInvalid && OpenVR.Overlay != null;
        }
        
        public static bool IsOpenVRReady()
        {
            return OpenVR.System != null && OpenVR.Overlay != null;
        }
    }
}