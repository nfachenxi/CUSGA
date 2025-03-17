using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform player; // 玩家的Transform
    public float smoothSpeed = 0.125f; // 摄像机跟随的平滑速度
    public Vector3 offset; // 摄像机与玩家的偏移量
    private float minX = -27f; // x轴最小限制
    private float maxX = 0f; // x轴最大限制
    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player is not assigned to the camera.");
            return;
        }
        // 计算目标位置
        Vector3 desiredPosition = player.position + offset;
        // 限制x轴的范围
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        // 保持y轴和z轴不变
        desiredPosition.y = transform.position.y;
        desiredPosition.z = transform.position.z;
        // 平滑移动摄像机
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}