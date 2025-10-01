using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        // ��駤�Ҿ�ѧ���Ե�������������
        currentHealth = maxHealth;
    }

    // �ѧ��ѹ���ж١���¡�ҡʤ�Ի����� (�� Bullet) ����Ŵ���ʹ
    public void TakeDamage(int damage)
    {
        // Ŵ���ʹ��������������Ѻ
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        // ��Ǩ�ͺ������ʹ��������ѧ
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // ����ö�����Ϳ࿡��͹��������� �� ���§���Դ, Particle System
        Debug.Log(gameObject.name + " has died.");

        // ����� GameObject �ͧ�ѵ�ٵ�ǹ��
        Destroy(gameObject);
    }
}