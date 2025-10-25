using UnityEngine;
using UnityEngine.AI; // �Ӥѭ�ҡ

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
    public int attackDamage = 15; // �����ç㹡������
    private bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // Get references automatically
        agent = GetComponent<NavMeshAgent>();

        // --- ?? �������ǹ������������ ?? ---
        // 1. ��Ǩ�ͺ����ҡ Player ������ Inspector �����ѧ
        if (player == null)
        {
            // 2. ����ѧ (�� null) �������ѵ��ѵԴ��� Tag "Player"
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            // 3. �������
            if (playerObject != null)
            {
                // 4. ��� Transform �ͧ Player �������
                player = playerObject.transform;
            }
            else
            {
                // 5. (�ѹ��Ҵ) ���������ͨ�ԧ� ��� b?o l?i ��лԴ��÷ӧҹ AI ��ǹ��
                Debug.LogError(gameObject.name + ": Cannot find GameObject with tag 'Player'. AI will be disabled.");
            }
        }
        // --- ?? ����ǹ������� ?? ---
    }

    private void Update()
    {
        // --- ?? ���� Safety Check ?? ---
        // ����� Player ����� (player �ѧ���� null) �����ش�ӧҹ�ѧ��ѹ Update �������
        if (player == null)
            return;
        // --- ?? ����ǹ������� ?? ---

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