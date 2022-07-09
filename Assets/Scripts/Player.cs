using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    

    private PlayerInput _input;
    private Rigidbody2D _rb;
    private Animator _an;
    [SerializeField] private CinemachineCameraOffset _cco;

    private Vector2 moveDir;
    private Vector2 aimDir;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float maxAimOffset = 2;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
        if (/*_input.currentControlScheme == "Gamepad"*/ true) aimDir = _input.currentActionMap["Gamepad Aim"].ReadValue<Vector2>();
        else  {
            
        }
        // Debug.Log(aimDir);
        if (_cco) {
            _cco.m_Offset = Vector2.ClampMagnitude(aimDir, 1) * maxAimOffset;
        }
        _an.SetFloat("facingX", aimDir.x);
        _an.SetFloat("facingY", aimDir.y);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector2.ClampMagnitude(moveDir, 1) * baseSpeed;
    }

    void OnPrimaryAttack() {
        Debug.Log("Primary attack");
    }
}
