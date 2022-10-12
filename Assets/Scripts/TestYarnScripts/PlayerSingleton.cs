using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton PlayerSing {get; private set;}
    public Player Play {get; private set;}
    public PlayerInput PInput {get; private set;}

    void Awake() {
        PlayerSing = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Play = GetComponent<Player>();
        PInput = GetComponent<PlayerInput>();
    }
}
