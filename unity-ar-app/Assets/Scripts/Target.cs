using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Score")]
    // [SerializeField] private int scoreValue = 1;
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 12f;

    [Header("Damage")]
    [SerializeField] private int damageToBase = 1;

    private bool isDead = false;
    private float lifeTimer;
    private EnemySpawner spawner;
    private Transform ballTarget;
    private Rigidbody rb;

    void OnEnable()
    {
        isDead = false;
        lifeTimer = lifeTime;
        rb = GetComponentInChildren<Rigidbody>();

        if (PlayerBase.Instance != null)
            ballTarget = PlayerBase.Instance.transform;
    }

    void Update()
    {
        if (isDead) return;
        if (GameManager.Instance == null || !GameManager.Instance.GameActive) return;
        if (ballTarget == null) return;

        Vector3 dir = (ballTarget.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) ReturnToPool();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.GetComponentInParent<PlayerBase>() != null)
        {
            Debug.Log("HIT: " + collision.gameObject.name);
            isDead = true;

            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            GameManager.Instance?.TakeDamage(damageToBase);
            ReturnToPool();
        }
    }

    public void OnHit()
    {
        if (isDead) return;
        isDead = true;

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // GameManager.Instance?.AddScore(scoreValue);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (spawner != null)
            spawner.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }

    public void SetSpawner(EnemySpawner s) => spawner = s;
}