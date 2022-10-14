using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    List<Interactible> inRange;
    Interactible target = null;
    [SerializeField] private LayerMask interactibleLayers;
    // Start is called before the first frame update
    void Start()
    {
        inRange = new List<Interactible>();
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) | interactibleLayers) == interactibleLayers) {
            Interactible newInter = other.GetComponent<Interactible>();
            inRange.Add(newInter);
            FindTarget();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (((1 << other.gameObject.layer) | interactibleLayers) == interactibleLayers) {
            Interactible oldInter = other.GetComponent<Interactible>();
            inRange.Remove(oldInter);
    
        }
    }

    private void FindTarget() {
        if (inRange.Count <= 0) {
            if (target) target.Detarget();
            target = null;
        }
        else {
            Interactible oldTarget = target;
            target = null;
            foreach (Interactible inter in inRange) {
                if ((!target || inter.GetPriority() > target.GetPriority() 
                || (inter.GetPriority() == target.GetPriority() && (inter.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)) && inter.CanTarget()) target = inter;
            }
            if (target != oldTarget) {
                if (target) target.Target();
                if (oldTarget) oldTarget.Detarget();
            }
        }
    }

    public bool CanInteract { get => target != null; } 

    public void Interact() {
        if (target) {
            bool keepTarget = target.Interact();
            if (!keepTarget) {
                target.Detarget();
                FindTarget();
            }
        }
    }
}
