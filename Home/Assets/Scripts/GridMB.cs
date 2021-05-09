using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMB : MonoBehaviour
{
    
    public LayerMask collisionObjects;
    public  float side = 1.0f;
    public  int xSize = 14;
    public  int zSize = 14;
    public bool showGrid = true;
    public bool showPaths = true;

    public GameObject trackObject;

    Node[,] nodes;

    private static GridMB instance;
    public static GridMB Instance { get { return instance; } }

    
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

    // Start is called before the first frame update
    void Start()
    {
        CreateNodes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateNodes()
    {
        nodes = new Node[xSize, zSize];

        Vector3 p = new Vector3(0, 0, 0);
        Vector3 a = new Vector3(side, side, side);
        Vector3 sml = a / 3;
        Vector3 sml2 = sml / 2;
        Vector3 sml3 = sml / 3;


            for (int i = 0; i < xSize; ++i)
        {
            for (int j = 0; j < zSize; ++j)
            {
                Vector3 pos = new Vector3(i * side, 0, j * side);
                bool open = !(Physics.CheckSphere(pos, side / 6, collisionObjects));

                nodes[i, j] = new Node(open, pos, i, j);

                nodes[i, j].m_pathOpen[(int)Node.Direction.N] = CheckPathOpen(i, j, 0, -1);
                nodes[i, j].m_pathOpen[(int)Node.Direction.NE] = CheckPathOpen(i, j, 1, -1);
                nodes[i, j].m_pathOpen[(int)Node.Direction.E] = CheckPathOpen(i, j, 1, 0);
                nodes[i, j].m_pathOpen[(int)Node.Direction.SE] = CheckPathOpen(i, j, 1, 1);
                nodes[i, j].m_pathOpen[(int)Node.Direction.S] = CheckPathOpen(i, j, 0, 1);
                nodes[i, j].m_pathOpen[(int)Node.Direction.SW] = CheckPathOpen(i, j, -1, 1);
                nodes[i, j].m_pathOpen[(int)Node.Direction.W] = CheckPathOpen(i, j, -1, 0);
                nodes[i, j].m_pathOpen[(int)Node.Direction.NW] = CheckPathOpen(i, j, -1, -1);

                nodes[i, j].m_adjacentNodes = new Node[(int)Node.Direction.NUM];

            }
        }

        for (int i = 0; i < xSize; ++i)
        {
            for (int j = 0; j < zSize; ++j)
            {
                if (j > 0)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.N] = nodes[i, j - 1];
                if (j > 0 && i < xSize - 1)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.NE] = nodes[i + 1, j - 1];
                if (i < xSize - 1)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.E] = nodes[i + 1, j];
                if (j < zSize - 1 && i < xSize - 1)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.SE] = nodes[i + 1, j + 1];
                if (j < zSize - 1)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.S] = nodes[i, j + 1];
                if (j < zSize - 1 && i > 0)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.SW] = nodes[i - 1, j + 1];
                if (i > 0)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.W] = nodes[i - 1, j];
                if (j > 0 && i > 0)
                    nodes[i, j].m_adjacentNodes[(int)Node.Direction.NW] = nodes[i - 1, j - 1];
            }
        }


        /* print map
        string s= "-- MAP ---------------------\n";
        for (int j = 0; j < zSize; ++j)
        {
            for (int i = 0; i < xSize; ++i)
            {
                if (nodes[i, j].IsOpen())
                    s += "0";
                else
                    s += "X";
            }
            s += "\n";
        }
        s+="----------------------------\n";
        Debug.Log(s);
        */


    }

    public Node GetNodeFromPosition(Vector3 pos)
    {
        int i = Mathf.RoundToInt(pos.x / side);
        int j = Mathf.RoundToInt(pos.z / side);
        i = Mathf.Clamp(i, 0, xSize - 1); 
        j = Mathf.Clamp(j, 0, zSize - 1);

        return nodes[i, j];
    }

    public Node PickRandomNearbyOpenNode(Node current, int gridRadius)
    {
        Node res = null;

        int sx = Mathf.Clamp(current.m_xGridPos - gridRadius, 0, xSize - 1);
        int sz = Mathf.Clamp(current.m_zGridPos - gridRadius, 0, zSize - 1);

        int ex = Mathf.Clamp(current.m_xGridPos + gridRadius, 0, xSize - 1);
        int ez = Mathf.Clamp(current.m_zGridPos + gridRadius, 0, zSize - 1);


        // compile list of open node nearby
        List<Node> eligibleNodes = new List<Node>();

        for (int i = sx; i <= ex; ++i)
        {
            for (int j = sz; j <= ez; ++j)
            {
                if (nodes[i, j] != current && nodes[i, j].IsOpen())
                {
                    eligibleNodes.Add(nodes[i, j]);
                }
            }
        }

        if (eligibleNodes.Count == 0)
        {
            res = current;
        }
        else
        {
            res = eligibleNodes[Random.Range(0, eligibleNodes.Count - 1)];
        }

        return res;
    }


    void OnDrawGizmos()
    {
        if (showGrid || showPaths)
        {
            Vector3 p = new Vector3(0, 0, 0);
            Vector3 a = new Vector3(side, side, side);
            Vector3 sml = a / 3;
            Vector3 sml2 = sml / 2;
            Vector3 sml3 = sml / 3;

            for (int i = 0; i < xSize; ++i)
            {
                for (int j = 0; j < zSize; ++j)
                {
                    if (showGrid)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(p, a);
                    }

                    if (showPaths)
                    {
                        Gizmos.color = GetAccessColour(i, j, 1, 0);
                        Gizmos.DrawWireCube(p + new Vector3(sml.x, 0, 0), sml2);

                        Gizmos.color = GetAccessColour(i, j, 1, 1, true);
                        Gizmos.DrawWireCube(p + new Vector3(sml.x, 0, sml.z), sml3);

                        Gizmos.color = GetAccessColour(i, j, 0, 1);
                        Gizmos.DrawWireCube(p + new Vector3(0, 0, sml.z), sml2);

                        Gizmos.color = GetAccessColour(i, j, -1, 1, true);
                        Gizmos.DrawWireCube(p + new Vector3(-sml.x, 0, sml.z), sml3);

                        Gizmos.color = GetAccessColour(i, j, -1, 0);
                        Gizmos.DrawWireCube(p + new Vector3(-sml.x, 0, 0), sml2);

                        Gizmos.color = GetAccessColour(i, j, -1, -1, true);
                        Gizmos.DrawWireCube(p + new Vector3(-sml.x, 0, -sml.z), sml3);

                        Gizmos.color = GetAccessColour(i, j, 0, -1);
                        Gizmos.DrawWireCube(p + new Vector3(0, 0, -sml.z), sml2);

                        Gizmos.color = GetAccessColour(i, j, 1, -1, true);
                        Gizmos.DrawWireCube(p + new Vector3(sml.x, 0, -sml.z), sml3);

                     
  
                        if (Physics.CheckSphere(p, sml.x/2, collisionObjects) == true)
                        {
                            Gizmos.color = Color.black;
                            Gizmos.DrawWireSphere(p, sml.x);
                        }                      
                    }

                    p.z += side;
                }
                p.z = 0.0f;
                p.x += side;
            }


            // show track object if set
            if(trackObject != null)
            {
                int i = Mathf.RoundToInt(trackObject.transform.position.x / side);
                int j = Mathf.RoundToInt(trackObject.transform.position.z / side);
                i = Mathf.Clamp(i, 0, xSize); // exclusive of max?
                j = Mathf.Clamp(j, 0, zSize);

                Gizmos.color = Color.black;
                p = new Vector3(i * side, 0.0f, j * side);
                Gizmos.DrawWireCube(p, a);

            }
        }


    }

    Color GetAccessColour(int i, int j, int dx, int dy, bool diagonal = false)
    {

        Color res = Color.green;

        if(CheckPathOpen(i, j, dx, dy) == false)
        {
            res = Color.red;
        }

        return res;
    }

    bool CheckPathOpen(int i, int j, int dx, int dy)
    {
        bool res = true;

        Vector3 start = new Vector3(i * side, 0, j * side);

        if (i + dx < 0 || i + dx >= xSize)
        {
            res = false;
        }
        else if (j + dy < 0 || j + dy >= zSize)
        {
            res = false;
        }
        else if(Physics.CheckSphere(start, side / 6, collisionObjects) == true)
        {
            res = false;
        }
        else
        {
            Vector3 end = new Vector3((i + dx) * side, 0, (j + dy) * side);
            if (Physics.Linecast(start, end, collisionObjects))
            {
                res = false;
            }
        }

        return res;
    }

    public int MaxHeapSize()
    {
        return xSize * zSize;
    }



}
