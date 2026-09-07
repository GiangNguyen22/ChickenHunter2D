using UnityEngine;

public class ShootOnClick : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Bullet Settings")]
    public float bulletSpeed = 12f;

    private Camera mainCamera;
    private float nextFireTime;

    private int GunLevel
    {
        get { return GameManager.Instance != null ? GameManager.Instance.GunLevel : 1; }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanActivate()) return;
        if (bulletPrefab == null) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + GetFireInterval();
        }
    }

    private int BulletCount()
    {
        int lvl = GunLevel;
        if (lvl >= 4) return 7;
        if (lvl == 3) return 5;
        if (lvl == 2) return 3;
        return 1;
    }

    private float GetFireInterval()
    {
        int lvl = GunLevel;
        if (lvl >= 4) return 0.12f;
        if (lvl == 3) return 0.15f;
        if (lvl == 2) return 0.24f;
        return 0.33f;
    }

    private float GetSpread()
    {
        int lvl = GunLevel;
        if (lvl >= 4) return 36f;
        if (lvl == 3) return 26f;
        if (lvl == 2) return 14f;
        return 0f;
    }

    private void Shoot()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 baseDir = (worldPos - origin).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        int count = BulletCount();
        float spread = GetSpread();

        for (int i = 0; i < count; i++)
        {
            int offset = i - (count - 1) / 2;
            float angle = baseAngle;
            if (count > 1)
            {
                angle += spread * (offset / (float)(count - 1));
            }

            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bulletObj = Instantiate(bulletPrefab, origin, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Initialize(dir);
                bullet.speed = bulletSpeed;
            }
            bulletObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        AudioManager.Instance?.PlayShoot();
    }
}