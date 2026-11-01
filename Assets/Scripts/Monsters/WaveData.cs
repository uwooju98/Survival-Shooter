
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Monster Count")]
    [SerializeField] private int normalCount;
    [SerializeField] private int fastCount;
    [SerializeField] private int heavyCount;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int spawnCountPerBatch = 2;


    public int NormalCount => normalCount;
    public int FastCount => fastCount;
    public int HeavyCount => heavyCount;

    public float SpawnInterval => spawnInterval;
    public int SpawnCountPerBatch => spawnCountPerBatch;
}
