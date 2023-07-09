using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Globalization;

public class ItemBuffUI : MonoBehaviour
{
    [SerializeField] Image imgThumbnail;
    [SerializeField] Image imgProgress;
    [SerializeField] TextMeshProUGUI tmpTimeRemaining;
    [SerializeField] TextMeshProUGUI tmpItemName;
    [SerializeField] TextMeshProUGUI tmpItemDescription;
    string format = "dd/MM/yyyy hh:mm:ss tt";
    string format_2 = "MM/dd/yyyy hh:mm:ss tt";
    DateTime? startUsedTime;
    DateTime? expiredDate;
    TimeSpan totalTimeUse;
    float duration;


    private void Update()
    {
        if (!startUsedTime.HasValue || !expiredDate.HasValue)
        {
            return;
        }
        DateTime dt = ConvertDateTime(DateTime.UtcNow, format_2);
        TimeSpan currentTimeRemaining = expiredDate.Value - dt;
        if (currentTimeRemaining.TotalSeconds > 0)
        {
            tmpTimeRemaining.text = $"{currentTimeRemaining.TotalHours:00}:{currentTimeRemaining.Minutes:00}:{currentTimeRemaining.Seconds:00}";
            imgProgress.fillAmount = ((float)currentTimeRemaining.TotalSeconds / (float)totalTimeUse.TotalSeconds);
        }
        else
        {
            tmpTimeRemaining.text = "Item Expired";
            imgProgress.fillAmount = 0f;
        }
    }

    private void OnDestroy()
    {
        startUsedTime = null;
        expiredDate = null;
    }

    public void UpdateItem(Sprite thumbnail, DateTime startUsedTime, DateTime expiredDate, string itemName, string description, float duration)
    {
        imgThumbnail.sprite = thumbnail;
        //Debug.Log($"<color=cyan>startUsedTime: {startUsedTime}</color>");
        //Debug.Log($"<color=cyan>expiredDate: {expiredDate}</color>");

        this.startUsedTime = ConvertDateTime(startUsedTime, format);
        this.expiredDate = ConvertDateTime(expiredDate, format);
        //this.startUsedTime = startUsedTime;
        //this.expiredDate = expiredDate;

        //Debug.Log($"<color=cyan>startUsedTime after convert: {this.startUsedTime}</color>");
        //Debug.Log($"<color=cyan>expiredDate after convert: {  this.expiredDate }</color>");


        tmpItemName.text = itemName;
        tmpItemDescription.text = description;
        totalTimeUse = expiredDate - startUsedTime;
        this.duration = duration;
    }

    private DateTime ConvertDateTime(DateTime inputTime, string format)
    {
        string formattedTime = FormatDateTime(inputTime, format);
        DateTime dateTime = DateTime.Parse(formattedTime);
        return dateTime;
    }

    public string FormatDateTime(DateTime dateTime, string format)
    {
        CultureInfo cultureInfo = new CultureInfo("en-US");
        return dateTime.ToString(format, cultureInfo);
    }
}
