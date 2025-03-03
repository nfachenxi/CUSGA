using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("武器设置")]
    public GameObject meleeAreaPrefab; // 近战攻击区域预制体
    public GameObject projectilePrefab; // 三清铃子弹预制体
    public float meleeAttackRange = 1f; // 近战攻击范围 (以玩家为中心)
    public float meleeAttackWidth = 1f; //近战判定矩形的宽度
    public float meleeAttackHeight = 1f; //近战判定矩形的高度
    public int meleeDamage = 1;  //近战伤害
    public float projectileSpeed = 10f;
    public int projectileDamage = 1;
    public float killCountYOffset = 0.8f; // 击杀数显示在头顶的偏移量
    public GameObject killCountUIPrefab; // 击杀数UI预制体

    [Header("玩家设置")]
    // 这些按键设置现在通过 Input System 来处理, 这里不再需要
    // public KeyCode meleeAttackKey = KeyCode.J; // 近战攻击键
    // public KeyCode rangedAttackKey = KeyCode.K; // 远程攻击键

    private int killCount = 0;
    private GameObject killCountUIInstance;
    private TextMeshProUGUI killCountText;

    // 输入和移动相关变量
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Vector2 smoothVelocity;
    private Vector2 lastFacingDirection = Vector2.right; // 初始方向为右

    private void Awake()
    {
        // 初始化输入控制
        playerControls = new PlayerControls();

        // 订阅攻击输入事件
        playerControls.Player.MeleeAttack.performed += ctx => PerformMeleeAttack();
        playerControls.Player.RangedAttack.performed += ctx => ShootProjectile();
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

        // 实例化击杀数UI预制体
        if (killCountUIPrefab != null)
        {
            killCountUIInstance = Instantiate(killCountUIPrefab, transform.position + Vector3.up * killCountYOffset, Quaternion.identity);
            killCountUIInstance.transform.SetParent(transform); // 设置为子对象，跟随玩家移动
            // 获取TextMeshPro组件
            killCountText = killCountUIInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (killCountText == null)
            {
                Debug.LogError("kill Count UI prefab does not contain a TextMeshPro component.");
            }
            UpdateKillCountUI();
        }
        else
        {
            Debug.LogError("kill Count UI prefab is not assigned in the Enemy script.");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        playerControls.Player.Move.performed -= OnMoveInput;
        playerControls.Player.Move.canceled -= OnMoveInput;

        // 取消攻击事件的订阅 (重要：防止内存泄漏)
        playerControls.Player.MeleeAttack.performed -= ctx => PerformMeleeAttack();
        playerControls.Player.RangedAttack.performed -= ctx => ShootProjectile();
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

        //更新击杀UI位置
        if (killCountUIInstance != null)
        {
            killCountUIInstance.transform.position = transform.position + Vector3.up * killCountYOffset;
        }
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

        // 更新最后一次移动的方向 (只有在有移动输入时才更新)
        if (moveInput.magnitude > 0.01f)
        {
            lastFacingDirection = moveInput.normalized;
        }
    }

    // 近战攻击
    void PerformMeleeAttack()
    {
        // 计算攻击区域的中心点 (根据玩家朝向)
        Vector2 attackPosition = (Vector2)transform.position + (IsFacingRight() ? Vector2.right : Vector2.left) * meleeAttackRange;


        // 使用OverlapBoxAll检测区域内的敌人
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPosition, new Vector2(meleeAttackWidth, meleeAttackHeight), 0);


        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
        //显示攻击区域
        if (meleeAreaPrefab != null)
        {
            GameObject melee = Instantiate(meleeAreaPrefab, attackPosition, transform.rotation);
            Destroy(melee, 0.2f);
        }
        else
        {
            Debug.LogError("melee Area Prefab is null,please check!");
        }
    }

    // 远程攻击 (发射三清铃)
    void ShootProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity); // 不需要旋转，子弹自己会处理朝向
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 根据玩家朝向设置子弹速度
                rb.velocity = (IsFacingRight() ? Vector2.right : Vector2.left) * projectileSpeed;

            }

            // 设置子弹的伤害
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.damage = projectileDamage;
            }
        }
        else
        {
            Debug.LogError("Projectile Prefab is null,please check it!");
        }
    }
    //判断玩家朝向,根据移动输入的方向来判断
    private bool IsFacingRight()
    {
        return lastFacingDirection.x >= 0;  //如果水平输入大于等于0则视为面向右
    }

    // 增加击杀数
    public void AddKill()
    {
        killCount++;
        UpdateKillCountUI();
    }

    void UpdateKillCountUI()
    {
        if (killCountText != null)
        {
            killCountText.text = killCount.ToString();
        }
    }
}
