using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Spawn Distance")]
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 12f;

    [Header("Collision Check")]
    [SerializeField] private float spawnCheckRadius = 0.4f;
    [SerializeField] private int maxSpawnRetries = 5;

    private LayerMask enemyLayerMask;
    private readonly HashSet<GameObject> activeEnemies = new();
    private float spawnTimer;
    private Vector3 spawnCenterPosition;

    void Start()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");

        if (PlayerBase.Instance != null)
            spawnCenterPosition = PlayerBase.Instance.transform.position;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(spawnPrefab);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.GameActive) return;

        CleanupEnemies();

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void CleanupEnemies()
    {
        activeEnemies.RemoveWhere(e => e == null || !e.activeInHierarchy);
    }

    private void TrySpawnEnemy()
    {
        if (activeEnemies.Count >= poolSize) return;

        for (int i = 0; i < maxSpawnRetries; i++)
        {
            Vector3 spawnPos = GenerateSpawnPosition();
            if (!Physics.CheckSphere(spawnPos, spawnCheckRadius, enemyLayerMask))
            {
                SpawnEnemy(spawnPos);
                return;
            }
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        // Full sphere around the ball, slightly biased above ground
        Vector3 randomDir = Random.onUnitSphere;
        randomDir.y = Mathf.Clamp(randomDir.y, -0.2f, 0.8f);
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
        return spawnCenterPosition + randomDir.normalized * radius;
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = enemyPool.Count > 0 ? enemyPool.Dequeue() : Instantiate(spawnPrefab);

        enemy.transform.position = position;

        // Face the ball immediately on spawn
        Vector3 lookDir = (spawnCenterPosition - position).normalized;
        enemy.transform.rotation = Quaternion.LookRotation(lookDir);

        Target t = enemy.GetComponent<Target>();
        if (t != null) t.SetSpawner(this);

        enemy.SetActive(true);
        activeEnemies.Add(enemy);
    }

    public void ReturnToPool(GameObject enemy)
    {
        if (!enemy.activeInHierarchy) return;
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
    public void ResetSpawner()
    {
        // Disable all active enemies
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }

        activeEnemies.Clear();

        // Reset timer
        spawnTimer = 0f;
    }
}