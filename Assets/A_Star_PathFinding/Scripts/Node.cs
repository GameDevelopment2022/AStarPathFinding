using System;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool isBlocked;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;
    private int heapIndex;

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

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }
}