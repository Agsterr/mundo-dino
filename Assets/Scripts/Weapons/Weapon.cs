using System;
using System.Collections;
using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponStats stats;
        [SerializeField] private Transform firePoint;
        [SerializeField] private LineRenderer bulletTrail;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private float trailDuration = 0.05f;

        private int _ammoInMagazine;
        private int _reserveAmmo;
        private float _nextFireTime;
        private bool _isReloading;

        public WeaponStats Stats => stats;
        public int AmmoInMagazine => _ammoInMagazine;
        public int ReserveAmmo => _reserveAmmo;
        public bool IsReloading => _isReloading;

        public event Action OnFired;
        public event Action OnReloadStarted;
        public event Action OnReloadFinished;
        public event Action OnAmmoChanged;

        private void Awake()
        {
            EnsureFirePoint();
            EnsureBulletTrail();

            if (stats != null)
            {
                ResetAmmo();
            }

            if (bulletTrail != null)
            {
                bulletTrail.enabled = false;
            }
        }

        private void EnsureFirePoint()
        {
            if (firePoint != null)
            {
                return;
            }

            GameObject point = new GameObject("FirePoint");
            point.transform.SetParent(transform);
            point.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            firePoint = point.transform;
        }

        private void EnsureBulletTrail()
        {
            if (bulletTrail != null)
            {
                return;
            }

            bulletTrail = gameObject.AddComponent<LineRenderer>();
            bulletTrail.startWidth = 0.04f;
            bulletTrail.endWidth = 0.02f;
            bulletTrail.positionCount = 2;
            bulletTrail.material = new Material(Shader.Find("Sprites/Default"));
            bulletTrail.startColor = new Color(1f, 0.9f, 0.3f, 1f);
            bulletTrail.endColor = new Color(1f, 0.3f, 0.1f, 0.6f);
            bulletTrail.enabled = false;
        }

        public void SetStats(WeaponStats newStats)
        {
            stats = newStats;
            ResetAmmo();
        }

        public void ResetAmmo()
        {
            _isReloading = false;
            _nextFireTime = 0f;
            _ammoInMagazine = stats.magazineSize;
            _reserveAmmo = stats.maxReserveAmmo;
            OnAmmoChanged?.Invoke();
        }

        public bool TryFire(Camera aimCamera)
        {
            if (stats == null || aimCamera == null || _isReloading)
            {
                return false;
            }

            if (Time.time < _nextFireTime)
            {
                return false;
            }

            if (_ammoInMagazine <= 0)
            {
                return false;
            }

            _ammoInMagazine--;
            _nextFireTime = Time.time + stats.fireRate;
            OnAmmoChanged?.Invoke();

            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 direction = ApplySpread(ray.direction);

            Vector3 origin = firePoint != null ? firePoint.position : ray.origin;
            Vector3 endPoint = origin + direction * stats.range;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, stats.range, hitMask, QueryTriggerInteraction.Ignore))
            {
                endPoint = hit.point;

                Health health = hit.collider.GetComponentInParent<Health>();
                if (health != null)
                {
                    health.TakeDamage(stats.damage);
                }
            }

            ShowTrail(origin, endPoint);
            NotifyGunfire(origin);
            OnFired?.Invoke();
            return true;
        }

        private static void NotifyGunfire(Vector3 shotOrigin)
        {
            VelociraptorAI[] raptors = FindObjectsByType<VelociraptorAI>(FindObjectsSortMode.None);
            foreach (VelociraptorAI raptor in raptors)
            {
                raptor.AlertToGunfire(shotOrigin);
            }
        }

        public bool TryReload()
        {
            if (_isReloading || stats == null)
            {
                return false;
            }

            if (_ammoInMagazine >= stats.magazineSize)
            {
                return false;
            }

            if (_reserveAmmo <= 0)
            {
                return false;
            }

            StartCoroutine(ReloadRoutine());
            return true;
        }

        private IEnumerator ReloadRoutine()
        {
            _isReloading = true;
            OnReloadStarted?.Invoke();

            yield return new WaitForSeconds(stats.reloadTime);

            int needed = stats.magazineSize - _ammoInMagazine;
            int toLoad = Mathf.Min(needed, _reserveAmmo);
            _ammoInMagazine += toLoad;
            _reserveAmmo -= toLoad;

            _isReloading = false;
            OnReloadFinished?.Invoke();
            OnAmmoChanged?.Invoke();
        }

        private Vector3 ApplySpread(Vector3 direction)
        {
            if (stats.spreadDegrees <= 0f)
            {
                return direction;
            }

            float spreadX = UnityEngine.Random.Range(-stats.spreadDegrees, stats.spreadDegrees);
            float spreadY = UnityEngine.Random.Range(-stats.spreadDegrees, stats.spreadDegrees);
            Quaternion spread = Quaternion.Euler(spreadY, spreadX, 0f);
            return spread * direction;
        }

        private void ShowTrail(Vector3 start, Vector3 end)
        {
            if (bulletTrail == null)
            {
                return;
            }

            bulletTrail.positionCount = 2;
            bulletTrail.SetPosition(0, start);
            bulletTrail.SetPosition(1, end);
            bulletTrail.enabled = true;
            StartCoroutine(HideTrailAfterDelay());
        }

        private IEnumerator HideTrailAfterDelay()
        {
            yield return new WaitForSeconds(trailDuration);
            if (bulletTrail != null)
            {
                bulletTrail.enabled = false;
            }
        }
    }
}
