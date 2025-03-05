using UnityEngine;
using TMPro;

/// <summary>
/// 敌人控制类，负责处理敌人的生命值、血条显示和死亡逻辑
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// 敌人的最大生命值
    /// </summary>
    public int maxHealth = 3;

    /// <summary>
    /// 血条在敌人头顶的垂直偏移量
    /// </summary>
    public float healthBarYOffset = 0.5f;

    /// <summary>
    /// 血条预制体，包含Canvas和TextMeshPro组件
    /// </summary>
    public GameObject healthBarPrefab;

    /// <summary>
    /// 当前生命值
    /// </summary>
    private int currentHealth;

    /// <summary>
    /// 血条实例对象
    /// </summary>
    private GameObject healthBarInstance;

    /// <summary>
    /// 血条文本组件
    /// </summary>
    private TextMeshProUGUI healthText;

    /// <summary>
    /// 初始化敌人状态，创建血条实例
    /// </summary>
    void Start()
    {
        currentHealth = maxHealth;

        // 实例化血条预制体
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * healthBarYOffset, Quaternion.identity);
            healthBarInstance.transform.SetParent(transform); // 设置为子对象，跟随敌人移动

            // 获取TextMeshPro组件
            healthText = healthBarInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (healthText == null)
            {
                Debug.LogError("Health bar prefab does not contain a TextMeshPro component.");
            }

            UpdateHealthUI();
        }
        else
        {
            Debug.LogError("Health bar prefab is not assigned in the Enemy script.");
        }
    }
    //受伤
    /// <summary>
    /// 处理敌人受到伤害的逻辑
    /// </summary>
    /// <param name="damage">受到的伤害值</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // 防止血量低于0
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    //更新血条显示
    /// <summary>
    /// 更新血条UI显示
    /// </summary>
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }
    //死亡
    /// <summary>
    /// 处理敌人死亡逻辑，包括通知玩家增加击杀数和销毁对象
    /// </summary>
    void Die()
    {
        // 通知玩家增加击杀数 (稍后在PlayerController中实现)
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddKill();
        }

        // 销毁血条和自身
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
        Destroy(gameObject);
    }
    //因为敌人在移动，所以要实时更新血条位置
    /// <summary>
    /// 每帧更新血条位置，使其跟随敌人移动
    /// </summary>
    void Update()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + Vector3.up * healthBarYOffset;
        }
    }
}
