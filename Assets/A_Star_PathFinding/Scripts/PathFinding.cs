using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private PathManager pathManager;
    [SerializeField] Grid grid;


    public void StartFindPath(Vector3 start, Vector3 end)
    {
        StartCoroutine(FindPath(start, end));
    }

    IEnumerator FindPath(Vector3 start, Vector3 end)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Vector3[] wayPoints = new Vector3[] { };
        bool pathFound = false;


        var startNode = grid.NodeFromWorldPoint(start);
        var endNode = grid.NodeFromWorldPoint(end);

        if (!startNode.isBlocked && !endNode.isBlocked)
        {
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
                    pathFound = true;

                    break;
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
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;

        if (pathFound)
        {
            wayPoints = RetracePath(startNode, endNode);
        }

        pathManager.FinishedProcessingPath(wayPoints, pathFound);
    }

    private Vector3[] RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        List<Vector3> wayPoints = SimplifyPath(path);

        wayPoints.Reverse();

        return wayPoints.ToArray();
    }

    List<Vector3> SimplifyPath(List<Node> path)
    {
        List<Vector3> pathPoints = new List<Vector3>();

        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

            if (directionNew != directionOld)
            {
                pathPoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }


        return pathPoints;
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
        if (pathManager == null)
            pathManager = GetComponent<PathManager>();
    }
}