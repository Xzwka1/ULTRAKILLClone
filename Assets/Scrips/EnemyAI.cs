using UnityEngine;
using UnityEngine.AI; // �Ӥѭ�ҡ����Ѻ NavMeshAgent

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // ����Ҵ����ǹ (Patrolling)
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // ������� (Attacking)
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    // public GameObject projectile; // �ҡ��ͧ�������ԧ�����͡��

    // ʶҹТͧ AI (States)
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // player = GameObject.Find("Player").transform; // <--- ź ������� // ˹�Һ�÷Ѵ���
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // ��Ǩ�ͺ���С���ͧ�����С������
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // �Ѵ���ʶҹТͧ AI
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

        // �����件֧�ش����
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // �ӹǳ�Ҩش�ԹẺ��������з���˹�
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // ��Ǩ�ͺ��Ҩش����������躹��� (NavMesh) �������
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // ������ѵ����ش�Թ����ѹ˹���Ҽ�����
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // --- ����ǹ������� ---
            // �ҡ��ͧ�������ԧ projectile ��� un-comment ��÷Ѵ��ҹ��ҧ
            // Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            // rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            // rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            Debug.Log("Enemy is attacking Player!"); // �ʴ���ͤ���� Console ��ҡ��ѧ����
            // --------------------

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // --- �Ҵ Gizmos ��������������� Editor (�����繵�͡�÷ӧҹ) ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}