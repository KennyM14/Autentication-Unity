using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";
    string token;
    string username;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText; 

    void Start()
    {
        token = PlayerPrefs.GetString("token");
        username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
        {
            Debug.Log("No hay token almacenado.");
        }
        else
        {
            Debug.Log("Token almacenado: " + token);
            Debug.Log("Usuario almacenado: " + username);
            StartCoroutine(GetProfile());
        }
    }

    public void Login()
    {
        Credentials credentials = new Credentials
        {
            username = usernameInput.text,
            password = passwordInput.text
        };
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine(LoginPost(postData));
    }

    public void Register()
    {
        Credentials credentials = new Credentials
        {
            username = usernameInput.text,
            password = passwordInput.text
        };
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine(RegisterPost(postData));
    }

    IEnumerator RegisterPost(string postData)
    {
        string path = "/api/usuarios";
        UnityWebRequest request = new UnityWebRequest(url + path, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro exitoso.");
            feedbackText.text = "Registro exitoso, iniciando sesión...";
            feedbackText.color = Color.green;

            StartCoroutine(LoginPost(postData)); // Auto-login después de registro
        }
        else
        {
            Debug.Log("Error en registro: " + request.downloadHandler.text);
            feedbackText.text = "Error en registro: " + request.downloadHandler.text;
            feedbackText.color = Color.red;
        }
    }

    IEnumerator LoginPost(string postData)
    {
        string path = "/api/auth/login";
        UnityWebRequest request = new UnityWebRequest(url + path, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);

            PlayerPrefs.SetString("token", response.token);
            PlayerPrefs.SetString("username", response.usuario.username);
            token = response.token;
            username = response.usuario.username;

            Debug.Log("Login exitoso!");
            feedbackText.text = "Inicio de sesión exitoso";
            feedbackText.color = Color.green;
        }
        else
        {
            Debug.Log("Error en login: " + request.downloadHandler.text);
            feedbackText.text = "Error en login: " + request.downloadHandler.text;
            feedbackText.color = Color.red;
        }
    }

    IEnumerator GetProfile()
    {
        string path = "/api/usuarios";
        UnityWebRequest request = UnityWebRequest.Get(url + path);
        request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Usuario autenticado correctamente.");
        }
        else
        {
            Debug.Log("Token inválido, redirigiendo a login.");
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("username");
            feedbackText.text = "Sesión expirada, inicia sesión nuevamente";
            feedbackText.color = Color.red;
        }
    }

    [Serializable]
    public class Credentials
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class AuthResponse
    {
        public UserModel usuario;
        public string token;
    }

    [Serializable]
    public class UserModel
    {
        public string _id;
        public string username;
        public bool estado;
    }
}
