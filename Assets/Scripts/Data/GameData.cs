using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public Dictionary<string, string> itemDescriptionDicts = new Dictionary<string, string>();

    public Dictionary<string, Sprite> itemThumbnails = new Dictionary<string, Sprite>();

    public void GetItemDescriptionDictionary(List<ItemConfig> listItemConfig, Action loadDataDone)
    {
        for (int i = 0; i < listItemConfig.Count; i++)
        {
            if (itemDescriptionDicts.ContainsKey(listItemConfig[i].id)) continue;
            itemDescriptionDicts.Add(listItemConfig[i].id, listItemConfig[i].description);
            if (!itemThumbnails.ContainsKey(listItemConfig[i].id))
            {
                StartCoroutine(DownloadImage(listItemConfig[i].id, $"{ApiUrl.host}/{listItemConfig[i].thumbnail}"));
            }
        }
        loadDataDone?.Invoke();
    }

    IEnumerator DownloadImage(string itemId, string thumbnailUrl)
    {
        Texture2D texture2D;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(thumbnailUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite avaSprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
            itemThumbnails.Add(itemId, avaSprite);
        }
    }

    public string GetDescription(string key)
    {
        if (!itemDescriptionDicts.ContainsKey(key))
        {
            return string.Empty;
        }
        return itemDescriptionDicts[key];
    }

    public void ClearData()
    {
        itemDescriptionDicts.Clear();
        itemThumbnails.Clear();
    }
}
