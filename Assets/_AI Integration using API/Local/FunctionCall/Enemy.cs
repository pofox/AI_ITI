using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider healthbar;
    [SerializeField] int health = 100;
    int totalhealth = 100;
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthbar.value = health / totalhealth;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
    public void Attack()
    {
        Debug.Log("Enemy Attacked");
    }
}
