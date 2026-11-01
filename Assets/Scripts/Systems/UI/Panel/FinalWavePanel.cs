
using UnityEngine;
using UnityEngine.UI;

public class FinalWaveUI : MonoBehaviour
{
    [Header("Final Wave Timer")]
    [SerializeField] private Sprite[] numberSprites;
    [SerializeField] private Image secondTens;
    [SerializeField] private Image secondOnes;
    [SerializeField] private Image decimalTenths;
    [SerializeField] private Image decimalHundredths;


    public void UpdateFinalTimer(float remainingTime)
    {
        int totalCentiseconds = Mathf.CeilToInt(Mathf.Max(0f, remainingTime) * 100f);

        int seconds = totalCentiseconds / 100;
        int decimals = totalCentiseconds % 100;

        int tens = (seconds / 10) % 10;
        int ones = seconds % 10;

        secondTens.gameObject.SetActive(tens > 0);

        if (tens > 0)
        {
            secondTens.sprite = numberSprites[tens];
        }

        secondOnes.sprite = numberSprites[ones];

        decimalTenths.sprite = numberSprites[decimals / 10];
        decimalHundredths.sprite = numberSprites[decimals % 10];
    }
}
