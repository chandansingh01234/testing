using UnityEngine;
using TMPro;
using System.Collections;

public class Attacker : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 1.5f;       // Speed when chasing the Ball
    public float carryingSpeed = 0.75f;    // Speed when carrying the Ball
    public float reactivateTime = 2.5f;   // Time before reactivation after being caught

    [Header("Visual Effects")]
    public GameObject catchEffect;
    public ParticleSystem reactivateEffect;
    public GameObject visualModel;

    [Header("Audio")]
    public AudioSource movementAudio;

    private GameObject ball;               // Reference to the Ball
    private bool isHoldingBall = false;    // Flag to check if the Attacker is holding the Ball
    private bool isInactivated = false;    // Flag to check if the Attacker is inactivated
    private Vector3 lastPosition;
    private float movementThreshold = 0.1f;

    void Start()
    {
        // Find the Ball in the scene
        ball = GameObject.FindWithTag("Ball");
        lastPosition = transform.position;

        if (ball == null)
        {
            Debug.LogError("Attacker: Ball not found!");
        }
    }

    void Update()
    {
        if (isInactivated) return; // Skip update if inactivated

        UpdateMovement();
        UpdateAudio();
        UpdateStatusText();
    }

    void UpdateMovement()
    {
        if (isHoldingBall)
        {
            catchEffect.SetActive(true);
            MoveTowardsEnemyGate(carryingSpeed);
        }
        else
        {
            catchEffect.SetActive(false);
            ChaseBall(normalSpeed);
        }
    }

    void UpdateAudio()
    {
        if (movementAudio != null)
        {
            float movement = Vector3.Distance(transform.position, lastPosition);
            movementAudio.volume = movement > movementThreshold ? 0.5f : 0f;
        }
        lastPosition = transform.position;
    }

    void UpdateStatusText()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.statusText.text = isHoldingBall ? "Carrying Ball!" : "Chasing Ball!";
        }
    }

    void ChaseBall(float speed)
    {
        if (ball != null)
        {
            float distanceToBall = Vector3.Distance(transform.position, ball.transform.position);
            if (distanceToBall > 0.1f) // Adjust the threshold as needed
            {
                Vector3 direction = (ball.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(ball.transform);
            }
        }
    }

    void MoveTowardsEnemyGate(float speed)
    {
        GameObject enemyGate = GameObject.FindWithTag("EnemyGate");
        if (enemyGate != null)
        {
            Vector3 direction = (enemyGate.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(enemyGate.transform);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInactivated) return;

        if (other.CompareTag("Ball"))
        {
            PickUpBall(other.gameObject);
        }

        if (other.CompareTag("EnemyGate") && isHoldingBall)
        {
            // Player wins the match
            GameManager.Instance.WinGame();
            Debug.Log("Attacker reached the Enemy Gate! Player wins!");
        }

        if (other.CompareTag("Defender"))
        {
            Debug.Log("Attacker caught by Defender!");
            HandleDefenderCollision();
        }
    }

    void HandleDefenderCollision()
    {
        if (isHoldingBall)
        {
            PassBallToNearestAttacker();
            DropBall();
        }
        Inactivate();
    }

    public void PickUpBall(GameObject ballToPickup)
    {
        if (ballToPickup == null || isHoldingBall) return;

        isHoldingBall = true;
        ball = ballToPickup;
        ball.GetComponent<Ball>()?.SetHolder(gameObject);
        AudioManager.Instance?.PlayBallPickupSound();
    }

    public void DropBall()
    {
        isHoldingBall = false;
        if (ball != null)
        {
            ball.GetComponent<Ball>()?.Release();
            ball = null;
            Debug.Log("Attacker dropped the Ball!");
        }
    }

    public void Inactivate()
    {
        isInactivated = true;
        if (visualModel != null)
        {
            visualModel.SetActive(false);
        }
        Invoke("Reactivate", reactivateTime);
    }

    void Reactivate()
    {
        isInactivated = false;
        if (visualModel != null)
        {
            visualModel.SetActive(true);
        }
        if (reactivateEffect != null)
        {
            reactivateEffect.Play();
        }
        Debug.Log("Attacker is reactivated!");
    }

    public bool IsHoldingBall()
    {
        return isHoldingBall;
    }

    public void PassBallToNearestAttacker()
    {
        if (ball == null)
        {
            Debug.LogError("No ball to pass.");
            return;
        }

        GameObject[] attackers = GameObject.FindGameObjectsWithTag("Attacker");
        GameObject nearestAttacker = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject attacker in attackers)
        {
            if (attacker != this.gameObject) // Exclude self
            {
                Attacker attackerScript = attacker.GetComponent<Attacker>();
                if (attackerScript != null && !attackerScript.IsInactivated()) // Check if the Attacker is active
                {
                    float distance = Vector3.Distance(transform.position, attacker.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestAttacker = attacker;
                    }
                }
            }
        }

        if (nearestAttacker != null)
        {
            Debug.Log("Passing ball to: " + nearestAttacker.name);
            nearestAttacker.GetComponent<Attacker>().PickUpBall(ball);
            ball = null; // Clear the ball reference after passing
            Debug.Log("Ball passed to nearest Attacker: " + nearestAttacker.name);
        }
        else
        {
            Debug.Log("No other Attackers available to pass the Ball to!");
            GameManager.Instance.LoseGame(); // Defender wins the match
        }
    }

    public bool IsInactivated()
    {
        return isInactivated;
    }
}