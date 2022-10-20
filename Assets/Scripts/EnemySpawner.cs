using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private SpriteRenderer _sp;
    
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] private float actualSpawnTimestamp = .75f;
    [SerializeField] private float spawnAnimSpeed = 1;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private EnemyData enemyToSpawn;

    public bool SpawnComplete { get; private set; } = false;
    public bool SpawnKilled { get => SpawnComplete && (!childEnemy || childEnemy.GetComponent<Enemy>().IsDead); }

    private GameObject childEnemy;

    // Start is called before the first frame update
    void Start()
    {
        _sp = GetComponent<SpriteRenderer>();
        _sp.material.SetFloat("_Progress", 0);
        // Spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    Coroutine spawnCoroutine;

    private IEnumerator SpawnRoutine(float delay) {
        float spawnTimer = -delay;
        while (spawnTimer < 0) {
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        while (spawnTimer < actualSpawnTimestamp) {
            spawnTimer += Time.deltaTime * spawnAnimSpeed;
            _sp.material.SetFloat("_Progress", spawnTimer);
            yield return null;
        }
        if (enemyToSpawn) {
            childEnemy = Instantiate(enemyPrefab, transform.position + (Vector3)spawnOffset, Quaternion.identity);
            childEnemy.GetComponent<Enemy>().Initialize(enemyToSpawn);
            SpawnComplete = true;
        }
        while (spawnTimer < 1) {
            spawnTimer += Time.deltaTime * spawnAnimSpeed;
            _sp.material.SetFloat("_Progress", spawnTimer);
            yield return null;
        }

        _sp.material.SetFloat("_Progress", 0);

    }

    public void Spawn(float delay) {
        spawnCoroutine = StartCoroutine(SpawnRoutine(delay));
    }

    public void Reset() {
        SpawnComplete = false;
        StopAllCoroutines();
        _sp.material.SetFloat("_Progress", 0);
        if (childEnemy) Destroy(childEnemy);
    }
}
