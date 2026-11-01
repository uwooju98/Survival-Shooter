using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
[SerializeField] private int damage = 50;
[SerializeField] private int startAmmoRemain = int.MaxValue;
[SerializeField] private int magCapacity = 25;
[SerializeField] private float timeBetFire = 0.2f;
[SerializeField] private float reloadTime = 1.5f;

public int Damage => damage;
public int StartAmmoRemain => startAmmoRemain;
public int MagCapacity => magCapacity;
public float TimeBetFire => timeBetFire;
public float ReloadTime => reloadTime;
}
