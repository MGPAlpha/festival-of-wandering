using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour {
    [SerializeField] private UnityEvent onInteract;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            //collider.gameObject.GetComponent<Player>().setCanInteract(true);
            onInteract.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            collider.gameObject.GetComponent<Player>().setCanInteract(false);
        }
    }
}
