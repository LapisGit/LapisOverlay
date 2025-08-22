using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CurrentDate : MonoBehaviour
{
    private TextMeshProUGUI label;

    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        UpdateDate();
    }

    void Update()
    {
        UpdateDate();
    }

    void UpdateDate()
    {
        DateTime currentDate = DateTime.Now;
        label.text = $"{currentDate.Month}/{currentDate.Day}/{currentDate.Year}";
    }
}
