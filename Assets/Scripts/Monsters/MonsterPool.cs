
using System.Collections.Generic;
using UnityEngine;
using static MonsterSpawner;

public class MonsterPool : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Monster normalMonsterPrefab;
    [SerializeField] private Monster fastMonsterPrefab;
    [SerializeField] private Monster heavyMonsterPrefab;


    private List<Monster> normalPool = new List<Monster>();
    private List<Monster> fastPool = new List<Monster>();
    private List<Monster> heavyPool = new List<Monster>();


    public Monster GetMonster(MonsterType type)
    {
        switch (type)
        {
            case MonsterType.Normal:
                return GetInactiveMonster(normalPool, normalMonsterPrefab);

            case MonsterType.Fast:
                return GetInactiveMonster(fastPool, fastMonsterPrefab);

            case MonsterType.Heavy:
                return GetInactiveMonster(heavyPool, heavyMonsterPrefab);
        }

        return null;
    }


    private Monster GetInactiveMonster(List<Monster> pool, Monster prefab)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                return pool[i];
            }
        }

        Monster newMonster = Instantiate(prefab, transform);

        newMonster.gameObject.SetActive(false);

        pool.Add(newMonster);

        return newMonster;
    }
}
