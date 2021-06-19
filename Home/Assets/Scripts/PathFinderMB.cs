using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinderMB : MonoBehaviour
{
    private static PathFinderMB instance;
    public static PathFinderMB Instance { get { return instance; } }

    PathRequestManagerMB m_requestManager;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        m_requestManager = GetComponent<PathRequestManagerMB>();
    }

    /// <summary>
    /// initiates a coroutine to calculate a A* path
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }


    /// <summary>
    /// Coroutine to calc path
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    IEnumerator FindPath(Vector3 start, Vector3 end)
    {
        

        List<Node> res = null;
        bool pathSuccess = false;


        Node startNode = GridMB.Instance.GetNodeFromPosition(start);
        Node targetNode = GridMB.Instance.GetNodeFromPosition(end);

        Heap<Node> openSet = new Heap<Node>(GridMB.Instance.MaxHeapSize());
        List<Node> closeSet = new List<Node>();

        openSet.Add(startNode);

        int cnt = 0;

        int largestOpenSet = 0;

        while (openSet.Count > 0)
        {
            cnt++;
            if (cnt > 5000)
            {
                print("failed to find path");
                break;
            }

            if(openSet.Count > largestOpenSet)
            {
                largestOpenSet = openSet.Count;
            }

            // find lowest fCost
            Node current = openSet.RemoveFirst();
            closeSet.Add(current);

            // found target
            if(current == targetNode)
            {
                pathSuccess = true;
                res = CreatePathList(startNode, targetNode);
                break;
            }

            // look at current node's adjacent nodes
            List<Node> adjacentNodes = current.GetOpenAdjacentNodes();
       
            foreach (Node neighbour in adjacentNodes)
            {
                if (closeSet.Contains(neighbour) == false)
                {
                    int moveCost = current.gCost + GetGridDistanceBetweenNodes(current, neighbour);
                   
                    if (moveCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = moveCost;
                        neighbour.hCost = GetGridDistanceBetweenNodes(neighbour, targetNode);
                        neighbour.m_pathParent = current;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;

        m_requestManager.FinishedProcessingPath(res, pathSuccess);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int GetGridDistanceBetweenNodes(Node a, Node b)
    {
        int dx = Mathf.Abs(b.m_xGridPos - a.m_xGridPos);
        int dy = Mathf.Abs(b.m_zGridPos - a.m_zGridPos);

        int diagonalSteps = 0;
        int straightSteps = 0;

        if (dy < dx)
        {
            diagonalSteps = dy;
            straightSteps = dx - dy;
        }
        else
        {
            diagonalSteps = dx;
            straightSteps = dy - dx;
        }

        return diagonalSteps * 14 + straightSteps * 10; // 10/10/14 R.A.Triangle
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    List<Node> CreatePathList(Node startNode, Node targetNode)
    {
        List<Node> res = new List<Node>();

        Node cur = targetNode;
        while(cur != startNode)
        {
            res.Add(cur);
            cur = cur.m_pathParent;
        }

        res.Reverse(); // so it 

        // res = SimplifyPath(res);

        return res;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    List<Node> SimplifyPath(List<Node> path)
    {
        if (path.Count <= 1) return path;

        List<Node> res = new List<Node>();
        Vector3 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 directionNew = path[i].m_pos - path[i - 1].m_pos;
  
            if (directionNew != directionOld)
            {
                res.Add(path[i]);
            }
            directionOld = directionNew;
        }

        print("simpified path: " + path.Count + " -> " + res.Count);
        return res;
    }


    void PrintPath(Node startNode, List<Node> path)
    {
        string s = "\n ------------------------------- \n";
        s += " " + startNode.m_xGridPos + ", " + startNode.m_zGridPos + "\n";
        foreach (Node n in path)
        {
            s += " " + n.m_xGridPos + ", " + n.m_zGridPos + "\n";
        }
        s += "------------------------------- \n";

        print(s);
    }

}
 