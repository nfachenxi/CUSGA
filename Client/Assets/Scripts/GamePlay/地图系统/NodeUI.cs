using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NodeUI : MonoBehaviour
{
    public Image Icon;
    public Button NodeButton;

    public Node NodeData;
    public MapGeneration mapGenerator;

    public List<UIWire> wires = new List<UIWire>();//存储连接线 

    public delegate void NodeClickedHandler(Node node);
    public event NodeClickedHandler OnNodeClickedEvent;

    public void Initialize(Node node, MapGeneration generator)
    {
        NodeData = node;
        mapGenerator = generator;
        // 根据NodeType设置Icon的Sprite
        // NodeTypeText.text = node.NodeType.ToString(); // 可选
        if (this.Icon != null) this.Icon.overrideSprite = SpiritManager.Instance.areaImages[(int)node.nodeType];
        NodeButton.onClick.AddListener(OnNodeClicked);

    }

    private void Start()
    {
        DrawWires();
    }

    private void DrawWires()
    {
        foreach (var child in NodeData.children)
        {
            if (mapGenerator.NodeUIs.ContainsKey(child))
            {
                var childUI = mapGenerator.NodeUIs[child];
                var wire = CreateWire(this.GetComponent<RectTransform>(), childUI.GetComponent<RectTransform>());
                wires.Add(wire);
            }
        }
    }

    private UIWire CreateWire(RectTransform start, RectTransform end)
    {
        var wireObj = new GameObject("Wire", typeof(UIWire));
        wireObj.AddComponent<CanvasRenderer>(); // 为线添加CanvasRenderer组件
        wireObj.transform.SetParent(mapGenerator.mapContainer); // 将线放置在mapContainer下
        //新增：将连接线设置为第一个子物体，确保在节点下方
        wireObj.transform.SetAsFirstSibling();
        var wire = wireObj.GetComponent<UIWire>();
        wire.SetPoints(start, end, 5f, Color.white); // 设置线的宽度和颜色
        return wire;
    }


    public void SetInteractable(bool interactable)
    {
        NodeButton.interactable = interactable;
    }


    private void OnNodeClicked()
    {
        OnNodeClickedEvent?.Invoke(NodeData);
        mapGenerator.MoveToNode(NodeData);
    }
}
