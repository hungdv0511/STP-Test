using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserResponse
{
    public string Email { get; set; }
    [JsonProperty("Password")]
    public string AccessToken { get; set; }

    public UserResponse(string email = default, string accessToken = default)
    {
        Email = email;
        AccessToken = accessToken;
    }
}


public class UserRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("password")]
    public string Password { get; set; }
}

