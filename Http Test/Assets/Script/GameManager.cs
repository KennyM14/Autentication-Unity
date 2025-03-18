using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameOver = false;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverHighScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over!");
            Time.timeScale = 0; 
            gameOverPanel.SetActive(true); 

            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            int finalScore = scoreManager.GetScore();
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalScore > highScore)
            {
                highScore = finalScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }

            gameOverHighScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void Quit()
    {
        Application.Quit(); 
    }

    public void Home()
    {
        SceneManager.LoadScene("LoginScene"); 
    }
}