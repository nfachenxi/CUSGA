using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    // 输入和移动相关变量
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Vector2 smoothVelocity;

    private void Awake()
    {
        // 初始化输入控制
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        // 订阅移动输入事件
        playerControls.Player.Move.performed += OnMoveInput;
        playerControls.Player.Move.canceled += OnMoveInput;
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        playerControls.Player.Move.performed -= OnMoveInput;
        playerControls.Player.Move.canceled -= OnMoveInput;
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // 获取输入值
        moveInput = context.ReadValue<Vector2>();

        // 归一化输入，确保斜向移动不会更快
        if (moveInput.magnitude > 1f)
            moveInput.Normalize();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // 计算目标速度
        Vector2 targetVelocity = moveInput * moveSpeed;

        // 根据是否有输入选择加速度或减速度
        float smoothTime = moveInput.magnitude > 0.01f ? 1f / acceleration : 1f / deceleration;

        // 平滑过渡到目标速度
        currentVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref smoothVelocity,
            smoothTime
        );

        // 应用移动
        transform.position += (Vector3)currentVelocity * Time.deltaTime;
    }
}
