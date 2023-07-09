using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventoryField : MonoBehaviour
{
    [SerializeField] ItemBuffUI itemBuffPrefab;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject btnLogout;

    ListItemDto listItemDto;

    private void OnEnable()
    {
        Registry.itemService.GetItemBuff((listItemResponse) =>
        {
            //ShowListItem(listItemResponse);
            this.listItemDto = listItemResponse;
        }, () =>
        {
            Debug.LogError("Get Item Buff Failed");
        });
    }

    private void OnDisable()
    {
        this.listItemDto = null;
        StopAllCoroutines();
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnShowBuffStatus()
    {
        if (!btnLogout.activeInHierarchy)
        {
            btnLogout.SetActive(true);
        }
        if (content.transform.childCount > 0) return; //avoid manual enable/disable on editor
        StartCoroutine(IEDelayShowBuffs());
    }

    private IEnumerator IEDelayShowBuffs()
    {
        yield return new WaitUntil(() => this.listItemDto != null);
        ShowListItem(listItemDto);
    }

    private void ShowListItem(ListItemDto listItemDto)
    {
        if (listItemDto == null || listItemDto.data.Count == 0)
        {
            return;
        }
        QuickSort(listItemDto.data, 0, listItemDto.data.Count - 1);
        for (int i = 0; i < listItemDto.data.Count; i++)
        {
            ItemDto itemDto = listItemDto.data[i];
            if (!GameData.Instance.itemThumbnails.ContainsKey(itemDto.ItemId)) continue;
            if (!GameData.Instance.itemDescriptionDicts.ContainsKey(itemDto.ItemId)) continue;
            ItemBuffUI newItem = Instantiate(itemBuffPrefab, content);
            Sprite thumbnail = GameData.Instance.itemThumbnails[itemDto.ItemId];
            string itemName = itemDto.ItemId;
            DateTime startUsedTime = itemDto.useTime;
            DateTime expiredDate = itemDto.expiredDate;
            string description = GameData.Instance.GetDescription(itemDto.ItemId);
            newItem.UpdateItem(thumbnail, startUsedTime, expiredDate, itemName, description, itemDto.duration);
        }
    }


    public void QuickSort(List<ItemDto> itemDtos, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(itemDtos, low, high);

            QuickSort(itemDtos, low, pivotIndex - 1);
            QuickSort(itemDtos, pivotIndex + 1, high);
        }
    }

    public int Partition(List<ItemDto> itemDtos, int low, int high)
    {
        ItemDto pivot = itemDtos[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (itemDtos[j].useTime > pivot.useTime)
            {
                i++;
                Swap(itemDtos, i, j);
            }
        }

        Swap(itemDtos, i + 1, high);
        return i + 1;
    }

    public void Swap(List<ItemDto> itemDtos, int i, int j)
    {
        ItemDto temp = itemDtos[i];
        itemDtos[i] = itemDtos[j];
        itemDtos[j] = temp;
    }
}
