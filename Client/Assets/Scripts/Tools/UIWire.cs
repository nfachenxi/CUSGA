using UnityEngine;
using UnityEngine.UI;

public class UIWire : MaskableGraphic
{
    [SerializeField] private RectTransform startPoint;  // 起点RectTransform
    [SerializeField] private RectTransform endPoint;    // 终点RectTransform
    [SerializeField] private float wireWidth = 5f;      // 线宽

    // 新增：端点偏移
    public Vector2 startOffset = new Vector2(0,-25);
    public Vector2 endOffset= new Vector2(0,-25);

    // 属性变化时标记为需要重新构建Mesh
    public RectTransform StartPoint
    {
        get { return startPoint; }
        set { startPoint = value; SetVerticesDirty(); }
    }

    public RectTransform EndPoint
    {
        get { return endPoint; }
        set { endPoint = value; SetVerticesDirty(); }
    }

    public float WireWidth
    {
        get { return wireWidth; }
        set { wireWidth = value; SetVerticesDirty(); }
    }

    //可以根据需求设置颜色
    public void SetWireColor(Color color)
    {
        this.color = color;
        SetVerticesDirty();
    }

    //参数设置例子
    public void SetPoints(RectTransform start, RectTransform end, float width, Color col)
    {
        startPoint = start;
        endPoint = end;
        wireWidth = width;
        color = col;
        SetVerticesDirty(); // 标记顶点需要更新
    }
    //世界坐标转换到本地坐标
    private Vector2 WorldToLocalPoint(Vector3 worldPosition)
    {
        // 获取 Canvas
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("UIWire must be a child of a Canvas!");
            return Vector2.zero;
        }
        // 如果 Canvas 的渲染模式是 Screen Space - Overlay，可以直接使用 worldPosition
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return transform.InverseTransformPoint(worldPosition);
        }

        // 如果 Canvas 的渲染模式是 Screen Space - Camera 或 World Space，需要使用相机进行转换
        Camera camera = canvas.worldCamera;
        if (camera == null)
        {
            Debug.LogError("Canvas is in Screen Space - Camera or World Space mode, but no camera is assigned!");
            return Vector2.zero;
        }

        // 将世界坐标转换为屏幕坐标
        Vector2 screenPoint = camera.WorldToScreenPoint(worldPosition);

        // 将屏幕坐标转换为本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, screenPoint, camera, out Vector2 localPoint);
        return localPoint;
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); // 清除之前的顶点数据

        if (startPoint == null || endPoint == null)
        {
            return; // 如果起点或终点为空，则不绘制
        }

        // 获取起点和终点的世界坐标
        Vector3 startWorldPos = startPoint.position;
        Vector3 endWorldPos = endPoint.position;

        // 将世界坐标转换为相对于 Canvas 的本地坐标,并加上偏移
        Vector2 startPos = WorldToLocalPoint(startWorldPos) + startOffset;
        Vector2 endPos = WorldToLocalPoint(endWorldPos) + endOffset;

        // 计算线段的方向向量
        Vector2 direction = (endPos - startPos).normalized;

        // 计算线段的垂直方向向量（用于确定线宽方向）
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        // 计算四个顶点的偏移
        Vector2 halfWidth = perpendicular * (wireWidth / 2f);
        Vector2 topLeft = startPos + halfWidth;
        Vector2 topRight = endPos + halfWidth;
        Vector2 bottomLeft = startPos - halfWidth;
        Vector2 bottomRight = endPos - halfWidth;

        //顶点颜色
        Color32 color32 = color;

        // 添加顶点
        vh.AddVert(topLeft, color32, new Vector2(0, 1));
        vh.AddVert(bottomLeft, color32, new Vector2(0, 0));
        vh.AddVert(bottomRight, color32, new Vector2(1, 0));
        vh.AddVert(topRight, color32, new Vector2(1, 1));


        // 添加三角形（两个三角形组成一个矩形）
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    //每帧检查变化
    protected void Update()
    {
        //base.Update();
        if (startPoint != null && endPoint != null)
        {
            // 检查起点或终点的位置是否发生变化
            if (startPoint.hasChanged || endPoint.hasChanged)
            {
                SetVerticesDirty(); // 标记顶点需要更新,触发OnPopulateMesh
                startPoint.hasChanged = false;
                endPoint.hasChanged = false;
            }
        }
    }
}
