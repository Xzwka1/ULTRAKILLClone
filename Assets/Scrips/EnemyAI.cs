using UnityEngine;
using UnityEngine.AI; // สำคัญมาก

public class EnemyAI : MonoBehaviour
{
    // References
    private NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    // Patrolling
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    public int attackDamage = 15; // ความแรงในการโจมตี
    private bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // Get references automatically
        agent = GetComponent<NavMeshAgent>();

        // --- ?? นี่คือส่วนที่เพิ่มเข้ามา ?? ---
        // 1. ตรวจสอบว่าลาก Player มาใส่ใน Inspector หรือยัง
        if (player == null)
        {
            // 2. ถ้ายัง (เป็น null) ให้ค้นหาอัตโนมัติด้วย Tag "Player"
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            // 3. ถ้าหาเจอ
            if (playerObject != null)
            {
                // 4. เอา Transform ของ Player มาเก็บไว้
                player = playerObject.transform;
            }
            else
            {
                // 5. (กันพลาด) ถ้าหาไม่เจอจริงๆ ให้ b?o l?i และปิดการทำงาน AI ตัวนี้
                Debug.LogError(gameObject.name + ": Cannot find GameObject with tag 'Player'. AI will be disabled.");
            }
        }
        // --- ?? จบส่วนที่เพิ่ม ?? ---
    }

    private void Update()
    {
        // --- ?? เพิ่ม Safety Check ?? ---
        // ถ้าหา Player ไม่เจอ (player ยังคงเป็น null) ให้หยุดทำงานฟังก์ชัน Update นี้ไปเลย
        if (player == null)
            return;
        // --- ?? จบส่วนที่เพิ่ม ?? ---

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // State machine logic
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
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Find a random point within the walkPointRange
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Check if the point is on the NavMesh
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {

        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {

                playerHealth.TakeDamage(attackDamage);
            }


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}