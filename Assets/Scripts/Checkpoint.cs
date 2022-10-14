using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player)) {
            player.AddCheckpoint(this);
        }
    }

    public void ResetCheckpoint() {
        RoomArea[] rooms = GameObject.FindObjectsOfType<RoomArea>();
        foreach (RoomArea room in rooms) {
            room.Reset();
        }
        BulletController[] bullets = GameObject.FindObjectsOfType<BulletController>();
        foreach (BulletController bullet in bullets) {
            Destroy(bullet.gameObject);
        }
    }
}
