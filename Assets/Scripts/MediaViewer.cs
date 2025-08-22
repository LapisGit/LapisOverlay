using UnityEngine;
using TMPro;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class MediaInfo
{
    public string Title;
    public string Artist;
    public string Album;
    public string ThumbnailBase64;
    public string Timestamp;
    public string PlaybackStatus;
}

public class MediaViewer : MonoBehaviour
{
    [Header("UI References")] public TextMeshProUGUI mediaTitle;
    public TextMeshProUGUI mediaArtist;
    public Image mediaImage;

    [Header("Settings")] public float updateInterval = 1f;
    
    private Process mediaManagerProcess;
    private string executablePath;
    private string mediaLogPath;
    private DateTime lastLogWriteTime;
    private Coroutine monitoringCoroutine;
    private string currentThumbnailBase64;
    private Coroutine titleScrollCoroutine;
    private Coroutine artistScrollCoroutine;

    void Start()
    {
        SetupPaths();
        StartMediaManager();
        StartMonitoring();
    }

    void SetupPaths()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        executablePath = Path.Combine(streamingAssetsPath, "MediaOverlay", "LapisOverlayMediaManager.exe");
        mediaLogPath = Path.Combine(streamingAssetsPath, "MediaOverlay", "media_log.json");
    }

    void StartMediaManager()
    {
        try
        {
            if (File.Exists(executablePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = executablePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(executablePath)
                };

                mediaManagerProcess = Process.Start(startInfo);
                if (mediaManagerProcess != null)
                {
                    UnityEngine.Debug.Log($"Started MediaManager process with ID: {mediaManagerProcess.Id}");
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to start MediaManager process - Process.Start returned null");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"MediaManager executable not found at: {executablePath}");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to start MediaManager: {ex.Message}");
        }
    }

    void StartMonitoring()
    {
        if (monitoringCoroutine != null)
        {
            StopCoroutine(monitoringCoroutine);
        }

        UnityEngine.Debug.Log("MediaViewer: Starting media log monitoring...");
        monitoringCoroutine = StartCoroutine(MonitorMediaLog());
    }

    IEnumerator MonitorMediaLog()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            CheckForLogUpdates();
        }
    }

    void CheckForLogUpdates()
    {
        try
        {
            if (File.Exists(mediaLogPath))
            {
                DateTime currentWriteTime = File.GetLastWriteTime(mediaLogPath);

                if (currentWriteTime != lastLogWriteTime)
                {
                    lastLogWriteTime = currentWriteTime;
                    ReadAndParseLog();
                }
            }
            else
            {
                UnityEngine.Debug.Log($"MediaViewer: Media log file does not exist at: {mediaLogPath}");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error checking log updates: {ex.Message}");
        }
    }

    void ReadAndParseLog()
    {
        try
        {
            string jsonContent = File.ReadAllText(mediaLogPath);

            if (!string.IsNullOrEmpty(jsonContent))
            {
                MediaInfo mediaInfo = JsonUtility.FromJson<MediaInfo>(jsonContent);
                if (mediaInfo != null)
                {
                    UpdateMediaDisplay(mediaInfo);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("MediaViewer: Failed to deserialize JSON - result was null");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("MediaViewer: JSON content was empty or null");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error reading/parsing media log: {ex.Message}");
        }
    }

    void UpdateMediaDisplay(MediaInfo mediaInfo)
    {
        if (mediaInfo != null)
        {
            string titleText = "No Media";
            string artistText = "";

            if (!string.IsNullOrEmpty(mediaInfo.Title) && !string.IsNullOrEmpty(mediaInfo.Artist))
            {
                titleText = mediaInfo.Title;
                artistText = mediaInfo.Artist;
            }
            else if (!string.IsNullOrEmpty(mediaInfo.Title))
            {
                titleText = mediaInfo.Title;
            }
            else if (!string.IsNullOrEmpty(mediaInfo.Artist))
            {
                artistText = mediaInfo.Artist;
            }
            else
            {
                titleText = "No Media";
            }

            mediaTitle.text = titleText;
            mediaArtist.text = artistText;

            if (!string.IsNullOrEmpty(mediaInfo.ThumbnailBase64) && mediaInfo.ThumbnailBase64 != currentThumbnailBase64)
            {
                currentThumbnailBase64 = mediaInfo.ThumbnailBase64;
                StartCoroutine(LoadThumbnail(mediaInfo.ThumbnailBase64));
            }
        }
    }


    IEnumerator LoadThumbnail(string base64String)
    {
        byte[] imageBytes = Convert.FromBase64String(base64String);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        mediaImage.sprite =
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        yield return null;
    }

    void OnApplicationQuit()
    {
        KillMediaManager();
    }

    void OnDestroy()
    {
        KillMediaManager();
    }

    void KillMediaManager()
    {
        try
        {
            if (mediaManagerProcess != null && !mediaManagerProcess.HasExited)
            {
                mediaManagerProcess.Kill();
                mediaManagerProcess.WaitForExit(5000);
                UnityEngine.Debug.Log("MediaManager process terminated");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error killing MediaManager process: {ex.Message}");
        }
        finally
        {
            if (mediaManagerProcess != null)
            {
                mediaManagerProcess.Dispose();
                mediaManagerProcess = null;
            }
        }

        if (monitoringCoroutine != null)
        {
            StopCoroutine(monitoringCoroutine);
            monitoringCoroutine = null;
        }

    }
}