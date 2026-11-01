using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MonsterSpawner monsterSpawner;

    [Header("Wave UI")]
    [SerializeField] private Image waveOnesImage;

    [Header("Monster UI")]
    [SerializeField] private Image monsterTensImage;
    [SerializeField] private Image monsterOnesImage;

    [Header("Sprites")]
    [SerializeField] private Sprite[] numberSprites;

    private void OnEnable()
    {
        monsterSpawner.OnWaveStarted += UpdateWave;
        monsterSpawner.OnRemainingMonsterChanged += UpdateMonsterCount;
    }

    private void OnDisable()
    {
        monsterSpawner.OnWaveStarted -= UpdateWave;
        monsterSpawner.OnRemainingMonsterChanged -= UpdateMonsterCount;
    }

    private void UpdateWave(int wave)
    {
        waveOnesImage.sprite = numberSprites[wave];
    }

    private void UpdateMonsterCount(int monsterCount)
    {
        int tens = monsterCount / 10;
        int ones = monsterCount % 10;

        if (monsterCount >= 10)
        {
            monsterTensImage.gameObject.SetActive(true);
            monsterTensImage.sprite = numberSprites[tens];
        }
        else
        {
            monsterTensImage.gameObject.SetActive(false);
        }

        monsterOnesImage.sprite = numberSprites[ones];
    }
}