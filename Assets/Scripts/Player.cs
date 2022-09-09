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
    private WeaponEmitter _weaponEmitter;
    [SerializeField] private bool inDialog;
    [SerializeField] private bool canAttack;
    [SerializeField] private CinemachineCameraOffset _cco;
    [SerializeField] private GameObject fireworkWavePrefab;

    [SerializeField] private PlayerWeaponBase[] weapons = new PlayerWeaponBase[2];
    [SerializeField] private Weapon spell;

    private Vector2 moveDir;
    private Vector2 aimDir;
    private Vector2? targetPos;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float maxAimOffset = 2;

    [SerializeField] private InteractManager interactManager;

    private bool dodging = false;
    [SerializeField] private float dodgeSpeedFactor = 1.75f;
    [SerializeField] private float dodgeTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
        _attackComponent = GetComponentInChildren<PlayerAttackComponent>();
        _weaponEmitter = GetComponentInChildren<WeaponEmitter>();
        inDialog = false;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
        if (_cco) {
            _cco.m_Offset = Vector2.ClampMagnitude(aimDir, 1) * maxAimOffset;
        }
        if (aimDir.magnitude == 0) aimDir = moveDir;
        if (dodging) return;
        Vector2 normalizedAim = aimDir.normalized;
        _an.SetFloat("facingX", normalizedAim.x);
        _an.SetFloat("facingY", normalizedAim.y);
    }

    private void OnGamepadAim() {
        aimDir = _input.currentActionMap["Gamepad Aim"].ReadValue<Vector2>();
        // Debug.Log(aimDir);
        
    }

    private void FixedUpdate()
    {
        if (dodging) return;
        float speed = baseSpeed;
        if (_attackComponent.Attacking) speed *= _attackComponent.CurrAttack.MovementSpeed;
        _rb.velocity = Vector2.ClampMagnitude(moveDir, 1) * speed;
    }

    void OnPrimaryAttack() {
        if (weapons[0] && canAttack)
            _attackComponent.TriggerWeapon(weapons[0], aimDir);
    }

    void OnSecondaryAttack() {
        if (weapons[1] && canAttack)
            _attackComponent.TriggerWeapon(weapons[1], aimDir);
    }

    void OnSpell() {
        if (!spell || _weaponEmitter.FiringActive) return;
        _weaponEmitter.Fire(spell, aimDir);
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

    IEnumerator Dodge(Vector2 dir) {
        dodging = true;
        _an.SetFloat("facingX", dir.x);
        _an.SetFloat("facingY", dir.y);
        float dodgeSpeed = dodgeSpeedFactor * baseSpeed;
        _rb.velocity = dodgeSpeed * dir;
        float elapsedTime = 0;
        while (elapsedTime < dodgeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        dodging = false;
    }

    void OnDodge() {
        if (!dodging) {
            Vector2 dodgeDir = moveDir;
            if (moveDir == Vector2.zero) dodgeDir = aimDir;
            StartCoroutine(Dodge(dodgeDir.normalized));
        }
    }

    void OnFirework() {
        Instantiate(fireworkWavePrefab, transform.position, Quaternion.identity);
    }

    public void SetPrimary(PlayerWeaponBase weapon) {
        weapons[0] = weapon;
    }

    public void SetSecondary(PlayerWeaponBase weapon) {
        weapons[1] = weapon;
    }
}
