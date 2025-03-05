using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // 从起点到当前节点的成本
    public int hCost; // 从当前节点到终点的估计成本
    public Node parent;

    public int fCost { get { return gCost + hCost; } }

    public Node(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
