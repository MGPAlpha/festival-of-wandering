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
        
    }

    public void Interact() {

    }
}
