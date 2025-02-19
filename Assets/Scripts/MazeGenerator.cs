using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;  // Should be odd
    public int height = 21; // Should be odd
    public GameObject wallPrefab;
    public float spacing = 1f;

    private int[,] maze;

    public int[,] Maze
    {
        get { return maze; }
    }

    private Vector2Int[] directions = { 
        Vector2Int.up, 
        Vector2Int.right, 
        Vector2Int.down, 
        Vector2Int.left 
    };

    void Start()
    {
        // Ensure dimensions are odd
        width = width % 2 == 0 ? width + 1 : width;
        height = height % 2 == 0 ? height + 1 : height;
        
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];
        
        // Initialize all cells as walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 0; // 0 = wall, 1 = path
            }
        }

        // Use recursive backtracking to generate the maze
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int startPos = new Vector2Int(1, 1);
        maze[startPos.x, startPos.y] = 1;
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(current);

            if (unvisitedNeighbors.Count > 0)
            {
                // Choose random unvisited neighbor
                Vector2Int chosen = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                
                // Mark the chosen cell and the wall between as path
                maze[chosen.x, chosen.y] = 1;
                maze[(chosen.x + current.x) / 2, (chosen.y + current.y) / 2] = 1;
                
                stack.Push(chosen);
            }
            else
            {
                stack.Pop();
            }
        }

        // Create entrance and exit
        CreateEntranceAndExit();
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir * 2;
            if (IsInBounds(neighbor) && maze[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    void CreateEntranceAndExit()
    {
        // Create entrance at bottom center
        int entranceX = width / 2;
        maze[entranceX, 0] = 1;
        maze[entranceX, 1] = 1;

        // Create exit at top center
        maze[entranceX, height - 1] = 1;
        maze[entranceX, height - 2] = 1;
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Place wall if:
                // 1. Cell is a wall (0)
                // 2. Cell is on the boundary (except entrance/exit)
                if (maze[x, y] == 0 || 
                    (x == 0 && y != height/2) || 
                    (y == 0 && x != width/2) || 
                    (x == width - 1 && y != height/2) || 
                    (y == height - 1 && x != width/2))
                {
                    Vector3 position = new Vector3(
                        x * spacing, 
                        0, 
                        y * spacing
                    );
                    
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x > 0 && pos.x < width - 1 && 
               pos.y > 0 && pos.y < height - 1;
    }
}