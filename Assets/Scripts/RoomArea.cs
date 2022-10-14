using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private List<EnemySpawner> spawners = new List<EnemySpawner>();

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.TryGetComponent<EnemySpawner>(out EnemySpawner spawner)) {
            spawners.Add(spawner);
        }
        if (other.TryGetComponent<Player>(out Player player)) {
            foreach (EnemySpawner spawn in spawners) {
                spawn.Spawn(Random.Range(.5f, 2f));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
