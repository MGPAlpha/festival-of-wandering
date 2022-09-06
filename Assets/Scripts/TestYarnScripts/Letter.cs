using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : Interactible
{
    private Collider2D _collider;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            GameObject player = collider.gameObject;

        }
    }
}
