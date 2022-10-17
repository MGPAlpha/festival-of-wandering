using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Triggerable : MonoBehaviour
{
    [SerializeField] private LayerMask triggerableLayers;
    [SerializeField] private bool singleUse;
    [SerializeField] UnityEvent enterTrigger;
    [SerializeField] UnityEvent exitTrigger;
    [SerializeField] UnityEvent stayTrigger;

    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) | triggerableLayers) == triggerableLayers) {
            enterTrigger.Invoke();
            if (singleUse) {
                Destroy(GetComponent<BoxCollider2D>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (((1 << other.gameObject.layer) | triggerableLayers) == triggerableLayers) {
            exitTrigger.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (((1 << other.gameObject.layer) | triggerableLayers) == triggerableLayers) {
            stayTrigger.Invoke();
        }
    }
}
