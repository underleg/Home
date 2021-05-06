using System.Collections;
using UnityEngine;

public class Node
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



}