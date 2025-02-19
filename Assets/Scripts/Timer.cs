using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeLimit = 180f; // 3 minutes
    public Text timeText;

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
                GameManager.Instance.GameOver();
            }
        }
    }

    void UpdateTimeText()
    {
        timeText.text = "Time Left: " + Mathf.Ceil(timeRemaining).ToString() + "s";
    }
} 