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

    public State CurrentState { get; private set; }

    public Transform fireTransform;

    public ParticleSystem muzzleFlashEffect;
    private LineRenderer bulletLineRenderer;

    public GunData gunData;

    public LayerMask targetLayer;

    private float fireDistance = 50f;

    private int ammoRemain;
    private int magAmmo;

    private float lastFireTime;

    private void Awake()
    {   
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

        private void OnEnable()
    {
        ammoRemain = gunData.StartAmmoRemain;
        magAmmo = gunData.MagCapacity;
        //uiManager.SetAmmoText(magAmmo, ammoRemain);

        CurrentState = State.Ready;
        lastFireTime = 0f;
    }

      public void Fire()
    {
        if (CurrentState == State.Ready && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            lastFireTime = Time.time;
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 hitPosition;

        Ray ray = new Ray(fireTransform.position, fireTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            hitPosition = hit.point;

            // IDamageable target = hit.collider.GetComponent<IDamageable>();
            // if (target != null)
            // {
            //     target.OnDamage(gunData.damage, hit.point, hit.normal);
            // }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(CoShotEffect(hitPosition));

        magAmmo--;
        //uiManager.SetAmmoText(magAmmo, ammoRemain);

        if (magAmmo == 0)
        {
            CurrentState = State.Empty;
        }
    }

        private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        // shellEjectEffect.Play();
        // gunAudioPlayer.PlayOneShot(gunData.shotClip);

        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;
        yield return new WaitForSeconds(0.01f);

        bulletLineRenderer.enabled = false;
    }

}
