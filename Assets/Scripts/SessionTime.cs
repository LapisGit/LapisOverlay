using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SessionTime : MonoBehaviour
{
    private TextMeshProUGUI label;
    private float sessionStartTime;

    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        sessionStartTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - sessionStartTime;
        
        int hours = (int)(elapsedTime / 3600);
        int minutes = (int)((elapsedTime % 3600) / 60);
        int seconds = (int)(elapsedTime % 60);
        
        label.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }
}
