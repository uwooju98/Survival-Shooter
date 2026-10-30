using UnityEngine;
using System.Collections;


public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,
        Empty,
        Realoding,
    }

    [Header("References")]
    [SerializeField] private Transform fireTransform;
    [SerializeField] private ParticleSystem muzzleFlashEffect;
    [SerializeField] private GunData gunData;

    [Header("Shooting")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float fireDistance = 50f;

    private LineRenderer bulletLineRenderer;

    private int ammoRemain;
    private int magAmmo;
    private float lastFireTime;

    public State CurrentState { get; private set; }

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
        magAmmo = gunData.MagCapacity;

        lastFireTime = 0f;
        CurrentState = State.Ready;

        // uiManager.SetAmmoText(magAmmo, ammoRemain);
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
        return
            CurrentState == State.Ready &&
            Time.time >= lastFireTime + gunData.TimeBetFire;
    }

    private void Shoot()
    {
        Vector3 hitPosition =
            GetHitPosition();

        magAmmo--;

        StartCoroutine(CoShotEffect(hitPosition));

        if (magAmmo <= 0)
        {
            CurrentState = State.Empty;
        }

        // uiManager.SetAmmoText(magAmmo, ammoRemain);
    }

    private Vector3 GetHitPosition()
    {
        Ray ray =
            new Ray(
                fireTransform.position,
                fireTransform.forward);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            fireDistance,
            targetLayer))
        {
            // IDamageable target =
            //     hit.collider.GetComponent<IDamageable>();
            //
            // if (target != null)
            // {
            //     target.OnDamage(
            //         gunData.Damage,
            //         hit.point,
            //         hit.normal);
            // }

            return hit.point;
        }

        return
            fireTransform.position +
            fireTransform.forward * fireDistance;
    }

    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();

        bulletLineRenderer.SetPosition(
            0,
            fireTransform.position);

        bulletLineRenderer.SetPosition(
            1,
            hitPosition);

        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.01f);

        bulletLineRenderer.enabled = false;
    }
}
