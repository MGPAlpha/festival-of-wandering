using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomArea : MonoBehaviour
{

    public static bool InCombat {get; private set;} = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool roomStarted = false;
    private bool roomComplete = false;

    [SerializeField] private GameObject mistPrefab;

    private List<EnemySpawner> spawners = new List<EnemySpawner>();

    private GameObject[] mists;

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
        if (!roomStarted && other.TryGetComponent<Player>(out Player player)) {
            roomStarted = true;

            InCombat = true;
            
            foreach (EnemySpawner spawn in spawners) {
                spawn.Spawn(Random.Range(.5f, 2f));
            }

            GameObject mist1 = Instantiate(mistPrefab, transform.position + (transform.lossyScale.y/2 + 1) * Vector3.up, Quaternion.identity);
            GameObject mist2 = Instantiate(mistPrefab, transform.position + (transform.lossyScale.y/2 + 1) * Vector3.down, Quaternion.identity);
            GameObject mist3 = Instantiate(mistPrefab, transform.position + (transform.lossyScale.x/2 + 1) * Vector3.left, Quaternion.identity);
            GameObject mist4 = Instantiate(mistPrefab, transform.position + (transform.lossyScale.x/2 + 1) * Vector3.right, Quaternion.identity);
            mist1.transform.localScale = new Vector3(transform.lossyScale.x, 2, 1);
            mist2.transform.localScale = new Vector3(transform.lossyScale.x, 2, 1);
            mist3.transform.localScale = new Vector3(2, transform.lossyScale.y, 1);
            mist4.transform.localScale = new Vector3(2, transform.lossyScale.y, 1);
            mists = new GameObject[]{mist1, mist2, mist3, mist4};
            foreach (GameObject mist in mists) {
                MistBarrier mb = mist.GetComponent<MistBarrier>();
                mb.SetMistActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!roomComplete && roomStarted) {
            bool allKilled = true;
            foreach (EnemySpawner sp in spawners) {
                if (!sp.SpawnKilled) {
                    allKilled = false;
                    break;
                }
            }
            if (allKilled) {
                roomComplete = true;
                InCombat = false;
                InCombat = false;
                foreach (GameObject mistObj in mists) {
                    mistObj.GetComponent<MistBarrier>().SetMistActive(false);
                }
            }
        }
    }

    public void Reset() {
        if (mists != null) {
            foreach (GameObject mist in mists) {
                if (mist) Destroy(mist);
            }
        }
        roomComplete = false;
        roomStarted = false;

        InCombat = false;

        foreach (EnemySpawner spawner in spawners) {
            spawner.Reset();
        }


    }
}
