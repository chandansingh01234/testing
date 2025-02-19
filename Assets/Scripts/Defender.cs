using UnityEngine;

public class Defender : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 1.0f;       // Speed when chasing the Attacker
    public float returnSpeed = 2.0f;       // Speed when returning to origin
    public float detectionRange = 5.0f;    // Range to detect Attackers holding the Ball
    public float reactivateTime = 4.0f;   // Time before reactivation after catching an Attacker
    
    [Header("Visual Feedback")]
    public GameObject rangeIndicator;
    public ParticleSystem catchEffect;
    public ParticleSystem reactivateEffect;

    private GameObject targetAttacker;     // Reference to the target Attacker
    private Vector3 originPosition;        // Original position of the Defender
    private bool isChasing = false;        // Flag to check if the Defender is chasing
    private bool isInactivated = false;

    void Start()
    {
        originPosition = transform.position; // Save the original position
        UpdateRangeIndicator();
    }

    void Update()
    {
        if (isInactivated) return;

        if (isChasing)
        {
            ChaseAttacker(normalSpeed);
        }
        else
        {
            DetectAttackers();
            PatrolArea();
        }
    }

    void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = new Vector3(detectionRange * 2, 0.1f, detectionRange * 2);
        }
    }

    void DetectAttackers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Attacker"))
            {
                Attacker attacker = hitCollider.GetComponent<Attacker>();
                if (attacker != null && attacker.IsHoldingBall() && !attacker.IsInactivated())
                {
                    targetAttacker = hitCollider.gameObject;
                    isChasing = true;
                    Debug.Log("Defender detected an Attacker holding the Ball!");
                    break;
                }
            }
        }
    }

    void PatrolArea()
    {
        // Simple patrol behavior when not chasing
        float time = Time.time;
        Vector3 offset = new Vector3(
            Mathf.Sin(time) * 2f,
            0,
            Mathf.Cos(time) * 2f
        );
        Vector3 targetPosition = originPosition + offset;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            normalSpeed * 0.5f * Time.deltaTime
        );
    }

    void ChaseAttacker(float speed)
    {
        if (targetAttacker != null)
        {
            Vector3 direction = (targetAttacker.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(targetAttacker.transform);

            // Check if target is still valid
            Attacker attacker = targetAttacker.GetComponent<Attacker>();
            if (attacker == null || !attacker.IsHoldingBall() || attacker.IsInactivated())
            {
                targetAttacker = null;
                ReturnToOrigin();
            }
        }
        else
        {
            ReturnToOrigin();
        }
    }

    void ReturnToOrigin()
    {
        Vector3 direction = (originPosition - transform.position).normalized;
        transform.position += direction * returnSpeed * Time.deltaTime;
        transform.LookAt(originPosition);

        if (Vector3.Distance(transform.position, originPosition) < 0.1f)
        {
            isChasing = false;
            transform.position = originPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInactivated) return;

        if (other.CompareTag("Attacker"))
        {
            Attacker attacker = other.GetComponent<Attacker>();
            if (attacker != null && attacker.IsHoldingBall() && !attacker.IsInactivated())
            {
                CatchAttacker(attacker);
            }
        }
    }

    void CatchAttacker(Attacker attacker)
    {
        if (catchEffect != null)
        {
            catchEffect.Play();
        }

        attacker.PassBallToNearestAttacker();
        attacker.DropBall();
        attacker.Inactivate();

        targetAttacker = null;
        isChasing = false;
        Inactivate();
    }

    void Inactivate()
    {
        isInactivated = true;
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
        // Greyscale effect can be added here
        Invoke(nameof(Reactivate), reactivateTime);
    }

    void Reactivate()
    {
        isInactivated = false;
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }
        if (reactivateEffect != null)
        {
            reactivateEffect.Play();
        }
        Debug.Log("Defender is reactivated!");
    }

    void OnDrawGizmosSelected()
    {
        // Visualize detection range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}