using UnityEngine;

/// <summary>
/// 敌人控制类，负责处理敌人的生命值和死亡逻辑
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// 敌人的最大生命值
    /// </summary>
    public int maxHealth = 3;

    /// <summary>
    /// 当前生命值
    /// </summary>
    private int _currentHealth;

    /// <summary>
    /// 初始化敌人状态
    /// </summary>
    void Start()
    {
        _currentHealth = maxHealth;
    }

    /// <summary>
    /// 处理敌人受到伤害的逻辑
    /// </summary>
    /// <param name="damage">受到的伤害值</param>
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth); // 防止血量低于0

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 处理敌人死亡逻辑，包括通知玩家增加击杀数和销毁对象
    /// </summary>
    void Die()
    {
        // 通知玩家增加击杀数
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddKill();
        }

        // 销毁自身
        Destroy(gameObject);
    }
}