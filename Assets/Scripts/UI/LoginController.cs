using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Mail;
using System;
using UnityEngine.Events;
using System.Text.RegularExpressions;

public class LoginController : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;

    [SerializeField] TextMeshProUGUI tmpWarningEmail;
    [SerializeField] TextMeshProUGUI tmpWarningPassword;

    [Header("Events: ")]
    [SerializeField] UnityEvent onLoginSuccessEvent;
    [SerializeField] UnityEvent onBlockLogin;
    [SerializeField] UnityEvent onTryLater;


    private void Start()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("AcessToken")))
        {
            onLoginSuccessEvent.Invoke();
        }
        else
        {
            //emailInput.text = "ss@gmail.com";
            //passwordInput.text = "Test123@";
        }
    }

    private void OnDisable()
    {
        emailInput.text = string.Empty;
        passwordInput.text = string.Empty;
    }

    public void OnClickLogin()
    {
        if (!IsEmail(emailInput.text))
        {
            tmpWarningEmail.text = "Incorrect Email";
            Debug.LogError("Email is invalid format!");
            return;
        }
        Registry.authenticateService.SendRequestLogin(new UserRequest() { Email = emailInput.text, Password = passwordInput.text }, (accessToken) =>
        {
            Debug.Log("Login Success!");
            onLoginSuccessEvent?.Invoke();
        }, () =>
        {
            if (STPRestClient.ErrorResponse.Contains("USER_NOT_FOUND"))
            {
                tmpWarningEmail.text = "Incorrect Email";
                Debug.LogError("Email is invalid format!");
            }
            if (STPRestClient.ErrorResponse.Contains("PASSWORD_IS_WRONG"))
            {
                tmpWarningPassword.text = "Incorrect Password";
                Debug.LogError("Wrong Password!");
            }
            if (STPRestClient.ErrorResponse.Contains("TOO_MANY_WRONG_LOGIN"))
            {
                onBlockLogin?.Invoke();
            }
            if (!STPRestClient.ErrorResponse.Contains("USER_NOT_FOUND") && !STPRestClient.ErrorResponse.Contains("PASSWORD_IS_WRONG") && !STPRestClient.ErrorResponse.Contains("TOO_MANY_WRONG_LOGIN"))
            {
                onTryLater?.Invoke();
            }
        });
    }

    public bool IsEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
    }

    public void OnClickLogout()
    {
        STPRestClient.AccessToken = string.Empty;
        STPRestClient.ErrorResponse = string.Empty;
        GameData.Instance.ClearData();
        tmpWarningEmail.text = string.Empty;
        tmpWarningPassword.text = string.Empty;
    }
}
