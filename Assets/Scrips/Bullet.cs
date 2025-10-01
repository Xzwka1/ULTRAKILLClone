using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 3f;
    public int damage = 25; // <--- ��������ô�����ͧ����ع
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // --- ��ǹ������ ---
        // 1. ��Ǩ�ͺ����ѵ�ط�誹��ʤ�Ի�� EnemyHealth �������
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

        // 2. ����� (���¤��������Ҫ��ѵ��)
        if (enemyHealth != null)
        {
            // 3. ���¡��ѧ��ѹ TakeDamage �������ҧ�����������
            enemyHealth.TakeDamage(damage);
        }
        // ------------------

        // ����¡���ع��ѧ�ҡ�� (����͹���)
        Destroy(gameObject);
    }
}