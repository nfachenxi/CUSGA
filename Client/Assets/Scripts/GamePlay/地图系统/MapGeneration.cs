using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class MapGeneration : MonoBehaviour
{
    [FormerlySerializedAs("NodePrefab")] public GameObject nodePrefab;
    [FormerlySerializedAs("MapData")] public Map mapData;
    [FormerlySerializedAs("MaxDepth")] public int maxDepth = 10;
    [FormerlySerializedAs("MaxWidth")] public int maxWidth = 5;
    [FormerlySerializedAs("NodeSpacingX")] public float nodeSpacingX = 2f;
    [FormerlySerializedAs("NodeSpacingY")] public float nodeSpacingY = 2f;

    [FormerlySerializedAs("MinRandomSpacingX")] public float minRandomSpacingX = 1.5f; // 最小随机X间隔
    [FormerlySerializedAs("MaxRandomSpacingX")] public float maxRandomSpacingX = 2.5f; // 最大随机X间隔
    [FormerlySerializedAs("MinRandomSpacingY")] public float minRandomSpacingY = 1.5f; // 最小随机Y间隔
    [FormerlySerializedAs("MaxRandomSpacingY")] public float maxRandomSpacingY = 2.5f; // 最大随机Y间隔

    public RectTransform mapContainer; // 使用 RectTransform 作为地图容器

    public Dictionary<Node, NodeUI> NodeUIs = new Dictionary<Node, NodeUI>();
    public GameObject mapRoot;

    private Vector2 _dragStartPosition;
    private Vector3 _mapStartPos;

    //新增：滚轮滚动速度
    public float scrollSpeed = 50f;

    void Start()
    {
        GenerateAndDisplayMap();
        // 初始地图位置（可以根据需要调整）
        mapContainer.localPosition = new Vector3(0, 0, 0);
    }
    private void Update()
    {
        HandleMapDragging();
        HandleMapScrolling(); // 添加滚轮滚动处理
    }
    public void GenerateAndDisplayMap()
    {
        // 如果已经有地图了，先销毁
        if (mapRoot != null)
        {
            Destroy(mapRoot);
        }
        mapData = GenerateStructuredMap(); // 生成地图数据
        NodeUIs.Clear();
        // 实例化节点, 并设置父对象为 mapContainer
        foreach (Node node in mapData.nodes)
        {
            GameObject nodeObj = Instantiate(nodePrefab, mapContainer); // 注意这里的变化
            NodeUI nodeUI = nodeObj.GetComponent<NodeUI>();
            nodeUI.Initialize(node, this);
            float randomSpacingX = Random.Range(minRandomSpacingX, maxRandomSpacingX);
            float randomSpacingY = Random.Range(minRandomSpacingY, maxRandomSpacingY);
            // 修改：Y坐标计算，实现自下而上
            nodeObj.transform.localPosition =
                new Vector3(node.position.y * randomSpacingX, node.position.x * randomSpacingY, 0);
            NodeUIs.Add(node, nodeUI);
        }
        HighlightAvailablePaths();
    }
    private void HandleMapDragging()
    {
        if (Input.GetMouseButtonDown(0)) // 0 代表鼠标左键
        {
            _dragStartPosition = Input.mousePosition;
            _mapStartPos = mapContainer.localPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 dragDelta = (Vector2)Input.mousePosition - _dragStartPosition;

            //反转了Y轴方向
            mapContainer.localPosition = _mapStartPos + new Vector3(dragDelta.x, -dragDelta.y, 0);
            ClampMapPosition();
        }
    }

    //新增鼠标滚动方法
    private void HandleMapScrolling()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            Vector3 pos = mapContainer.localPosition;
            pos.y += scroll * scrollSpeed; // 根据滚动方向调整
            mapContainer.localPosition = pos;
            ClampMapPosition();
        }
    }

    private void ClampMapPosition()
    {
        // 获取地图的边界,这里的计算需要根据你的节点分布和数量来确定。
        // 例如，你可以遍历所有节点，找到最左、最右、最上、最下的节点的坐标，然后加上一定的边距。
        float mapWidth = maxWidth * nodeSpacingX * 1.5f;  // 使用最大间隔估算
        float mapHeight = maxDepth * nodeSpacingY * 1.8f;

        // 获取 Canvas 的尺寸（假设 MapContainer 填满了整个 Canvas）
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // 计算可滚动的边界
        float minX = canvasWidth - mapWidth;
        float maxX = 0;
        float minY = canvasHeight - mapHeight;
        float maxY = 0;

        // 如果地图内容小于 Canvas，则不允许滚动
        if (mapWidth < canvasWidth)
        {
            minX = maxX = 0;  // 将地图水平居中
        }
        if (mapHeight < canvasHeight)
        {
            minY = maxY = 0; // 将地图垂直居中
        }


        // 限制 mapContainer 的位置
        Vector3 clampedPosition = mapContainer.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        mapContainer.localPosition = clampedPosition;
    }


    // 其余方法保持不变，除了 GenerateAndDisplayMapFromLoad 中的 Instantiate 也要修改
    public void GenerateAndDisplayMapFromLoad()
    {
        if (mapRoot != null)
        {
            Destroy(mapRoot);
        }

        mapRoot = new GameObject("MapRoot");
        NodeUIs.Clear();

        foreach (Node node in mapData.nodes)
        {
            GameObject nodeObj = Instantiate(nodePrefab, mapContainer);
            NodeUI nodeUI = nodeObj.GetComponent<NodeUI>();
            nodeUI.Initialize(node, this);
            // 修改：Y坐标计算，实现自下而上
            nodeObj.transform.localPosition =
                new Vector3(node.position.y * nodeSpacingX, node.position.x * nodeSpacingY, 0);
            NodeUIs.Add(node, nodeUI);
        }

        HighlightAvailablePaths();
    }

    // Todo 修正第二节点与第三节点的未连接问题
    public Map GenerateStructuredMap()
    {
        Map map = new Map();
        map.maxDepth = maxDepth;
        map.maxWidth = maxWidth;
        map.nodes = new List<Node>();

        // 创建起始节点（第0层）
        Node startNode = CreateNode(NodeType.Combat, new Vector2(0, 0));
        startNode.isLocked = false;
        map.nodes.Add(startNode);
        map.currentNode = startNode;

        // 创建三条主要分支的起点（第1层，全部为普通战斗）
        Node branch1Start = CreateNode(NodeType.Combat, new Vector2(1, -1));
        Node branch2Start = CreateNode(NodeType.Combat, new Vector2(1, 0));
        Node branch3Start = CreateNode(NodeType.Combat, new Vector2(1, 1));

        map.nodes.Add(branch1Start);
        map.nodes.Add(branch2Start);
        map.nodes.Add(branch3Start);

        // 连接起始节点到三个分支起点
        ConnectNodes(startNode, branch1Start);
        ConnectNodes(startNode, branch2Start);
        ConnectNodes(startNode, branch3Start);

        // 生成第一条分支路径（左侧）
        List<Node> branch1Nodes = GenerateBranchPath(1, -1.5f, map, true);

        // 生成第三条分支路径（右侧）
        List<Node> branch3Nodes = GenerateBranchPath(1, 1.5f, map, true);

        // 生成第二条分支路径（中间）
        List<Node> branch2Nodes = GenerateBranchPath(1, 0, map, false);

        // 为第二条分支创建交叉连接
        // 随机选择X和Y的值（第3-6层之间）
        int xLayer = Random.Range(3, 7);
        int yLayer = Random.Range(3, 7);

        // 找到对应层的节点
        Node xNode = branch2Nodes.Find(n => (int)n.position.x == xLayer);
        Node xTargetNode = branch1Nodes.Find(n => (int)n.position.x == xLayer + 1);

        Node yNode = branch2Nodes.Find(n => (int)n.position.x == yLayer);
        Node yTargetNode = branch3Nodes.Find(n => (int)n.position.x == yLayer + 1);

        // 创建交叉连接
        if (xNode != null && xTargetNode != null)
        {
            ConnectNodes(xNode, xTargetNode);
        }

        if (yNode != null && yTargetNode != null)
        {
            ConnectNodes(yNode, yTargetNode);
        }

        // 创建Boss节点（第10层）
        Node bossNode = CreateNode(NodeType.Boss, new Vector2(9, 0));
        map.nodes.Add(bossNode);

        // 连接第9层所有节点到Boss节点
        List<Node> layer9Nodes = map.nodes.FindAll(n => (int)n.position.x == 8);
        foreach (Node node in layer9Nodes)
        {
            ConnectNodes(node, bossNode);
        }

        return map;
    }

    // 生成单条分支路径
    private List<Node> GenerateBranchPath(int startDepth, float horizontalPosition, Map map, bool isOuterPath)
    {
        List<Node> branchNodes = new List<Node>();

        // 已经添加的节点计数
        int eliteCount = 0;
        int shopCount = 0;
        int eventCount = 0;

        // 第2层也是普通战斗关
        Node layer2Node = CreateNode(NodeType.Combat, new Vector2(2, horizontalPosition));
        map.nodes.Add(layer2Node);
        branchNodes.Add(layer2Node);

        // 连接到对应的第1层节点
        Node layer1Node = map.nodes.Find(n => (int)n.position.x == 1 &&
                                         Mathf.Approximately(n.position.y, horizontalPosition));
        if (layer1Node != null)
        {
            ConnectNodes(layer1Node, layer2Node);
        }

        // 生成第3-9层的节点
        for (int depth = 3; depth <= 8; depth++)
        {
            NodeType nodeType;

            // 根据路径类型和已有节点数量决定节点类型
            if (isOuterPath) // 第一条和第三条路径
            {
                // 4-5个普通关+精英关，最多1个精英关
                if (depth <= 7 && eliteCount < 1 && Random.value < 0.25f)
                {
                    nodeType = NodeType.EliteCombat;
                    eliteCount++;
                }
                // 2-3个商店+事件，最多1个商店
                else if (shopCount + eventCount < 3)
                {
                    if (shopCount < 1 && Random.value < 0.3f)
                    {
                        nodeType = NodeType.Shop;
                        shopCount++;
                    }
                    else
                    {
                        nodeType = NodeType.Mystery; // 事件节点
                        eventCount++;
                    }
                }
                else
                {
                    nodeType = NodeType.Combat;
                }
            }
            else // 第二条路径
            {
                // 第二条路径的节点类型分布类似，但会有分叉
                if (depth <= 7 && eliteCount < 1 && Random.value < 0.25f)
                {
                    nodeType = NodeType.EliteCombat;
                    eliteCount++;
                }
                else if (shopCount + eventCount < 3)
                {
                    if (shopCount < 1 && Random.value < 0.3f)
                    {
                        nodeType = NodeType.Shop;
                        shopCount++;
                    }
                    else
                    {
                        nodeType = NodeType.Mystery; // 事件节点
                        eventCount++;
                    }
                }
                else
                {
                    nodeType = NodeType.Combat;
                }
            }

            // 创建节点
            Node newNode = CreateNode(nodeType, new Vector2(depth, horizontalPosition));
            map.nodes.Add(newNode);
            branchNodes.Add(newNode);

            // 连接到上一层节点
            Node previousNode = branchNodes[branchNodes.Count - 2];
            ConnectNodes(previousNode, newNode);
        }

        return branchNodes;
    }

    // 连接两个节点
    private void ConnectNodes(Node parent, Node child)
    {
        if (!parent.children.Contains(child))
        {
            parent.children.Add(child);
        }

        if (!child.parents.Contains(parent))
        {
            child.parents.Add(parent);
        }
    }

    private Node CreateNode(NodeType type, Vector2 position)
    {
        Node node = new Node();
        node.nodeType = type;
        node.position = position;
        return node;
    }
    
    private List<Node> GetNodesAtDepth(Map map, int depth)
    {
        return map.nodes.FindAll(n => (int)n.position.x == depth);
    }

    public void MoveToNode(Node node)
    {
        if (mapData.currentNode.children.Contains(node))
        {
            mapData.currentNode.isVisited = true;
            mapData.currentNode = node;
            // 禁用所有节点的交互性
            foreach (var nodeUI in NodeUIs.Values)
            {
                nodeUI.SetInteractable(false);
            }

            mapData.currentNode.isLocked = false;

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
        foreach (Node child in mapData.currentNode.children)
        {
            if (!child.isLocked)
            {
                NodeUIs[child].SetInteractable(true);
            }
        }
    }

    // 保存和加载方法保持不变
    public void SaveMap(string path)
    {
        string json = JsonUtility.ToJson(mapData);
        System.IO.File.WriteAllText(path, json);
    }

    public void LoadMap(string path)
    {
        string json = System.IO.File.ReadAllText(path);
        mapData = JsonUtility.FromJson<Map>(json);
        GenerateAndDisplayMapFromLoad();
    }
}
