// NodeUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NodeUI : MonoBehaviour
{
    public Image Icon;
    public Button NodeButton;
    public Text NodeTypeText; // 可选
    public LineRenderer LineRenderer; // 如果使用LineRenderer
    //public GameObject LinePrefab; // 如果使用LinePrefab
    public Node NodeData;
    public MapGeneration mapGenerator;

    public void Initialize(Node node, MapGeneration generator)
    {
        NodeData = node;
        mapGenerator = generator;
        // 根据NodeType设置Icon的Sprite
        // NodeTypeText.text = node.NodeType.ToString(); // 可选

        NodeButton.onClick.AddListener(OnNodeClicked);

    }

    private void Start() {
        DrawLines();
    }

    private void DrawLines()
    {
        if (NodeData.children.Count > 0 && LineRenderer != null) // 使用Line Renderer
        {
            LineRenderer.positionCount = NodeData.children.Count * 2;
            for (int i = 0; i < NodeData.children.Count; i++)
            {
                LineRenderer.SetPosition(i * 2, transform.position);
                LineRenderer.SetPosition(i * 2 + 1, mapGenerator.NodeUIs[NodeData.children[i]].transform.position);
            }
        }
    }
    public void SetInteractable(bool interactable)
    {
        NodeButton.interactable = interactable;
    }


    private void OnNodeClicked()
    {
        mapGenerator.MoveToNode(NodeData);
    }
}