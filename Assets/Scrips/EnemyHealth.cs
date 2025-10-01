using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        // ตั้งค่าพลังชีวิตเริ่มต้นให้เต็ม
        currentHealth = maxHealth;
    }

    // ฟังก์ชันนี้จะถูกเรียกจากสคริปต์อื่น (เช่น Bullet) เพื่อลดเลือด
    public void TakeDamage(int damage)
    {
        // ลดเลือดตามดาเมจที่ได้รับ
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        // ตรวจสอบว่าเลือดหมดหรือยัง
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // สามารถเพิ่มเอฟเฟกต์ตอนตายได้ที่นี่ เช่น เสียงระเบิด, Particle System
        Debug.Log(gameObject.name + " has died.");

        // ทำลาย GameObject ของศัตรูตัวนี้
        Destroy(gameObject);
    }
}