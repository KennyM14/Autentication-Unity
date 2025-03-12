using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com/"; 
    string Token; 
    string Username; 

    void Start()
    {
        Token = PlayerPrefs.GetString("token"); 
        Username = PlayerPrefs.GetString("username");

        if(string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Username))
        {
            Debug.Log("No hay token"); 
        }
        else
        {
            StartCoroutine("GetProfile"); 
        }
    }

    void Update()
    {
        
    }

    public void Login()
    {
        Credentials credentials = new Credentials(); 
        credentials.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text; 
        credentials.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text; 
        string postData = JsonUtility.ToJson(credentials); 
        StartCoroutine(LoginPost(postData)); 
    }

    public void Register()
    {
        Credentials credentials = new Credentials(); 
        credentials.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text; 
        credentials.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text; 
        string postData = JsonUtility.ToJson(credentials); 
        StartCoroutine(RegisterPost(postData)); 
    }

    IEnumerator RegisterPost(string postData)
    {
        string path = "/api/usuarios";  
        UnityWebRequest www = UnityWebRequest.PostWwwForm(url+path, postData); 
        www.method = "POST"; 
        www.SetRequestHeader("Content-Type", "application/json"); 
        yield return www.SendWebRequest(); 

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error); 
        }
        else
        {
            if(www.responseCode == 200)
            {
                Debug.Log(www.downloadHandler.text); 
                StartCoroutine(LoginPost(postData));
            }
            else
            {
                string mensaje = "status" + www.responseCode; 
                mensaje += "\nError: " + www.downloadHandler.text;  
                Debug.Log(mensaje); 
            }
        }
    }

    IEnumerator LoginPost(string postData)
    {
        string path = "/api/auth/login"; 
        Debug.Log(postData); 
        UnityWebRequest www = UnityWebRequest.PostWwwForm(url+path, postData); 
        www.method = "POST"; 
        www.SetRequestHeader("Content-Type", "application/json"); 
        yield return www.SendWebRequest(); 

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error); 
        }
        else
        {
            if(www.responseCode == 200)
            {
                string json = www.downloadHandler.text; 
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
                GameObject.Find("PanelAuth").SetActive(false); 
                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.usuario.username);

            }
            else
            {
                string mensaje = "status" + www.responseCode; 
                mensaje += "\nError: " + www.downloadHandler.text; 
                Debug.Log(mensaje); 
            }
        }
    }

    IEnumerator GetProfile()
    {
        string path = "/api/usuarios"; 
        UnityWebRequest www = UnityWebRequest.Get(url + path); 
        www.SetRequestHeader("x-token", Token); 
        yield return www.SendWebRequest(); 

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error); 
        }
        else
        {
            if(www.responseCode == 200)
            {
                string json = www.downloadHandler.text; 
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
                GameObject.Find("PanelAuth").SetActive(false); 
            }
            else
            {
                Debug.Log("Token vencido... redireccionar a Login"); 
            }
        }
    }

    public class Credentials
    {
        public string username; 
        public string password; 
    }

    [System.Serializable]
    public class AuthResponse
    {
        public UserModel usuario; 
        public string token; 
    }

    public class UserModel
    {
        public string _id;
        public string username; 
        public bool estado;
    }
}
