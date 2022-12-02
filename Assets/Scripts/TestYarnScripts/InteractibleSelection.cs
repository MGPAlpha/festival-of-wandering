using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof (Interactible))]
public class InteractibleSelection : MonoBehaviour
{
    public UnityEvent onTarget;
    public UnityEvent onDetarget;

    private SpriteRenderer _sp;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _sp = GetComponent<SpriteRenderer>();
    }

    public void OutlineOn() 
    {
        _sp.material.SetInt("_Outline", 1);
    }

    public void OutlineOff() 
    {
        _sp.material.SetInt("_Outline", 0);
    }

}
