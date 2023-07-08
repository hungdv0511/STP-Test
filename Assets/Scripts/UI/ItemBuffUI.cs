using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ItemBuffUI : MonoBehaviour
{
    [SerializeField] Image imgThumbnail;
    [SerializeField] Image imgProgress;
    [SerializeField] TextMeshProUGUI tmpTimeRemaining;
    [SerializeField] TextMeshProUGUI tmpItemName;
    [SerializeField] TextMeshProUGUI tmpItemDescription;

    DateTime? startUsedTime;
    DateTime? expiredDate;
    TimeSpan totalTimeUse;

    private void Update()
    {
        if (!startUsedTime.HasValue || !expiredDate.HasValue)
        {
            return;
        }
        TimeSpan currentTimeRemaining = expiredDate.Value - DateTime.UtcNow;
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

    public void UpdateItem(Sprite thumbnail, DateTime startUsedTime, DateTime expiredDate, string itemName, string description)
    {
        imgThumbnail.sprite = thumbnail;
        this.startUsedTime = startUsedTime;
        this.expiredDate = expiredDate;
        tmpItemName.text = itemName;
        tmpItemDescription.text = description;
        totalTimeUse = expiredDate - startUsedTime;
    }
}
