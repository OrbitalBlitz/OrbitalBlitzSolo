using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using OrbitalBlitz.Game.Features.API;
using OrbitalBlitz.Game.Features.API.Models;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class UserSession : MonoBehaviour {
    public static UserSession Instance;

    public event Action OnUserConnected;
    public event Action OnUserDisconnected;
    public event Action OnUserSignedIn;

    public event Action<string> onSignInError;
    public event Action<string> onLogInError;

    private bool TEST = false;

    private string URL {
        get {
            string url;
            #if UNITY_EDITOR
            url =  "https://127.0.0.1/api";
            #else
            url = "https://5.39.76.139/api";
            #endif
            return url;
        }
    }

    private HttpClient _client;

    private void Awake() {
        if (Instance == null)
            Instance = this;
    }

    private HttpClient client {
        get {
            _client ??= new();
            return client;
        }
    }

    private static List<Record> dummyWorldRecords = new() {
        new Record { userId = new User { username = "PlayerX" }, time = 100000000 },
        new Record { userId = new User { username = "PlayerY" }, time = 200000000 },
        new Record { userId = new User { username = "PlayerZ" }, time = 300000000 },
    };

    private static List<Record> dummyPersonalRecords = new() {
        new Record { userId = new User { username = "Me" }, time = 400000000 }
    };

    private string token;
    public string username;
    public string user_id;

    public IEnumerator LogIn(string username, string password) {
        // Debug.Log($"LogIn({username},{password})");
        string jsonData = JsonUtility.ToJson(new LoginRequestData { username = username, password = password });
        // Debug.Log("Sending JSON data: " + jsonData); 

        using (UnityWebRequest webRequest = new($"{URL}/auth/login", "POST")) {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.certificateHandler = new BypassCertificate();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            // webRequest.SetRequestHeader("Authorization", "Bearer your_access_token_here");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                onLogInError?.Invoke($"Error: {webRequest.error}");
                Debug.Log($"Error: {webRequest.error}");
            }
            else {
                LoginResponseData response = JsonUtility.FromJson<LoginResponseData>(webRequest.downloadHandler.text);
                this.username = username;
                this.user_id = response.userId;
                this.token = response.token;
                Debug.Log($"Login Request Complete: {webRequest.downloadHandler.text}");
                OnUserConnected?.Invoke();
            }
        }
    }

    public void Disconnect() {
        token = null;
        username = null;
        user_id = null;
        OnUserDisconnected?.Invoke();
    }

    public IEnumerator SignIn(string username, string password) {
        string json_data = JsonUtility.ToJson(
            new LoginRequestData { username = username, password = password });

        using (UnityWebRequest webRequest = new($"{URL}/auth/signup", "POST")) {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.certificateHandler = new BypassCertificate();

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                onSignInError?.Invoke($"Error: {webRequest.error}");
                Debug.Log($"Error: {webRequest.error}");
            }
            else {
                SigninResponse response = JsonUtility.FromJson<SigninResponse>(
                    webRequest.downloadHandler.text);
                OnUserSignedIn?.Invoke();
                Debug.Log($"Signin Request Complete: {webRequest.downloadHandler.text}");
            }
        }
    }

    public IEnumerator GetPlayerRecords(
        string circuit_id,
        Action<List<Record>> onCompleted,
        Action<string> onError) {
        if (!IsConnected()) {
            onError?.Invoke("Cannot fetch records of an unauthenticated user.");
            yield break;
        }

        if (TEST) {
            Debug.Log($"GET {URL}/records/{user_id}/{circuit_id}");
            onCompleted?.Invoke(new List<Record>(dummyPersonalRecords));
            yield break;
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{URL}/records/{user_id}/{circuit_id}");
        webRequest.certificateHandler = new BypassCertificate();

        webRequest.SetRequestHeader("Authorization", $"Bearer {token}");

        yield return webRequest.SendWebRequest(); // This will pause the coroutine and resume after the request is done

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"Error fetching records: {webRequest.error}");
            onError?.Invoke(webRequest.error);
            yield break;
        }

        Debug.Log($"GET {URL}/records/{user_id}/{circuit_id}\nResponse:\n{webRequest.downloadHandler.text}");
        try {
            List<Record> records = getObjectListFromJson<Record>(webRequest.downloadHandler.text);
            onCompleted?.Invoke(records);
        }
        catch (Exception ex) {
            onError?.Invoke(
                $"Failed to parse player records json {webRequest.downloadHandler.text}. Error Report: {ex.Message}");
        }
    }

    /// <summary>
    /// Little trick function to let JsonUtility parse lists of objects
    /// </summary>
    private List<T> getObjectListFromJson<T>(string json) {
        json = "{ \"items\":" + json + "}";
        return JsonUtility.FromJson<ItemsList<T>>(json).items;
    }


    public IEnumerator GetPlayerMedal(
        string circuit_id,
        Action<Circuit.MedalType> onCompleted,
        Action<string> onError) {
        
        if (!IsConnected()) {
            onCompleted(Circuit.MedalType.NoMedal);
            yield break;
        }

        if (TEST) {
            Debug.Log($"GET {URL}/records/{username}/{circuit_id}");
            System.Random rd = new();
            int rd_medal_index = rd.Next(Enum.GetValues(typeof(Circuit.MedalType)).Length);
            onCompleted((Circuit.MedalType)rd_medal_index);
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{URL}/medals/{user_id}/{circuit_id}");
        webRequest.SetRequestHeader("Authorization", $"Bearer {token}");

        webRequest.certificateHandler = new BypassCertificate();

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError) {
            onError?.Invoke(webRequest.error);
        }
        else {
            try {
                List<MedalData> medals = getObjectListFromJson<MedalData>(
                    webRequest.downloadHandler.text);

                Dictionary<string, Circuit.MedalType> mapping = new() {
                    { "gold", Circuit.MedalType.Gold },
                    { "silver", Circuit.MedalType.Silver },
                    { "bronze", Circuit.MedalType.Bronze },
                    { "none", Circuit.MedalType.NoMedal }
                };
                var best_medal = medals
                    .Select(medal => mapping[medal.medal])
                    .Aggregate((best, medal) => (medal > best) ? medal : best);
                onCompleted?.Invoke(best_medal);
            }
            catch (Exception ex) {
                onError?.Invoke(
                    $"Failed to parse player medals json {webRequest.downloadHandler.text}. Error Report: {ex.Message}");
            }
        }
    }

    public IEnumerator GetWorldRecords(
        string circuit_id,
        Action<List<Record>> onCompleted,
        Action<string> onError) {
        if (TEST) {
            Debug.Log($"GET {URL}/records/{username}/{circuit_id}");
            onCompleted(dummyWorldRecords);
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{URL}/records/{circuit_id}");
        webRequest.certificateHandler = new BypassCertificate();

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"Error fetching world records: {webRequest.error}");
            onError?.Invoke(webRequest.error);
        }
        else {
            try {
                List<Record> records = getObjectListFromJson<Record>(
                    webRequest.downloadHandler.text);
                onCompleted?.Invoke(records);
            }
            catch (Exception ex) {
                onError?.Invoke(
                    $"Failed to parse world records json {webRequest.downloadHandler.text}. Error Report: {ex.Message}");
            }
        }
    }

    public IEnumerator SaveRecord(
        string circuit_id,
        long time,
        Action<string> onCompleted,
        Action<string> onError) {
        if (!IsConnected()) {
            onError?.Invoke("Cannot save records when unauthenticated.");
        }

        string json_data = JsonUtility.ToJson(
            new RecordToSave {
                userId = user_id,
                circuitId = circuit_id,
                time = (int)time
            });
        Debug.Log($"POST {URL}/records \nbody : {json_data}");

        if (TEST) {
            onCompleted?.Invoke("Dummy record saved.");
        }

        using (UnityWebRequest webRequest = new($"{URL}/records", "POST")) {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.certificateHandler = new BypassCertificate();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            // webRequest.SetRequestHeader("Authorization", $"Bearer {token}");


            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                onSignInError?.Invoke($"Error: {webRequest.error}");
                Debug.Log($"Error: {webRequest.error}");
            }
            else {
                RecordSavedResponse response = JsonUtility.FromJson<RecordSavedResponse>(
                    webRequest.downloadHandler.text);
                onCompleted?.Invoke(response.message);
            }
        }
    }

    public IEnumerator SaveMedal(
        string circuit_id,
        Circuit.MedalType medal,
        Action<string> onCompleted,
        Action<string> onError) {
        if (!IsConnected()) onError?.Invoke("Cannot save medal when unauthenticated.");
        if (medal == Circuit.MedalType.NoMedal) onCompleted?.Invoke("No medal to save.");

        var json_data = JsonUtility.ToJson(
            new MedalToSave(user_id, circuit_id, medal.ToString().ToLower()));
        var values = new Dictionary<string, object> {
            { "userId", user_id },
            { "circuitId", circuit_id },
            { "medal", medal.ToString().ToLower() }
        };

        if (TEST) {
            Debug.Log($"POST {URL}/medals \nbody : {values.Serialize()}");
            onCompleted?.Invoke("Saved dummy medal.");
        }

        using (UnityWebRequest webRequest = new($"{URL}/records", "POST")) {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_data);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.certificateHandler = new BypassCertificate();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {token}");


            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                onSignInError?.Invoke($"Error: {webRequest.error}");
                Debug.Log($"Error: {webRequest.error}");
            }
            else {
                MedalSavedResponse response = JsonUtility.FromJson<MedalSavedResponse>(
                    webRequest.downloadHandler.text);
                onCompleted?.Invoke(response.message);
            }
        }
    }

    public bool IsConnected() {
        return token != null;
    }
}