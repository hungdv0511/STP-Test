using Newtonsoft.Json;
using Proyecto26;
using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ApiUrl
{
    public static string host => "http://27.72.98.184:1688";
    public static string PostLoginUrl() => $"{host}/v1/auth/login";
    public static string GetItemBuffUrl() => $"{host}/v1/user/items/inUse";
    public static string GetItemConfigUrl() => $"{host}/v1/item/config";
}


public class STPRestClient : MonoBehaviour
{
    public enum RefreshTokenResult
    {
        SUCCESS,
        FAILED_BY_EXPIRED,
        FAILED_BY_REFRESH,
        FAILED_BY_NETWORKING,
        FAILED_BY_UNKNOW
    }

    public static string StepID { get; set; }
    public static string ErrorResponse { get; set; }

    public static string AccessToken
    {
        get
        {
            return PlayerPrefs.GetString("AcessToken");
        }
        set
        {
            PlayerPrefs.SetString("AcessToken", value);
        }
    }
    public static DateTime RefreshTokenExpires { get; set; }

    public bool IsAuthed
    {
        get
        {
            return !string.IsNullOrEmpty(AccessToken);
        }
    }

    public const int MAX_ERROR_LOOP_HANDLE = 4;

    public static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp);
    }

    public static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalSeconds);
    }

    public static bool CheckAccessTokenExpired()
    {
        try
        {
            string payload = JWT.JsonWebToken.Decode(AccessToken, new byte[0], false);
            var headerData = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
            var exp = headerData["exp"];
            var expired = Convert.ToDouble(exp);
            var currentEpoch = ConvertToUnixTimestamp(DateTime.UtcNow);
            return expired > currentEpoch;
            //            return false;
        }
        catch (Exception e)
        {
            return false;
        }

    }


    public static void AppendAccessToken(RequestHelper req)
    {
        RemoveAccessToken(req);
        req.Headers.Add("Authorization", $"Bearer {AccessToken}");

    }

    public static void RemoveAccessToken(RequestHelper req)
    {
        req.Headers.Remove("Authorization");

    }

    public static IPromise SendRequest(STPParameter parameter, bool isPlayerInCombat = false, Action callbackFailed = null)
    {
        if (parameter.AuthOptions == AuthOptions.NotRequired || parameter.AuthOptions == AuthOptions.AppendIfExists)
        {
            return SendRequestInternal(parameter, isPlayerInCombat, callbackFailed);
        }
        else if (parameter.AuthOptions == AuthOptions.ErrorIfNotExists)
        {
            return null;
        }
        else if (parameter.AuthOptions == AuthOptions.SkipIfNotExists)
        {
            return null;
        }
        else
        {
            bool accessTokenEffective = CheckAccessTokenExpired();
            if (accessTokenEffective)
            {
                AppendAccessToken(parameter.RequestHelper);
                return SendRequestInternal(parameter, isPlayerInCombat, callbackFailed);
            }
            else
            {
            }
        }
        return null;
    }

    private static IPromise SendRequestInternal(STPParameter parameter, bool isPlayerInCombat = false, Action callbackFailed = null)
    {
        return RestClient.Request(parameter.RequestHelper)
         .Then(res =>
         {
             parameter.Callback?.Invoke(res);
         })
         .Catch((err) =>
         {
             /*
              * Handle loop exception
              */
             parameter.ErrorCount++;
             if (parameter.ErrorCount >= MAX_ERROR_LOOP_HANDLE)
             {
                 return;
             }

             callbackFailed?.Invoke();
             if (isPlayerInCombat == true)
             {
                 Debug.LogError("Error message: " + err.Message);
                 return;
             }
             if (err is RequestException rErr)
             {
                 ErrorResponse = rErr.Response;
                 if (rErr.IsHttpError)
                 {
                     if (rErr.StatusCode == 400)
                     {
                         parameter.SendRequestFailed?.Invoke();
                         // bad request: loi param gui len

                         EikenBadRequest eikenBadRequest = JsonConvert.DeserializeObject<EikenBadRequest>(rErr.Response);
                         //var messageDisplay = string.Join("\n- ", eikenBadRequest.Errors.Select(x => x.Message));
                         //var messageDisplay = "Something went wrong :(";
                         StringBuilder messageBuilder = new StringBuilder();
                         if (eikenBadRequest.Errors != null)
                         {
                             for (int i = 0; i < eikenBadRequest.Errors.Count; i++)
                             {
                                 messageBuilder.AppendLine($"{i + 1}. {eikenBadRequest.Errors[i].Message}");
                             }
                         }
                         Debug.LogError(messageBuilder);

                         if (parameter.SendRequestFailed == null)
                         {
                             string errorMessage = messageBuilder.ToString();
                             if (string.IsNullOrEmpty(errorMessage))
                             {
                                 errorMessage = "Wrong username or password";
                             }
                         }
                     }
                     else if (rErr.StatusCode == 401)
                     {
                         parameter.SendRequestFailed?.Invoke();
                         // chua co dang nhap
                         RestClient.ClearDefaultParams();
                     }
                     else if (rErr.StatusCode == 404)
                     {

                     }
                     else
                     {
                         if (rErr.StatusCode == 502)
                         {

                         }
                         else
                         {
                             parameter.SendRequestFailed?.Invoke();
#if UNITY_EDITOR
                             Debug.LogError("Sttatus Code: " + rErr.StatusCode);
                             Debug.LogError("Error message: " + rErr.Message);
                             Debug.LogError("API Error: " + parameter.RequestHelper.Uri);
#endif
                         }
                     }
                 }
                 else if (rErr.IsNetworkError)
                 {
                     parameter.SendRequestFailed?.Invoke();
                     if (parameter.SendRequestFailed == null)
                     {

                     }
                 }

             }
             else
             {
                 parameter.SendRequestFailed?.Invoke();
#if UNITY_EDITOR
                 Debug.LogError("Error message: " + err.Message);
                 Debug.LogError("API Error: " + parameter.RequestHelper.Uri);
#endif
             }


         });
    }


    public class STPParameter
    {
        public RequestHelper RequestHelper { get; set; }

        public Action<ResponseHelper> Callback { get; set; }

        public Action SendRequestFailed { get; set; }

        public AuthOptions AuthOptions { get; set; } = AuthOptions.AppendIfExists;

        public bool ShowNetworkingError { get; set; } = false;

        public int ErrorCount { get; set; } = 0;
    }

    public enum AuthOptions
    {
        NotRequired,
        AppendIfExists,
        ErrorIfNotExists,
        SkipIfNotExists
    }
}