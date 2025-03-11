// MapGenerator.cs

using UnityEngine;
using System.Collections.Generic;

public class MapGeneration : MonoBehaviour
{
    public GameObject NodePrefab;

    //public GameObject LinePrefab; // 如果使用LinePrefab
    public Map MapData;
    public int MaxDepth = 7;
    public int MaxWidth = 5;
    public float NodeSpacingX = 2f;
    public float NodeSpacingY = 2f;

    public Dictionary<NodeType, float> NodeTypeWeights = new Dictionary<NodeType, float>()
    {
        { NodeType.Combat, 0.4f },
        { NodeType.EliteCombat, 0.1f },
        { NodeType.Shop, 0.1f },
        { NodeType.Treasure, 0.1f },
        { NodeType.Mystery, 0.2f },
        { NodeType.Rest, 0.1f }
    };

    public Dictionary<Node, NodeUI> NodeUIs = new Dictionary<Node, NodeUI>();
    public GameObject mapRoot;

    void Start()
    {
        GenerateAndDisplayMap();
    }

    public void GenerateAndDisplayMap()
    {
        // 如果已经有地图了，先销毁
        if (mapRoot != null)
        {
            Destroy(mapRoot);
        }

        mapRoot = new GameObject("MapRoot");
        MapData = GenerateMap(MaxDepth, MaxWidth, NodeTypeWeights);
        NodeUIs.Clear();

        // 实例化节点
        foreach (Node node in MapData.nodes)
        {
            GameObject nodeObj = Instantiate(NodePrefab, mapRoot.transform);
            NodeUI nodeUI = nodeObj.GetComponent<NodeUI>();
            nodeUI.Initialize(node, this);
            nodeObj.transform.localPosition =
                new Vector3(node.position.y * NodeSpacingX, -node.position.x * NodeSpacingY, 0);
            NodeUIs.Add(node, nodeUI);
        }

        HighlightAvailablePaths();
    }

    public Map GenerateMap(int maxDepth, int maxWidth, Dictionary<NodeType, float> nodeTypeWeights)
    {
        Map map = new Map();
        map.maxDepth = maxDepth; //赋值最大深度和宽度
        map.maxWidth = maxWidth;

        // 1. 创建起始节点
        Node startNode = CreateNode(NodeType.Rest, new Vector2Int(0, 0)); //初始类型
        startNode.isLocked = false; //初始节点解锁
        map.nodes.Add(startNode); //将初始节点添加到节点列表
        map.currentNode = startNode; //将起始节点设为当前节点

        // 2. 递归生成节点
        for (int depth = 1; depth < maxDepth; depth++) //从第一层开始生成
        {
            int nodeCount = Random.Range(1, maxWidth + 1); //随机生成该层的节点数量（根据权重生成）
            for (int i = 0; i < nodeCount; i++) //为每个节点创建一个Node对象，并设置Positon
            {
                NodeType type = GetRandomNodeType(nodeTypeWeights); //获取随机节点类型
                Node newNode = CreateNode(type, new Vector2Int(depth, i)); //创建节点并生成节点
                map.nodes.Add(newNode); //加进Map节点列表
            }
        }

        // 3. 连接节点
        for (int depth = 0; depth < maxDepth - 1; depth++) //遍历所有深度
        {
            List<Node> currentLayerNodes = GetNodesAtDepth(map, depth); //获取当前层节点
            List<Node> nextLayerNodes = GetNodesAtDepth(map, depth + 1); //获取下一层节点

            foreach (Node currentNode in currentLayerNodes) //遍历当前层节点
            {
                int childCount = Random.Range(1, 4); // 每个节点1-3个子节点
                for (int i = 0; i < childCount; i++) //为每个节点生成子节点
                {
                    if (nextLayerNodes.Count > 0) //如果下一层节点不为空
                    {
                        Node childNode = nextLayerNodes[Random.Range(0, nextLayerNodes.Count)]; //获取下一层节点
                        currentNode.children.Add(childNode); //将子节点添加到当前节点的子节点列表中
                        childNode.parents.Add(currentNode); //将当前节点添加到子节点的父节点列表中
                    }
                }
            }
        }

        // 确保所有节点都可达
        for (int depth = 1; depth < maxDepth; depth++) //遍历所有深度
        {
            List<Node> currentLayerNodes = GetNodesAtDepth(map, depth); //获取当前层节点
            foreach (Node node in currentLayerNodes) //遍历当前层节点
            {
                if (node.parents.Count == 0) //如果父节点为空
                {
                    List<Node> previousLayerNodes = GetNodesAtDepth(map, depth - 1); //获取上一层节点
                    Node randomParent = previousLayerNodes[Random.Range(0, previousLayerNodes.Count)]; //随机获取上一层节点
                    node.parents.Add(randomParent); //将当前节点添加到上一层节点
                    randomParent.children.Add(node); //将上一层节点添加到当前节点的父节点列表中
                }
            }
        }

        // 4. 设置Boss节点
        List<Node> lastLayerNodes = GetNodesAtDepth(map, maxDepth - 1); //获取最后一层节点
        if (lastLayerNodes.Count > 0) //如果最后一层节点不为空
        {
            lastLayerNodes[Random.Range(0, lastLayerNodes.Count)].nodeType = NodeType.Boss; //设置最后一层节点为Boss节点
        }

        return map;
    }

