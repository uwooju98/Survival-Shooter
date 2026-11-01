using UnityEngine;
using System.Collections;
using System;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,
        Empty,
        Reloading,
    }

    [Header("References")]
    [SerializeField] private Transform fireTransform;
    [SerializeField] private ParticleSystem muzzleFlashEffect;
    [SerializeField] private GunData gunData;

    [Header("Shooting")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float fireDistance = 50f;


    private LineRenderer bulletLineRenderer;
    private Coroutine reloadCoroutine;

    private bool canUse = true;

    private int ammoRemain;
    private int magAmmo;
    private int magCapacity;

    private int damage;
    private float lastFireTime;


    public int CurrentAmmo => magAmmo;

    public bool IsReloading => CurrentState == State.Reloading;

    public State CurrentState { get; private set; }


    public event Action<int> OnAmmoChanged;
    public event Action<float> OnReloadStarted;
    public event Action OnReloadFinished;


    private void Awake()
    {
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }


    private void OnEnable()
    {
        InitializeGun();
    }


    private void InitializeGun()
    {
        ammoRemain = gunData.StartAmmoRemain;
        magCapacity = gunData.MagCapacity;
        magAmmo = magCapacity;
        damage = gunData.Damage;

        OnAmmoChanged?.Invoke(magAmmo);

        lastFireTime = 0f;
        CurrentState = State.Ready;
    }


    public void Fire()
    {
        if (!CanFire())
            return;

        lastFireTime = Time.time;

        Shoot();
    }


    private bool CanFire()
    {
        return canUse && CurrentState == State.Ready && Time.time >= lastFireTime + gunData.TimeBetFire;
    }


    private void Shoot()
    {
        Vector3 hitPosition = GetHitPosition();
        magAmmo--;
        OnAmmoChanged?.Invoke(magAmmo);

        StartCoroutine(CoShotEffect(hitPosition));

        if (magAmmo <= 0)
        {
            CurrentState = State.Empty;
        }
    }


    private Vector3 GetHitPosition()
    {
        Ray ray = new Ray(fireTransform.position,fireTransform.forward);

        if (Physics.Raycast(ray,out RaycastHit hit,fireDistance,targetLayer))
        {
            Monster monster = hit.collider.GetComponentInParent<Monster>();

            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            return hit.point;
        }

        return
            fireTransform.position +
            fireTransform.forward * fireDistance;
    }


    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        bulletLineRenderer.SetPosition(0,fireTransform.position);
        bulletLineRenderer.SetPosition(1,hitPosition);
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.01f);

        bulletLineRenderer.enabled = false;
    }


    public void Reload()
    {
        if (!CanReload())
            return;

        reloadCoroutine = StartCoroutine(CoReload());
    }


    private bool CanReload()
    {
        return canUse && CurrentState != State.Reloading && magAmmo < magCapacity && ammoRemain > 0;
    }


    public void CancelReload()
    {
        if (!IsReloading)
            return;

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        if (magAmmo > 0)
        {
            CurrentState = State.Ready;
        }
        else
        {
            CurrentState = State.Empty;
        }

        OnReloadFinished?.Invoke();
    }


    private IEnumerator CoReload()
    {
        CurrentState = State.Reloading;

        OnReloadStarted?.Invoke(gunData.ReloadTime);

        yield return new WaitForSeconds(gunData.ReloadTime);

        int neededAmmo = magCapacity - magAmmo;

        int reloadAmmo = Mathf.Min(neededAmmo,ammoRemain);

        magAmmo += reloadAmmo;
        ammoRemain -= reloadAmmo;

        OnAmmoChanged?.Invoke(magAmmo);

        CurrentState = State.Ready;

        reloadCoroutine = null;

        OnReloadFinished?.Invoke();
    }


    public void EnableGun()
    {
        canUse = true;
    }


    public void DisableGun()
    {
        canUse = false;

        if (IsReloading)
        {
            CancelReload();
        }

        StopAllCoroutines();

        bulletLineRenderer.enabled = false;
    }


    public void IncreaseMagCapacity(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        magCapacity += amount;
    }
}