using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueSingleton : MonoBehaviour
{
    public static DialogueSingleton Dialogue { get; private set; }
    public DialogueRunner Runner {get; private set;}

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        Dialogue = this;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        Runner = GetComponent<DialogueRunner>();
    }
}