using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 2f; // 子弹的生命周期
    [HideInInspector]
    public int damage = 1;  //伤害

    void Start()
    {
        Destroy(gameObject, lifetime); // 生命周期结束后销毁
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否击中敌人
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // 击中敌人后销毁自身
        }

        // 如果击中其他物体 (例如墙壁), 也销毁自身.  需要给墙壁添加碰撞体和Tag等信息
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
