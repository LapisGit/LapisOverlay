using UnityEngine;
using System;
using TMPro;

public class Watch : MonoBehaviour
{
    private TextMeshProUGUI label;

    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        var hour = DateTime.Now.Hour;
        var minute = DateTime.Now.Minute;
        var second = DateTime.Now.Second;
        label.text = $"{hour:00}:{minute:00}:{second:00}";
    }
}