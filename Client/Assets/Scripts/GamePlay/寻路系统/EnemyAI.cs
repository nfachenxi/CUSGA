using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float pathUpdateRate = 0.5f;
    public float stoppingDistance = 0.5f;
    public float turnSpeed = 3f; // 转向速度

    private Transform _target;
    
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;

        if (_target == null)
        {
            Debug.LogError("没有找到标签为Player的物体！");
            return;
        }
        
    }

    void Update()
    {
        if (_target == null)
            return;

        // 直接移动到玩家位置
        transform.position = Vector2.MoveTowards(transform.position, _target.position, moveSpeed * Time.deltaTime);
    }

}