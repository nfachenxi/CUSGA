using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;
    public float healthBarYOffset = 0.5f; // 血条在头顶的偏移量
    public GameObject healthBarPrefab; // 血条预制体 (包含Canvas和TextMeshPro)

    private int currentHealth;
    private GameObject healthBarInstance;
    private TextMeshProUGUI healthText;

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
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }
    //死亡
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
    void Update()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + Vector3.up * healthBarYOffset;
        }
    }
}

