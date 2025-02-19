using UnityEngine;

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager Instance;

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

    public void PlayerWins()
    {
        Debug.Log("Player Wins: Reached the Enemy's Gate!");
        // Handle win logic
    }

    public void GameOver()
    {
        Debug.Log("Game Over: Time's up!");
        // Handle game over logic
    }
} 