using UnityEngine;
using System.Collections.Generic;
// Add the namespace if applicable
// using YourNamespace;

public class MazeBall : MonoBehaviour
{
    public ParticleSystem pickupEffect;
    public MazeGenerator mazeGenerator;

    void Start()
    {
        PlaceBall();
    }

    void PlaceBall()
    {
        List<Vector2Int> pathPositions = new List<Vector2Int>();
        for (int x = 0; x < mazeGenerator.width; x++)
        {
            for (int y = 0; y < mazeGenerator.height; y++)
            {
                if (mazeGenerator.Maze[x, y] == 1)
                {
                    pathPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (pathPositions.Count > 0)
        {
            Vector2Int randomPathPosition = pathPositions[Random.Range(0, pathPositions.Count)];
            transform.position = new Vector3(randomPathPosition.x * mazeGenerator.spacing, 0.5f, randomPathPosition.y * mazeGenerator.spacing);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            other.GetComponent<MazeAttacker>()?.PickUpBall();

            Destroy(gameObject);
        }
    }
}