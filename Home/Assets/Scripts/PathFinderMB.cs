using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderMB : MonoBehaviour
{
    private static PathFinderMB instance;
    public static PathFinderMB Instance { get { return instance; } }

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
    }

    public List<Node> FindPath(Vector3 start, Vector3 end)
    {
        List<Node> res = null;

        Node startNode = GridMB.Instance.GetNodeFromPosition(start);
        Node targetNode = GridMB.Instance.GetNodeFromPosition(end);

        List<Node> openSet = new List<Node>();
        List<Node> closeSet = new List<Node>();

        openSet.Add(startNode);

        int cnt = 0;

        while (openSet.Count > 0)
        {
            cnt++;
            if (cnt > 1000 || openSet.Count > 1000 || closeSet.Count > 1000)
            {
                Debug.Log("failed to find path");
                break;
            }

            // find lowest fCost
            Node current = openSet[0];
            for(int i = 1; i < openSet.Count; ++i)
            {
                if(openSet[i].fCost < current.fCost ||
                   (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            
            openSet.Remove(current);
            closeSet.Add(current);

            // found target
            if(current == targetNode)
            {
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

        return res;
   
    }


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

        Debug.Log(s);
    }

}
 