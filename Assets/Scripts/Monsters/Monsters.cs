
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public enum State
    {
        Chase,
        Attack,
        Dead
    }


    private static readonly int HashMove = Animator.StringToHash("Move");
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashDeath = Animator.StringToHash("Death");


    [Header("Data")]
    [SerializeField] private MonsterData monsterData;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Animator monsterAnimator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private Transform model;


    private MonsterSpawner monsterSpawner;
    private PlayerHealth playerHealth;

    private int currentHp;

    private float attackDelay;
    private float attackTimer;
    private float moveAmount;
    private float distanceToPlayer;

    private bool isSinking;
    private float sinkTimer;
    private float sinkSpeed;
    private Vector3 modelStartPosition;


    public State CurrentState { get; private set; }


    private void Awake()
    {
        modelStartPosition = model.localPosition;

        agent.speed = monsterData.MoveSpeed;
        agent.angularSpeed = monsterData.AngularSpeed;
        agent.acceleration = monsterData.Acceleration;
        agent.stoppingDistance = monsterData.StoppingDistance;
        agent.autoBraking = monsterData.AutoBraking;
    }

    private void OnEnable()
    {
        model.localPosition = modelStartPosition;

        agent.enabled = true;
        bodyCollider.enabled = true;

        isSinking = false;

        currentHp = monsterData.MaxHp;
        attackDelay = monsterData.AttackDelay;
        attackTimer = attackDelay;

        sinkSpeed = monsterData.SinkSpeed;

        ChangeState(State.Chase);
    }

    private void Update()
    {
        if (CurrentState == State.Dead)
        {
            Sink();
            return;
        }

        UpdateStatus();

        monsterAnimator.SetFloat(HashMove, moveAmount);

        switch (CurrentState)
        {
            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;
        }
    }


    public void Initialize(Transform target, PlayerController playerController, PlayerHealth playerHealth, MonsterSpawner monsterSpawner)
    {
        this.target = target;
        this.playerController = playerController;
        this.playerHealth = playerHealth;
        this.monsterSpawner = monsterSpawner;
    }


    private void UpdateStatus()
    {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        moveAmount = agent.velocity.magnitude / agent.speed;

        attackTimer += Time.deltaTime;
    }

    private void ChangeState(State newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case State.Chase:
                agent.isStopped = false;
                break;

            case State.Attack:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                LookAtPlayer();
                break;

            case State.Dead:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                bodyCollider.enabled = false;

                monsterAnimator.SetTrigger(HashDeath);
                break;
        }
    }


    private void Chase()
    {
        agent.SetDestination(target.position);

        if (distanceToPlayer <= monsterData.AttackRange && !playerController.isDashing)
        {
            ChangeState(State.Attack);
        }
    }

    private void Attack()
    {
        if (distanceToPlayer > monsterData.AttackRange || playerController.isDashing)
        {
            ChangeState(State.Chase);
            return;
        }

        LookAtPlayer();

        if (attackTimer >= attackDelay)
        {
            attackTimer = 0f;

            monsterAnimator.SetTrigger(HashAttack);

            playerHealth.TakeDamage(monsterData.Damage);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 lookDirection = target.position - transform.position;

        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }


    public void TakeDamage(int damage)
    {
        if (CurrentState == State.Dead)
        {
            return;
        }

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        ChangeState(State.Dead);

        monsterSpawner.MonsterDead();
    }


    public void StartSinking()
    {
        agent.enabled = false;

        isSinking = true;
        sinkTimer = 0f;
    }

    private void Sink()
    {
        if (!isSinking)
        {
            return;
        }

        model.position += Vector3.down * sinkSpeed * Time.deltaTime;

        sinkTimer += Time.deltaTime;

        if (sinkTimer >= 0.75f)
        {
            isSinking = false;

            gameObject.SetActive(false);
        }
    }
}
