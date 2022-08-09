using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof (Interactible))]
public class InteractibleSelection : MonoBehaviour
{
    public UnityEvent onTarget;
    public UnityEvent onDetarget;

    

}
