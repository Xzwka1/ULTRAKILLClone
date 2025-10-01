using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 3f;
    public int damage = 25; // <--- เพิ่มตัวแปรดาเมจของกระสุน
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // --- ส่วนที่แก้ไข ---
        // 1. ตรวจสอบว่าวัตถุที่ชนมีสคริปต์ EnemyHealth หรือไม่
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

        // 2. ถ้ามี (หมายความว่าเราชนศัตรู)
        if (enemyHealth != null)
        {
            // 3. เรียกใช้ฟังก์ชัน TakeDamage เพื่อสร้างความเสียหาย
            enemyHealth.TakeDamage(damage);
        }
        // ------------------

        // ทำลายกระสุนหลังจากชน (เหมือนเดิม)
        Destroy(gameObject);
    }
}