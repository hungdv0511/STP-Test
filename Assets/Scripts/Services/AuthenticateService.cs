using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using Newtonsoft.Json;
using RSG;

public class AuthenticateService
{
    public AuthenticateService() { }

    public void SendRequestLogin(UserRequest userRequest, Action<AuthenticateResponse> callback, Action sendRequestFailed = null)
    {
        STPRestClient.SendRequest(new STPRestClient.STPParameter()
        {
            RequestHelper = new RequestHelper()
            {
                Method = "POST",
                Uri = ApiUrl.PostLoginUrl(),
                ContentType = "application/json",
                BodyString = JsonConvert.SerializeObject(userRequest)
            },
            Callback = (res) =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                var data = JsonConvert.DeserializeObject<AuthenticateResponse>(res.Text, settings);
                STPRestClient.AccessToken = data.AccessToken;
                callback(data);
            },
            SendRequestFailed = sendRequestFailed
        });
    }
}
