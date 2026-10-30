using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
[SerializeField] private float damage = 10f;
[SerializeField] private int startAmmoRemain = 999;
[SerializeField] private int magCapacity = 999;
[SerializeField] private float timeBetFire = 0.2f;
[SerializeField] private float reloadTime = 1.5f;

public float Damage => damage;
public int StartAmmoRemain => startAmmoRemain;
public int MagCapacity => magCapacity;
public float TimeBetFire => timeBetFire;
public float ReloadTime => reloadTime;
}