    private Node CreateNode(NodeType type, Vector2Int position)
    {
        Node node = new Node();
        node.nodeType = type;
        node.position = position;
        return node;
    }

    /// <summary>
    /// 根据深度获取该层节点
    /// </summary>
    /// <param name="map"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private List<Node> GetNodesAtDepth(Map map, int depth)
    {
        return map.nodes.FindAll(n => n.position.x == depth); //返回该层节点
    }

    /// <summary>
    /// 根据权重随机生成节点类型
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    private NodeType GetRandomNodeType(Dictionary<NodeType, float> weights)
    {
        float totalWeight = 0; //计算权重总和
        foreach (var weight in weights.Values) //遍历权重字典
        {
            totalWeight += weight; //计算权重总和
        }

        float randomValue = Random.Range(0, totalWeight); //生成随机值
        float cumulativeWeight = 0; //累计权重

        foreach (var kvp in weights) //遍历权重字典
        {
            cumulativeWeight += kvp.Value; //累计权重
            if (randomValue <= cumulativeWeight) //如果随机值小于等于累计权重，则返回对应的节点类型
            {
                return kvp.Key;
            }
        }

        return NodeType.Combat; // 默认返回战斗
    }

    public void MoveToNode(Node node)
    {
        if (MapData.currentNode.children.Contains(node))
        {
            MapData.currentNode.isVisited = true;
            MapData.currentNode = node;
            // 禁用所有节点的交互性
            foreach (var nodeUI in NodeUIs.Values)
            {
                nodeUI.SetInteractable(false);
            }

            MapData.currentNode.isLocked = false;

            HighlightAvailablePaths();

            // 触发节点事件 (例如进入战斗、商店等，这里只是打印)
            Debug.Log("Entered node: " + node.nodeType);
            switch (node.nodeType)
            {
                // 根据不同的节点类型, 进入不同的场景或者进行其他操作
            }
        }
        else
        {
            Debug.LogWarning("Cannot move to node: " + node.nodeType);
        }
    }

    public void HighlightAvailablePaths()
    {
        // 高亮当前可走的路径

        // 先把所有节点设置为不可交互
        foreach (var kvp in NodeUIs)
        {
            kvp.Value.SetInteractable(false);
        }

        // 将当前节点可到达的节点设置为可交互
        foreach (Node child in MapData.currentNode.children)
        {
            if (!child.isLocked)
            {
                NodeUIs[child].SetInteractable(true);
            }
        }
    }

    // 在MapGenerator中添加保存和加载方法
    public void SaveMap(string path)
    {
        // 使用JsonUtility保存为JSON文件
        string json = JsonUtility.ToJson(MapData);
        System.IO.File.WriteAllText(path, json);
    }

    public void LoadMap(string path)
    {
        // 使用JsonUtility从JSON文件加载
        string json = System.IO.File.ReadAllText(path);
        MapData = JsonUtility.FromJson<Map>(json);
        //重新生成显示
        GenerateAndDisplayMapFromLoad();
    }

    public void GenerateAndDisplayMapFromLoad()
    {
        // 如果已经有地图了，先销毁
        if (mapRoot != null)
        {
            Destroy(mapRoot);
        }

        mapRoot = new GameObject("MapRoot");
        NodeUIs.Clear();
        // 实例化节点
        foreach (Node node in MapData.nodes)
        {
            GameObject nodeObj = Instantiate(NodePrefab, mapRoot.transform);
            NodeUI nodeUI = nodeObj.GetComponent<NodeUI>();
            nodeUI.Initialize(node, this);
            nodeObj.transform.localPosition =
                new Vector3(node.position.y * NodeSpacingX, -node.position.x * NodeSpacingY, 0);
            NodeUIs.Add(node, nodeUI);
        }

        HighlightAvailablePaths();
    }
}