using UnityEngine;
using UnityEngine.AI; // สำคัญมากสำหรับ NavMeshAgent

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // การลาดตระเวน (Patrolling)
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // การโจมตี (Attacking)
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    // public GameObject projectile; // หากต้องการให้ยิงอะไรออกมา

    // สถานะของ AI (States)
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // player = GameObject.Find("Player").transform; // <--- ลบ หรือใส่ // หน้าบรรทัดนี้
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // ตรวจสอบระยะการมองเห็นและการโจมตี
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // จัดการสถานะของ AI
        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // เมื่อไปถึงจุดหมาย
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // คำนวณหาจุดเดินแบบสุ่มในระยะที่กำหนด
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // ตรวจสอบว่าจุดสุ่มนั้นอยู่บนพื้น (NavMesh) หรือไม่
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // ทำให้ศัตรูหยุดเดินและหันหน้าหาผู้เล่น
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // --- โค้ดส่วนการโจมตี ---
            // หากต้องการให้ยิง projectile ให้ un-comment บรรทัดด้านล่าง
            // Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            // rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            // rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            Debug.Log("Enemy is attacking Player!"); // แสดงข้อความใน Console ว่ากำลังโจมตี
            // --------------------

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // --- วาด Gizmos เพื่อให้เห็นระยะใน Editor (ไม่จำเป็นต่อการทำงาน) ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}