using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;

    [Header("Fire Rate")]
    [SerializeField] private float fireRateCooldown = 1f;

    private Camera _mainCamera;
    private float _lastFireTime = -Mathf.Infinity;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (!GameManager.Instance.GameActive) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            TryFire();
        else if (Input.GetMouseButtonDown(0))
            TryFire();
    }

    void TryFire()
    {
        if (Time.time - _lastFireTime < fireRateCooldown)
            return;

        _lastFireTime = Time.time;
        Fire();
    }

    void Fire()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Shooter: No bullet prefab assigned.");
            return;
        }

        Transform cam = _mainCamera.transform;

        // Spawn bullet just in front of the camera
        Vector3 spawnPos = cam.position + cam.forward * 0.3f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, cam.rotation);

        // Add or reuse a Rigidbody to propel the bullet
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
            rb = bullet.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.velocity = cam.forward * bulletSpeed;

        // Attach the Bullet component that handles collision and cleanup
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent == null)
            bulletComponent = bullet.AddComponent<Bullet>();

        bulletComponent.Init(bulletLifetime);
    }
}