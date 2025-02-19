using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float matchTime = 140f;
    public int totalMatches = 5;
    
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winLoseText;
    
    [Header("UI Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject drawScreen;
    public GameObject penaltyScreen;
    
    [Header("Dependencies")]
    public EnergyManager energyManager;

    private float currentTime;
    private bool isMatchActive;
    private int playerScore;
    private int enemyScore;
    private int matchesPlayed;

    public event Action OnMatchStart;
    public event Action OnMatchEnd;
    public event Action<bool> OnGameEnd; // true for win, false for lose

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
        StartNewMatch();
    }

    void InitializeGame()
    {
        playerScore = 0;
        enemyScore = 0;
        matchesPlayed = 0;
        UpdateScoreUI();
    }

    void Update()
    {
        if (isMatchActive)
        {
            UpdateMatchTimer();
        }
    }

    void UpdateMatchTimer()
    {
        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0)
        {
            EndMatch(false);
        }
    }

    void StartNewMatch()
    {
        currentTime = matchTime;
        isMatchActive = true;
        HideAllScreens();
        UpdateTimerUI();
        UpdateScoreUI();
        SwitchRoles();
        OnMatchStart?.Invoke();
    }

    void HideAllScreens()
    {
        winScreen?.SetActive(false);
        loseScreen?.SetActive(false);
        drawScreen?.SetActive(false);
        penaltyScreen?.SetActive(false);
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Player: {playerScore} | Enemy: {enemyScore}";
        }
    }

    void SwitchRoles()
    {
        if (matchesPlayed % 2 == 0)
        {
            // Player attacks, Enemy defends
        }
        else
        {
            // Enemy attacks, Player defends
        }
    }

    public void WinGame()
    {
        playerScore++;
        matchesPlayed++;
        EndMatch(true);
        ShowEndScreen(winScreen, "You Win!");
        CheckForGameEnd();
    }

    public void LoseGame()
    {
        enemyScore++;
        matchesPlayed++;
        EndMatch(false);
        ShowEndScreen(loseScreen, "You Lose!");
        CheckForGameEnd();
    }

    public void DrawGame()
    {
        matchesPlayed++;
        EndMatch(false);
        ShowEndScreen(drawScreen, "Draw!");
        CheckForGameEnd();
    }

    void ShowEndScreen(GameObject screen, string message)
    {
        HideAllScreens();
        screen?.SetActive(true);
        if (winLoseText != null)
        {
            winLoseText.text = message;
        }
        UpdateScoreUI();
    }

    void EndMatch(bool isWin)
    {
        isMatchActive = false;
        OnMatchEnd?.Invoke();
    }

    void CheckForGameEnd()
    {
        if (matchesPlayed >= totalMatches)
        {
            HandleGameEnd();
        }
        else
        {
            Invoke(nameof(StartNewMatch), 3f);
        }
    }

    void HandleGameEnd()
    {
        if (playerScore > enemyScore)
        {
            ShowEndScreen(winScreen, "You Win the Game!");
            OnGameEnd?.Invoke(true);
        }
        else if (enemyScore > playerScore)
        {
            ShowEndScreen(loseScreen, "You Lose the Game!");
            OnGameEnd?.Invoke(false);
        }
        else
        {
            StartPenaltyGame();
        }
    }

    void StartPenaltyGame()
    {
        HideAllScreens();
        penaltyScreen?.SetActive(true);
        if (winLoseText != null)
        {
            winLoseText.text = "Penalty Game!";
        }
    }
    public void GameOver()
    {
        Debug.Log("Game Over: Time's up!");
        // Handle game over logic
    }
    public bool IsMatchActive()
    {
        return isMatchActive;
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }

    public (int playerScore, int enemyScore) GetCurrentScore()
    {
        return (playerScore, enemyScore);
    }
}