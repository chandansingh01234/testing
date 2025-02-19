using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Components")]
    public ParticleSystem passEffect;
    public Animator ballAnimator;
    public AudioSource passSound;
    public TrailRenderer trailRenderer;
    
    [Header("Settings")]
    public float outOfBoundsY = -10f;
    public float bounceForce = 5f;
    
    private GameObject currentHolder;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (currentHolder != null)
        {
            transform.position = currentHolder.transform.position;
        }
        
        // Check if ball is out of bounds
        if (transform.position.y < outOfBoundsY)
        {
            OnOutOfBounds();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentHolder == null && rb != null)
        {
            // Add bounce effect when hitting surfaces
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }

    public void SetHolder(GameObject holder)
    {
        if (currentHolder != null && holder != currentHolder)
        {
            PlayPassEffects();
        }

        currentHolder = holder;
        rb.isKinematic = true; // Disable physics while being held
        Debug.Log($"Ball is now held by: {holder.name}");

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }
        
        GameEvents.BallPickup(holder);
    }

    public void Release()
    {
        currentHolder = null;
        rb.isKinematic = false; // Re-enable physics
        Debug.Log("Ball has been released!");

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
        
        GameEvents.BallDrop(gameObject);
    }

    private void PlayPassEffects()
    {
        if (passEffect != null)
        {
            passEffect.Play();
        }

        if (ballAnimator != null)
        {
            ballAnimator.SetTrigger("Pass");
        }

        if (passSound != null)
        {
            passSound.Play();
        }
    }

    private void OnOutOfBounds()
    {
        GameEvents.BallDrop(gameObject); // Notify that the ball was dropped
        Destroy(gameObject);
    }

    public GameObject GetCurrentHolder()
    {
        return currentHolder;
    }
}
