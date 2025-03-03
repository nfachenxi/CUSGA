using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float pathUpdateRate = 0.5f;
    public float stoppingDistance = 0.5f;
    public float turnSpeed = 3f; // 转向速度

    private Transform target;
    private Pathfinding pathfinding;
    private List<Node> path;
    private List<Vector2> bezierPath = new List<Vector2>(); // 存储贝塞尔曲线上的点
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (target == null)
        {
            Debug.LogError("没有找到标签为Player的物体！");
            return;
        }

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (target != null)
            {
                path = pathfinding.FindPath(transform.position, target.position);
                currentWaypoint = 0;
                //计算贝塞尔曲线路径
                CalculateBezierPath();
            }

            yield return new WaitForSeconds(pathUpdateRate);
        }
    }
    void CalculateBezierPath()
    {
        bezierPath.Clear();
        if (path != null && path.Count > 1)
        {
            // 至少需要2个点才能生成曲线，这里简单处理，大于两个点就生成曲线
            if (path.Count == 2)
            {
                // 如果只有两个点，直接使用这两个点
                bezierPath.Add(path[0].worldPosition);
                bezierPath.Add(path[1].worldPosition);
            }
            else
            {
                for (int i = 0; i < path.Count - 1; i += 1)
                {
                    // 控制点可以根据需要进行调整，这里简单地取路径点之间的中点作为控制点
                    Vector2 p0 = path[i].worldPosition;
                    Vector2 p1;
                    if (i == 0)
                    {
                        p1 = path[i].worldPosition;
                    }
                    else
                    {
                        p1 = (path[i].worldPosition + path[i - 1].worldPosition) / 2f;
                    }

                    Vector2 p2;

                    if (i == path.Count - 2)
                    {
                        p2 = path[i + 1].worldPosition;
                    }
                    else
                    {
                        p2 = (path[i].worldPosition + path[i + 1].worldPosition) / 2f;
                    }

                    Vector2 p3 = path[i + 1].worldPosition;

                    // 在路径段上生成多个点，增加曲线的平滑度
                    for (float t = 0; t <= 1; t += 0.1f)
                    {
                        bezierPath.Add(Pathfinding.CalculateCubicBezierPoint(t, p0, p1, p2, p3));
                    }
                }
            }

        }
        else if (path != null && path.Count == 1)
        {
            // 如果只有一个路径点，直接添加到贝塞尔路径中
            bezierPath.Add(path[0].worldPosition);
        }
    }

    void Update()
    {
        if (target == null || bezierPath == null || bezierPath.Count == 0)
            return;

        // 检查是否已经足够接近玩家
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget <= stoppingDistance)
        {
            // 已经足够接近，停止移动
            return;
        }

        if (currentWaypoint >= bezierPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }
        //移动到下一个路点
        Vector2 nextWaypointPosition = bezierPath[currentWaypoint];
        // 使用插值平滑移动
        transform.position = Vector2.MoveTowards(transform.position, nextWaypointPosition, moveSpeed * Time.deltaTime);

        // 转向逻辑
        Vector2 direction = (nextWaypointPosition - (Vector2)transform.position).normalized;
        if (direction != Vector2.zero) // 避免朝向(0,0)方向
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }


        // 检查是否到达当前路径点
        if (Vector2.Distance(transform.position, nextWaypointPosition) < 0.1f)
        {
            currentWaypoint++;
        }
    }

}