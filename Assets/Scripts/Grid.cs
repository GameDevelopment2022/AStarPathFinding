using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask obstacleMask;

    public Vector2 gridSize;
    public float nodeRadius;

    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Start()
    {
        CreateGrid();
    }


    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomPoint =
            transform.position - (Vector3.right * gridSize.x / 2) - (Vector3.forward * gridSize.y / 2);


        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint =
                    worldBottomPoint
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool isBlocked = Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask);
                grid[x, y] = new Node(isBlocked, worldPoint, x, y);
            }
        }
    }

    /*public Node NodeFromWorldPoint(Vector3 worldPoint)
    {
        // Get percent along each axis relative to the grid
        float percentX = (worldPoint.x - gridSize.x) / (gridSizeX * nodeDiameter);
        float percentY = (worldPoint.z - gridSize.y) / (gridSizeY * nodeDiameter);

        // Clamp to 0–1 (in case worldPoint is outside the grid)
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Convert percent into actual grid indices
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }*/
    
    public Node NodeFromWorldPoint(Vector3 worldPoint)
    {
        Vector3 worldBottomPoint =
            transform.position - (Vector3.right * gridSize.x / 2) - (Vector3.forward * gridSize.y / 2);

        float percentX = (worldPoint.x - worldBottomPoint.x) / (gridSize.x);
        float percentY = (worldPoint.z - worldBottomPoint.z) / (gridSize.y);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }


    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    private void OnValidate()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
    }


    public List<Node> path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

        if (grid == null)
            return;

        foreach (Node node in grid)
        {
            Gizmos.color = node.isBlocked ? Color.red : Color.white;
            if (path != null)
                if (path.Contains(node))
                    Gizmos.color = Color.black;

            Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
        }
    }
}