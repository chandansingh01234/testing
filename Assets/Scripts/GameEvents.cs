using UnityEngine;
using System;

public static class GameEvents
{
    // Ball events
    public static event Action<GameObject> OnBallPickup;
    public static event Action<GameObject> OnBallDrop;
    
    // Match events
    public static event Action OnMatchStart;
    public static event Action OnMatchEnd;
    public static event Action<bool> OnMatchResult; // true for win, false for loss
    
    // Energy events
    public static event Action<bool, float> OnEnergySpent; // isPlayer, amount
    public static event Action<bool, float> OnEnergyGained; // isPlayer, amount

    // Event trigger methods
    public static void BallPickup(GameObject picker)
    {
        OnBallPickup?.Invoke(picker);
    }

    public static void BallDrop(GameObject dropper)
    {
        OnBallDrop?.Invoke(dropper);
    }

    public static void MatchStart()
    {
        OnMatchStart?.Invoke();
    }

    public static void MatchEnd()
    {
        OnMatchEnd?.Invoke();
    }

    public static void MatchResult(bool isWin)
    {
        OnMatchResult?.Invoke(isWin);
    }

    public static void EnergySpent(bool isPlayer, float amount)
    {
        OnEnergySpent?.Invoke(isPlayer, amount);
    }

    public static void EnergyGained(bool isPlayer, float amount)
    {
        OnEnergyGained?.Invoke(isPlayer, amount);
    }
} 