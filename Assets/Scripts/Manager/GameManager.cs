// using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController playerController;
    public TowerController towerController;
    public static GameManager Instance;
    public TMP_Text ScoreText;
    public GameObject GameOverPanel;
    public TMP_Text scoreOnGameOverText;
    public TMP_Text highScoreOnGameOverText;
    public GameObject StartGamePanel;
    public bool gameStarted = false;
    public float playerScore = 0;
    private float playerHighScore = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerHighScore = PlayerPrefs.GetFloat("playerHighScore", 0);

        if (StartGamePanel != null)
        {
            StartGamePanel.SetActive(true);
        }

        playerController.enabled = false;
    }

    public void AddAndUpdateScore(float scoreToAdd)
    {
        ManageDifficulty();
        playerScore += scoreToAdd;
        ScoreText.text = $"SCORE: {playerScore}";
        if (playerScore > playerHighScore)
        {
            playerHighScore = playerScore;
            PlayerPrefs.SetFloat("playerHighScore", playerScore);
        }
    }

    public void StartGame()
    {
        gameStarted = true;

        if (StartGamePanel != null)
        {
            StartGamePanel.SetActive(false);
        }
        playerController.enabled = true;
    }

    void ManageDifficulty()
    {
        if (playerController.speed < 6)
        {
            playerController.speed += 0.1f;
        }
    }

    public void OnGameOver()
    {
        GameOverPanel.SetActive(true);
        scoreOnGameOverText.text = $"SCORE: {playerScore}";
        highScoreOnGameOverText.text = $"HIGHSCORE: {playerHighScore}";
    }

    public void RestartGame()
    {
        // Debug.Log("restart button clicked");
        SceneManager.LoadScene(SceneManager.GetSceneByName("SpinTowerGame").buildIndex);
    }
}
