using UnityEngine;

public class MazeAttacker : MonoBehaviour
{
    public float speed = 2f;
    private bool hasBall = false;

    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MazeBall"))
        {
            hasBall = true;
            Destroy(other.gameObject);
            Debug.Log("Ball picked up!");
        }

        if (other.CompareTag("EnemyGate") && hasBall)
        {
            MazeGameManager.Instance.PlayerWins();
        }
    }

    public void PickUpBall()
    {
        hasBall = true;
        Debug.Log("Ball picked up by attacker!");
    }
} 