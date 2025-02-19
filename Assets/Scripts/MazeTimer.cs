using UnityEngine;
using TMPro;

public class MazeTimer : MonoBehaviour
{
    public float timeLimit = 180f; // 3 minutes
    public TextMeshProUGUI timeText;

    private float timeRemaining;
    private bool isRunning = false;

    void Start()
    {
        timeRemaining = timeLimit;
        isRunning = true;
    }

    void Update()
    {
        if (isRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimeText();
            }
            else
            {
                timeRemaining = 0;
                isRunning = false;
                MazeGameManager.Instance.GameOver();
            }
        }
    }

    void UpdateTimeText()
    {
        timeText.text = "Time Left: " + Mathf.Ceil(timeRemaining).ToString() + "s";
    }
} 