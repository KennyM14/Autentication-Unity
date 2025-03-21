using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    
    private string usuariosUrl = "https://sid-restapi.onrender.com/api/usuarios";
    private string Token;
    
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private TextMeshProUGUI[] nombreTexts;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        if (!string.IsNullOrEmpty(Token))
        {
            StartCoroutine(GetLeaderboard());
        }
        else
        {
            Debug.LogWarning("No se encontró un token válido.");
        }
    }

    IEnumerator GetLeaderboard()
    {
        UnityWebRequest request = UnityWebRequest.Get(usuariosUrl);
        request.SetRequestHeader("Authorization", "Bearer " + Token);
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            UserList userList = JsonUtility.FromJson<UserList>("{\"usuarios\":" + jsonResponse + "}");

            if (userList != null && userList.usuarios != null)
            {
                List<UserModel> sortedUsers = userList.usuarios.OrderByDescending(u => u.score).Take(5).ToList();
                DisplayLeaderboard(sortedUsers);
            }
            else
            {
                Debug.LogError("Error al deserializar los datos del leaderboard.");
            }
        }
        else
        {
            Debug.LogError("Error al obtener datos del leaderboard: " + request.error);
        }
    }

    void DisplayLeaderboard(List<UserModel> users)
    {
        for (int i = 0; i < nombreTexts.Length; i++)
        {
            if (i < users.Count)
            {
                nombreTexts[i].text = users[i].username;
                scoreTexts[i].text = users[i].score.ToString();
            }
            else
            {
                nombreTexts[i].text = "-";
                scoreTexts[i].text = "-";
            }
        }
    }
}
