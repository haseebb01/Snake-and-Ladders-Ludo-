using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Amazon.Runtime.Internal.Transform;
using System.Linq;
using UnityEngine.Networking;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;

public enum State
{
    get, add, update, delete
}
public class API_AWS : MonoBehaviour
{
    [SerializeField] private string PlayerID;
    [SerializeField] private WebResponseAPI PlayerData;


    [Header("Only Work For Editor")]
    public string identifier = "A";
    public string GUID_PlayerID
    {
        get
        {
#if UNITY_EDITOR
            string _Value = PlayerPrefs.GetString("guidPlayerID" + identifier, "");
#else
            string _Value = PlayerPrefs.GetString("guidPlayerID", "");
#endif
            PlayerID = _Value;
            return _Value;
        }
        set
        {
            string _Value = value;
            PlayerID = _Value;
#if UNITY_EDITOR
            PlayerPrefs.SetString("guidPlayerID" + identifier, _Value);
            PlayerPrefs.Save();
#else
           PlayerPrefs.SetString("guidPlayerID", _Value);
#endif

        }
    }
    private static API_AWS instance;
    public static API_AWS Instance()
    {
        if (instance == null) instance = FindObjectOfType<API_AWS>();
        return instance;
    }
    private void Awake()
    {
        instance = this;
        if (GUID_PlayerID == "")
        {
            Guid guid = Guid.NewGuid();
            GUID_PlayerID = guid.ToString();
            ///SetPlayerData(GUID_PlayerID, 0);
        }
        else
        {
            string id = GUID_PlayerID;
        }
    }
    private void Start()
    {
        //GetPlayerData("Player001");
        ///SetPlayerData("PlayerLudo", 300);
        //UpdatePlayerData("PlayerLudo", 25);
        //DeletePlayerData("PlayerLudo");

        //StartCoroutine(IEnumeratorGetPlayerData("b0171a46-82b0-41f8-b11c-602b1ef8f5a1"));
    }
    public async void DeletePlayerData(string PlayerID = "Player107")
    {
        string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={PlayerID}&state={State.delete}";
        using (var httpClient = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[0]);
            var response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                if (json == "") json = "Deleted";
                Debug.Log("JSON: POST " + json);

                var data = JsonConvert.DeserializeObject<WebResponseAPI>(json);
                PlayerData = data;

            }
            else
            {
                Debug.LogError("JSON: " + response.ReasonPhrase);
                Debug.LogError("JSON: " + response.RequestMessage.ToString());
            }
        }
    }
    public void UpdatePlayerData(string PlayerID = "Player105", int UpdateTokenValue = 130, Action<AWSData> OnComplete = null)
    {
        //string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={PlayerID}&UpdateTokenIndex={UpdateTokenValue}&state={State.update}";
        //using (var httpClient = new HttpClient())
        //{
        //    var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[0]);
        //    var response = await httpClient.PostAsync(url, content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string json = await response.Content.ReadAsStringAsync();
        //        if (json == "") json = "Updated";
        //        Debug.Log("JSON: POST " + json);

        //        PlayerData = JsonConvert.DeserializeObject<WebResponseAPI>(json);
        //        OnComplete?.Invoke(PlayerData.UpdatedData);
        //    }
        //    else
        //    {
        //        Debug.LogError("JSON: " + response.ReasonPhrase);
        //        Debug.LogError("JSON: " + response.RequestMessage.ToString());
        //    }
        //}
        StartCoroutine(IEnumeratorUpdatePlayer(PlayerID, UpdateTokenValue, OnComplete));
    }
    private IEnumerator IEnumeratorUpdatePlayer(string PlayerID = "Player105", int UpdateTokenValue = 130, Action<AWSData> OnComplete = null)
    {
        string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={PlayerID}&UpdateTokenIndex={UpdateTokenValue}&state={State.update}";
        Dictionary<string, string> JsonBody = new Dictionary<string, string>();
        using (UnityWebRequest request = UnityWebRequest.Post(url, JsonBody))
        {
            //request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            //request.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token");
            //request.SetRequestHeader("Access-Control-Allow-Credentials", true.ToString());
            //request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            //request.SetRequestHeader("Access-Control-Allow-Credentials", "false");
            //request.SetRequestHeader("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            //request.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("JSON: " + request.error);
            }
            else
            {
                if (request.downloadHandler == null)
                {
                    Debug.LogError("JSON: DownloadHander is null");
                    yield break;
                }
                Debug.Log("JSON: " + request.downloadHandler.text);
                string json = request.downloadHandler.text;
                PlayerData = JsonConvert.DeserializeObject<WebResponseAPI>(json);
                OnComplete?.Invoke(PlayerData.UpdatedData);
            }
        }
    }

    public void SetPlayerData(string newPlayerID = "Player107", int newTokenValue = 114)
    {
        //string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={newPlayerID}&TokenIndex={newTokenValue}&state={State.add}";
        //using (var httpClient = new HttpClient())
        //{
        //    var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[0]);
        //    var response = await httpClient.PostAsync(url, content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string json = await response.Content.ReadAsStringAsync();
        //        if (json == "") json = "Successfully";
        //        Debug.Log("JSON: POST " + json);
        //    }
        //    else
        //    {
        //        Debug.LogError("JSON: " + response.ReasonPhrase);
        //        Debug.LogError("JSON: " + response.RequestMessage.ToString());
        //    }
        //}
        StartCoroutine(IEnumeratorSetPlayerData(newPlayerID, newTokenValue));
    }

    private IEnumerator IEnumeratorSetPlayerData(string newPlayerID = "Player107", int newTokenValue = 114)
    {
        string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={newPlayerID}&TokenIndex={newTokenValue}&state={State.add}";

        Dictionary<string, string> JsonBody = new Dictionary<string, string>();
        using (UnityWebRequest request = UnityWebRequest.Post(url, JsonBody))
        {
            //request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            //request.SetRequestHeader("Access-Control-Allow-Credentials", "false");
            //request.SetRequestHeader("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            //request.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("JSON: " + request.error);
            }
            else
            {
                if (request.downloadHandler == null)
                {
                    Debug.LogError("JSON: DownloadHander is null");
                    yield break;
                }
                Debug.Log("JSON: " + request.downloadHandler.text);
                string json = request.downloadHandler.text;
            }
        }
    }
    public void GetPlayerData(string PlayerID = "Abc", Action<AWSData> OnComplete = null)
    {
        //string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={PlayerID}";
        //using (var httpClient = new HttpClient())
        //{
        //    var response = await httpClient.GetAsync(url);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string json = await response.Content.ReadAsStringAsync();
        //        Debug.Log("JSON: " + json);
        //        var data = JsonConvert.DeserializeObject<WebResponseAPI>(json);
        //        PlayerData = data;
        //        OnComplete?.Invoke(PlayerData.FetchData);
        //        if (data.FetchData == null || string.IsNullOrEmpty(data.FetchData.PlayerID))
        //        {
        //            Debug.LogError("JSON: ID Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("JSON: GET " + response.ReasonPhrase);
        //    }
        //}
        IEnumerator GetPD = IEnumeratorGetPlayerData(PlayerID, OnComplete);
        StartCoroutine(GetPD);
    }

    private IEnumerator IEnumeratorGetPlayerData(string PlayerID = "Abc", Action<AWSData> OnDataFetched = null)
    {
        string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={PlayerID}&state={State.get}";
        Dictionary<string, string> JsonBody = new Dictionary<string, string>();
        using (UnityWebRequest request = UnityWebRequest.Post(url, JsonBody))
        {
            //request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            //request.SetRequestHeader("Access-Control-Allow-Credentials", "false");
            //request.SetRequestHeader("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            //request.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("JSON: " + request.error);
            }
            else
            {
                if (request.downloadHandler == null)
                {
                    Debug.LogError("JSON: DownloadHander is null");
                    yield break;
                }
                Debug.Log("JSON: " + request.downloadHandler.text);
                string json = request.downloadHandler.text;
                var data = JsonConvert.DeserializeObject<WebResponseAPI>(json);
                PlayerData = data;
                OnDataFetched?.Invoke(PlayerData.FetchData);
            }
        }
    }

    //IEnumerator SetPlayerDataV1(string newPlayerID = "Player102", int newTokenValue = 112)
    //{
    //    string url = $"https://mindweox41.execute-api.eu-north-1.amazonaws.com/Prod/ludoapi?PlayerID={newPlayerID}&TokenIndex={newTokenValue}";
    //    string dataJson = ""; //$"'PlayerID':{newPlayerID}, 'TokenIndex':{newTokenValue}";

    //    using (UnityWebRequest request = UnityWebRequest.Post(url, dataJson))
    //    {
    //        request.SetRequestHeader("Content-Type", "application/json");
    //        yield return request.SendWebRequest();
    //        if (request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError("JSON: " + request.error);
    //        }
    //        else
    //        {
    //            if (request.downloadHandler == null)
    //            {
    //                Debug.LogError("JSON: DownloadHander is null");
    //                yield break;
    //            }
    //            Debug.Log("JSON: " + request.downloadHandler.text);
    //        }
    //    }
    //}
}

[System.Serializable]
public class AWSData
{
    public int TokenIndex;
    public string PlayerID;
}
[System.Serializable]
public class Metadata
{
    public int httpStatusCode;
    public string requestId;
    public int attempts;
    public int totalRetryDelay;
}
[System.Serializable]
public class WebResponseAPI
{
    [JsonProperty("$metadata")]
    public Metadata metadata;
    [JsonProperty("Item")]
    public AWSData FetchData;
    [JsonProperty("Attributes")]
    public AWSData UpdatedData;
}

