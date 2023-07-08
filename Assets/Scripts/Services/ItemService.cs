using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Proyecto26;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class ItemService
{
    public ItemService() { }

    public void GetItemBuff(Action<ListItemDto> callback, Action sendRequestFailed = null)
    {
        STPRestClient.SendRequest(new STPRestClient.STPParameter()
        {
            RequestHelper = new RequestHelper()
            {
                Headers = new Dictionary<string, string>
                    {
                        {
                            "Authorization", $"Bearer {STPRestClient.AccessToken}"
                        }
                    },
                Method = "GET",
                Uri = ApiUrl.GetItemBuffUrl(),
                ContentType = "application/json",
            },
            Callback = (res) =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                var data = JsonConvert.DeserializeObject<ListItemDto>(res.Text, settings);
                callback(data);
            },
            SendRequestFailed = sendRequestFailed
        });
    }


    public void GetItemConfig(Action<List<ItemConfig>> callback, Action sendRequestFailed = null)
    {
        STPRestClient.SendRequest(new STPRestClient.STPParameter()
        {
            RequestHelper = new RequestHelper()
            {
                Headers = new Dictionary<string, string>
                {
                    {
                        "Authorization", $"Bearer {STPRestClient.AccessToken}"
                    }
                },
                Method = "GET",
                Uri = ApiUrl.GetItemConfigUrl(),
                ContentType = "application/json",
            },
            Callback = (res) =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                var data = JsonConvert.DeserializeObject<List<ItemConfig>>(res.Text, settings);
                callback(data);
            },
            SendRequestFailed = sendRequestFailed
        });
    }
}
