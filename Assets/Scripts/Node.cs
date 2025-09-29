using System;
using UnityEngine;

public class Node
{
    public bool isBlocked;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;
    public int fCost
    {
        get { return gCost + hCost; }
    }


    public Node(bool isBlocked, Vector3 worldPosition, int gridX, int gridY)
    {
        this.isBlocked = isBlocked;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}