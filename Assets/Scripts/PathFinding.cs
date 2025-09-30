using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{
    public Transform seeker;
    public Transform target;


    [SerializeField] Grid grid;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            FindPath(seeker.position, target.position);
    }

    private void FindPath(Vector3 start, Vector3 end)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();


        var startNode = grid.NodeFromWorldPoint(start);
        var endNode = grid.NodeFromWorldPoint(end);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);

        // Without Heap
        //List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();


        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.RemoveFirst();

            //WithOut Heap
            // var currentNode = openSet[0];
            /*for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);*/

            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                stopwatch.Stop();
                print($"Path Found in {stopwatch.ElapsedMilliseconds} ms");
                RetracePath(startNode, endNode);
                return;
            }

            foreach (var neighbour in grid.GetNeighbours(currentNode))
            {
                if (neighbour.isBlocked || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    private void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }


    int GetDistance(Node start, Node end)
    {
        int dstX = Mathf.Abs(start.gridX - end.gridX);
        int dstY = Mathf.Abs(start.gridY - end.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnValidate()
    {
        if (grid == null)
            grid = GetComponent<Grid>();
    }
}