using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    

    private PlayerInput _input;
    private Rigidbody2D _rb;
    private Animator _an;
    private PlayerAttackComponent _attackComponent;
    [SerializeField] private CinemachineCameraOffset _cco;

    [SerializeField] private PlayerWeaponBase[] weapons = new PlayerWeaponBase[2];

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
        _attackComponent = GetComponentInChildren<PlayerAttackComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
        aimDir = _input.currentActionMap["Gamepad Aim"].ReadValue<Vector2>();
        // Debug.Log(aimDir);
        if (_cco) {
            _cco.m_Offset = Vector2.ClampMagnitude(aimDir, 1) * maxAimOffset;
        }
        if (aimDir.magnitude == 0) aimDir = moveDir;
        Vector2 normalizedAim = aimDir.normalized;
        _an.SetFloat("facingX", normalizedAim.x);
        _an.SetFloat("facingY", normalizedAim.y);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector2.ClampMagnitude(moveDir, 1) * baseSpeed;
    }

    void OnPrimaryAttack() {
        if (weapons[0])
            _attackComponent.TriggerWeapon(weapons[0], aimDir);
    }

    void OnSecondaryAttack() {
        if (weapons[1])
            _attackComponent.TriggerWeapon(weapons[1], aimDir);
    }
}
