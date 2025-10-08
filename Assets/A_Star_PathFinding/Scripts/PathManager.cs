using System;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public Queue<PathRequest> pathRequests = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    static PathManager instance;
    [SerializeField] private PathFinding _pathFinding;
    [SerializeField] bool isProcessingPath = false;

    private void OnValidate()
    {
        _pathFinding = GetComponent<PathFinding>();
    }

    private void Awake()
    {
        instance = this;
    }

    public static void RequestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        instance.pathRequests.Enqueue(newRequest);
        instance.TryProcessNextRequest();
    }

    private void TryProcessNextRequest()
    {
        if (!isProcessingPath && pathRequests.Count > 0)
        {
            currentPathRequest = pathRequests.Dequeue();
            isProcessingPath = true;
            _pathFinding.StartFindPath(currentPathRequest.start, currentPathRequest.end);
        }
    }


    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
    }

    public struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
        {
            this.start = start;
            this.end = end;
            this.callback = callback;
        }
    }
}