using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public Slider playerEnergyBar; // Player's energy bar
    public Slider enemyEnergyBar;  // Enemy's energy bar
    public TextMeshProUGUI playerEnergyText; // Player's energy text (optional)
    public TextMeshProUGUI enemyEnergyText;  // Enemy's energy text (optional)

    public float energyRegenRate = 0.5f;  // Energy regeneration rate per second
    public float maxEnergy = 6f;         // Maximum energy points

    private float playerEnergy;           // Player's current energy
    private float enemyEnergy;            // Enemy's current energy

    void Start()
    {
        playerEnergy = 0f;
        enemyEnergy = 0f;

        playerEnergyBar.maxValue = maxEnergy;
        enemyEnergyBar.maxValue = maxEnergy;

        playerEnergyBar.value = playerEnergy;
        enemyEnergyBar.value = enemyEnergy;

        UpdateEnergyUI();
    }

    void Update()
    {
        // Regenerate energy for both Player and Enemy
        if (playerEnergy < maxEnergy)
        {
            playerEnergy += energyRegenRate * Time.deltaTime;
            playerEnergyBar.value = playerEnergy;
        }

        if (enemyEnergy < maxEnergy)
        {
            enemyEnergy += energyRegenRate * Time.deltaTime;
            enemyEnergyBar.value = enemyEnergy;
        }

        UpdateEnergyUI();
    }

    public bool SpendEnergy(bool isPlayer, float cost)
    {
        if (isPlayer)
        {
            if (playerEnergy >= cost)
            {
                playerEnergy -= cost;
                playerEnergyBar.value = playerEnergy;
                UpdateEnergyUI();
                return true;
            }
        }
        else
        {
            if (enemyEnergy >= cost)
            {
                enemyEnergy -= cost;
                enemyEnergyBar.value = enemyEnergy;
                UpdateEnergyUI();
                return true;
            }
        }
        return false;
    }

    void UpdateEnergyUI()
    {
        if (playerEnergyText != null)
        {
            playerEnergyText.text = "Player Energy: " + Mathf.CeilToInt(playerEnergy).ToString();
        }

        if (enemyEnergyText != null)
        {
            enemyEnergyText.text = "Enemy Energy: " + Mathf.CeilToInt(enemyEnergy).ToString();
        }
    }
}