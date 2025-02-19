using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float respawnDelay = 3f;
    public float initialForce = 2f;
    
    [Header("Effects")]
    public ParticleSystem spawnEffect;
    
    private bool isRespawning = false;

    void OnEnable()
    {
        GameEvents.OnBallDrop += HandleBallDrop;
    }

    void OnDisable()
    {
        GameEvents.OnBallDrop -= HandleBallDrop;
    }

    void Start()
    {
        SpawnBall();
    }

    void SpawnBall()
    {
        if (spawnPoint != null && ballPrefab != null)
        {
            // Play spawn effect
            if (spawnEffect != null)
            {
                spawnEffect.Play();
            }

            // Spawn the ball with a small upward force
            GameObject spawnedBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody ballRb = spawnedBall.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.AddForce(Vector3.up * initialForce, ForceMode.Impulse);
            }

            // Play spawn sound
            AudioManager.Instance?.PlaySpawnSound();
            
            Debug.Log("Ball spawned at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("BallSpawner: Spawn point or ball prefab is not assigned!");
        }
    }

    void HandleBallDrop(GameObject ball)
    {
        RequestBallRespawn();
    }

    public void RequestBallRespawn()
    {
        if (!isRespawning)
        {
            isRespawning = true;
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnBall();
        isRespawning = false;
    }
}