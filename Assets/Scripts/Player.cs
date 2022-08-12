using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;


public class Player : MonoBehaviour
{
    private PlayerInput _input;
    private Rigidbody2D _rb;
    private Animator _an;
    private PlayerAttackComponent _attackComponent;
    [SerializeField] private bool inDialog;
    [SerializeField] private bool canAttack;
    [SerializeField] private CinemachineCameraOffset _cco;

    [SerializeField] private PlayerWeaponBase[] weapons = new PlayerWeaponBase[2];

    private Vector2 moveDir;
    private Vector2 aimDir;
    private Vector2? targetPos;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float maxAimOffset = 2;

    [SerializeField] private InteractManager interactManager;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
        _attackComponent = GetComponentInChildren<PlayerAttackComponent>();
        inDialog = false;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float speed = baseSpeed;
        if (_attackComponent.Attacking) speed *= _attackComponent.CurrAttack.MovementSpeed;
        _rb.velocity = Vector2.ClampMagnitude(moveDir, 1) * speed;
        if (targetPos != null && ((Vector2)(targetPos - transform.position)).magnitude <= 1) {
            moveDir = Vector2.zero;
            targetPos = null;
        }
    }

    private void OnMove() {
        moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
    }

    private void OnGamepadAim() {
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

    void OnPrimaryAttack() {
        if (weapons[0] && canAttack)
            _attackComponent.TriggerWeapon(weapons[0], aimDir);
    }

    void OnSecondaryAttack() {
        if (weapons[1] && canAttack)
            _attackComponent.TriggerWeapon(weapons[1], aimDir);
    }

    void OnInteract() {
        interactManager.Interact();
    }

    [YarnCommand("face_towards")]
    public void FaceTowards(GameObject gameObject) {
        Vector2 vector = gameObject.transform.position - this.transform.position;
        Vector2 normalVector = vector.normalized;
        _an.SetFloat("facingX", normalVector.x);
        _an.SetFloat("facingY", normalVector.y);
    }

    [YarnCommand("walk_to")]
    public void WalkTo(GameObject gameObject) {
        FaceTowards(gameObject);
            //_rb.velocity = Vector2.ClampMagnitude(gameObject.transform.position - this.transform.position, 1) * baseSpeed;
            //transform.position += (Vector3)(Vector2.ClampMagnitude(gameObject.transform.position - this.transform.position, 1) * baseSpeed * Time.deltaTime);
        targetPos = gameObject.transform.position;
        moveDir = gameObject.transform.position - transform.position;
    }
}
