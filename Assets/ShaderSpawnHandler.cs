using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSpawnHandler : MonoBehaviour
{

    [SerializeField] private Material spawnMaterial;
    [SerializeField] private Material defaultMaterialAfterSpawn;

    bool spawning = false;
    [SerializeField] private float spawnTime = 1;
    [SerializeField] private bool disableCollider = true;

    Renderer _re;
    Collider2D _col;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Spawn() {
        if (spawning) return;
        _re = GetComponent<Renderer>();
        _col = GetComponent<Collider2D>();
        StartCoroutine(SpawnCoroutine());
    }
     
    private IEnumerator SpawnCoroutine() {
        spawning = true;
        _col.enabled = false;
        if (!defaultMaterialAfterSpawn) {
            defaultMaterialAfterSpawn = _re.material;
        }
        _re.material = spawnMaterial;
        float timer = 0;
        while (timer < spawnTime) {
            _re.material.SetFloat("_Progress", timer / spawnTime);
            timer += Time.deltaTime;
            yield return null;
        }
        _re.material = defaultMaterialAfterSpawn;
        spawning = false;
        _col.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
