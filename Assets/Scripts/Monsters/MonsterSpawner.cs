
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    public enum MonsterType
    {
        Normal,
        Fast,
        Heavy
    }


    [Header("References")]
    [SerializeField] private MonsterPool monsterPool;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Camera mainCamera;

    [Header("Spawn Position")]
    [SerializeField] private float minSpawnDistance = 15f;
    [SerializeField] private float maxSpawnDistance = 25f;
    [SerializeField] private float navMeshSampleDistance = 3f;
    [SerializeField] private int maxSpawnAttempts = 20;

    [Header("Wave")]
    [SerializeField] private WaveData[] waves;

    [Header("Final Wave")]
    [SerializeField] private float finalSurviveTime = 15f;


    private List<MonsterType> spawnList = new List<MonsterType>();

    private int currentWaveIndex;
    private int aliveMonsterCount;
    private int spawnedMonsterCount;

    private float spawnTimer;
    private float finalSurviveTimer;

    private bool isSpawning;
    private bool isFinalClear;


    private WaveData CurrentWave => waves[currentWaveIndex];
    private bool IsFinalWave => currentWaveIndex == waves.Length - 1;


    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCleared;
    public event Action<int> OnRemainingMonsterChanged;

    public event Action OnFinalWaveReady;
    public event Action<float> OnFinalTimerChanged;
    public event Action OnFinalClear;


    private void Update()
    {
        if (!isSpawning)
        {
            return;
        }

        if (IsFinalWave && !isFinalClear)
        {
            finalSurviveTimer += Time.deltaTime;

            float remainingTime = Mathf.Max(0f, finalSurviveTime - finalSurviveTimer);

            OnFinalTimerChanged?.Invoke(remainingTime);

            if (finalSurviveTimer >= finalSurviveTime)
            {
                isFinalClear = true;

                OnFinalClear?.Invoke();

                return;
            }
        }

        if (spawnedMonsterCount >= spawnList.Count)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= CurrentWave.SpawnInterval)
        {
            spawnTimer = 0f;

            SpawnBatch();
        }
    }


    public void StartWave()
    {
        spawnedMonsterCount = 0;
        aliveMonsterCount = 0;
        spawnTimer = 0f;

        CreateSpawnList();
        ShuffleSpawnList();

        OnWaveStarted?.Invoke(currentWaveIndex + 1);
        OnRemainingMonsterChanged?.Invoke(spawnList.Count);

        if (IsFinalWave)
        {
            finalSurviveTimer = 0f;
            isFinalClear = false;

            OnFinalTimerChanged?.Invoke(finalSurviveTime);
        }

        isSpawning = true;
    }

    private void NextWave()
    {
        isSpawning = false;

        int clearedWave = currentWaveIndex + 1;

        currentWaveIndex++;

        if (IsFinalWave)
        {
            OnFinalWaveReady?.Invoke();
            return;
        }

        OnWaveCleared?.Invoke(clearedWave);
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }


    private void CreateSpawnList()
    {
        spawnList.Clear();

        for (int i = 0; i < CurrentWave.NormalCount; i++)
        {
            spawnList.Add(MonsterType.Normal);
        }

        for (int i = 0; i < CurrentWave.FastCount; i++)
        {
            spawnList.Add(MonsterType.Fast);
        }

        for (int i = 0; i < CurrentWave.HeavyCount; i++)
        {
            spawnList.Add(MonsterType.Heavy);
        }
    }

    private void ShuffleSpawnList()
    {
        for (int i = 0; i < spawnList.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnList.Count);

            MonsterType temp = spawnList[i];

            spawnList[i] = spawnList[randomIndex];
            spawnList[randomIndex] = temp;
        }
    }


    private void SpawnBatch()
    {
        for (int i = 0; i < CurrentWave.SpawnCountPerBatch; i++)
        {
            if (spawnedMonsterCount >= spawnList.Count)
            {
                break;
            }

            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        if (!TryGetSpawnPosition(out Vector3 spawnPosition))
        {
            Debug.Log("몬스터 생성 위치를 찾지 못했습니다.");
            return;
        }

        MonsterType monsterType = spawnList[spawnedMonsterCount];

        Monster monster = monsterPool.GetMonster(monsterType);

        monster.transform.position = spawnPosition;

        monster.Initialize(playerTransform, playerController, playerHealth, this);

        monster.gameObject.SetActive(true);

        spawnedMonsterCount++;
        aliveMonsterCount++;
    }


    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 randomPosition = playerTransform.position +
                new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                if (!IsInsideCamera(hit.position))
                {
                    spawnPosition = hit.position;
                    return true;
                }
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private bool IsInsideCamera(Vector3 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);

        return viewportPosition.z > 0f &&
               viewportPosition.x >= 0f &&
               viewportPosition.x <= 1f &&
               viewportPosition.y >= 0f &&
               viewportPosition.y <= 1f;
    }


    public void MonsterDead()
    {
        aliveMonsterCount--;

        int deadMonsterCount = spawnedMonsterCount - aliveMonsterCount;
        int remainingMonsterCount = spawnList.Count - deadMonsterCount;

        OnRemainingMonsterChanged?.Invoke(remainingMonsterCount);

        if (!IsFinalWave && spawnedMonsterCount >= spawnList.Count && aliveMonsterCount == 0)
        {
            NextWave();
        }
    }
}
