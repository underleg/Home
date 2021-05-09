using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public enum Direction
    {
        N = 0,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW,
        NUM
    };

    bool m_open = true;

    public Vector3 m_pos;

    public bool[] m_pathOpen; // true = open

    public Node[] m_adjacentNodes;

    public int m_xGridPos;
    public int m_zGridPos;

    public int gCost = 0;
    public int hCost = 0;

    public Node m_pathParent;

    int m_heapIndex;


    public Node(bool open, Vector3 pos, int xPos, int zPos)
    {
        m_pathOpen = new bool[(int)Direction.NUM];
        m_adjacentNodes = new Node[(int)Direction.NUM];


        m_open = open;
        m_pos = pos;
        m_xGridPos = xPos;
        m_zGridPos = zPos;
    }

    public bool IsOpen() { return this.m_open;  }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public List<Node> GetOpenAdjacentNodes()
    {
        List<Node> res = new List<Node>();

        for(int i = 0; i < m_adjacentNodes.Length; ++i)
        {
            if(m_adjacentNodes[i] != null && m_pathOpen[i] == true)
            {
                res.Add(m_adjacentNodes[i]);
            }
        }

        return res;
    }

    public int HeapIndex
    {
        get
        {
            return m_heapIndex;
        }
        set
        {
            m_heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}