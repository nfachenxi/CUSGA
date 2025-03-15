using System.Collections.Generic;

[System.Serializable]
public class Map
{
        public List<Node> nodes = new List<Node>();//存储所有节点
        public Node currentNode;//指向玩家当前所在的节点
        public int maxDepth;//地图最大层数(最大10层)
        public int maxWidth;//地图最大宽度
}
