using UnityEngine;

[CreateAssetMenu(
    fileName = "MonsterData",
    menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float sinkSpeed = 1f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float angularSpeed = 720f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private bool autoBraking = true;


    public int MaxHp => maxHp;
    public int Damage => damage;
    public float AttackDelay => attackDelay;
    public float AttackRange => attackRange;
    public float SinkSpeed => sinkSpeed;


    public float MoveSpeed => moveSpeed;
    public float AngularSpeed => angularSpeed;
    public float Acceleration => acceleration;
    public float StoppingDistance => stoppingDistance;
    public bool AutoBraking => autoBraking;
}