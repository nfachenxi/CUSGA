using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Combat,//战斗
    EliteCombat,//精英战斗
    Shop,//商店
    Treasure,//宝箱
    Mystery,//未知
    //Rest,//休息
    Boss,//boss
}

[System.Serializable]
public class Node
{
    public NodeType nodeType;
    public Vector2 position;//表示节点在地图中的坐标（x为层数，y为该层中的位置）
    public List<Node> children = new List<Node>();//存储指向该节点下一层可达节点的引用
    public List<Node> parents = new List<Node>();//存储可以到达该节点上一层节点的引用
    public bool isVisited = false;//表示该节点是否被访问过
    public bool isLocked = true;//表示该节点是否被锁定，锁定后不能被访问
}
