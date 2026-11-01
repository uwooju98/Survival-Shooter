using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Gun gun;

    [SerializeField] private Image tensImage;
    [SerializeField] private Image onesImage;

    [SerializeField] private Sprite[] numberSprites;


    private void OnEnable()
    {
        gun.OnAmmoChanged += UpdateAmmo;

        UpdateAmmo(gun.CurrentAmmo);
    }


    private void OnDisable()
    {
        gun.OnAmmoChanged -= UpdateAmmo;
    }


    private void UpdateAmmo(int ammo)
    {
        int tens = ammo / 10;
        int ones = ammo % 10;

        tensImage.sprite = numberSprites[tens];
        onesImage.sprite = numberSprites[ones];
    }
}