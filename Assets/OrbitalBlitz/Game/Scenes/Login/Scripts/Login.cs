using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

public class Login : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField userNameField;
    [SerializeField] TMPro.TMP_InputField passwordField;
    public Button loginButton;
    public Button registerButton;
    
    private const string URL = "https://127.0.0.1:3001/api/auth/";
    private string urlParameters = "?api_key=123";
    
    void Start()
    {
        loginButton.onClick.AddListener(checkLogin);
        registerButton.onClick.AddListener(register);
    }
    public async void checkLogin()
    {
        string userName = userNameField.text;
        string password = passwordField.text;
        
        HttpClient client = new HttpClient();

        var values = new Dictionary<string, string>
        {
            { "userName", userName },
            { "password", password }
        };

        var content = new FormUrlEncodedContent(values);

        var response = await client.PostAsync(String.Format("{0}/login", URL), content);

        var responseString = await response.Content.ReadAsStringAsync();
        
        Debug.Log(response);
    }
    public async void register()
    {
        string userName = userNameField.text;
        string password = passwordField.text;
        
        HttpClient client = new HttpClient();

        var values = new Dictionary<string, string>
        {
            { "userName", userName },
            { "password", password }
        };

        var content = new FormUrlEncodedContent(values);

        var response = await client.PostAsync(String.Format("{0}/signup", URL), content);

        var responseString = await response.Content.ReadAsStringAsync();
        
        Debug.Log(responseString);
    }
}
