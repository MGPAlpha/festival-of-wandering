using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour {
    private InteractibleSelection interactibleSelection;

    [SerializeField] private bool singleUse = true;
    [SerializeField] private bool active = true;
    [SerializeField] private int priority = 0;

    [SerializeField] private UnityEvent onInteract;
    
    public bool CanTarget() {
        return active;
    }

    public bool Interact() { // Returns whether to keep target;
        if (singleUse) {
            active = false;
        }
        onInteract.Invoke();
        return active;
    }

    public void Target() {
        if (CanTarget() && interactibleSelection) {
            interactibleSelection.onTarget.Invoke();
        }
    }

    public void Detarget() {
        if (interactibleSelection) {
            interactibleSelection.onDetarget.Invoke();
        }
    }

    public int GetPriority() {
        return priority;
    }

    public void SetInteractible(bool inter) {
        active = inter;
    }

    void Start() {
        TryGetComponent<InteractibleSelection>(out interactibleSelection);
    }

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
