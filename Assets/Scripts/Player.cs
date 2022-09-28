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

    private bool initAttack;
    private bool initMove;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool canMove;
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
        canMove = true;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_cco) {
            _cco.m_Offset = Vector2.ClampMagnitude(aimDir, 1) * maxAimOffset;
        }
        if (!canMove) {
            moveDir = Vector2.zero;
            _rb.velocity = Vector2.zero;
            return;
        }
        moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
        if (aimDir.magnitude == 0) aimDir = moveDir;
        if (dodging) return;
        Vector2 normalizedAim = aimDir.normalized;
        _an.SetFloat("facingX", normalizedAim.x);
        _an.SetFloat("facingY", normalizedAim.y);
        _an.SetBool("walking", moveDir.magnitude > 0);
    }

    private void OnGamepadAim() {
        if (canMove) {
            aimDir = _input.currentActionMap["Gamepad Aim"].ReadValue<Vector2>();
        }
        // Debug.Log(aimDir);
    }

    private void FixedUpdate()
    {
        if (dodging || !canMove) return;
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
        if (!canAttack || !spell || _weaponEmitter.FiringActive) return;
        _weaponEmitter.Fire(spell, aimDir);
    }
    
    void OnInteract() {
        interactManager.Interact();
    }

    [YarnCommand("p_face_towards")]
    public void FaceTowards(GameObject gameObject) {
        Vector2 dir = gameObject.transform.position - this.transform.position;
        Vector2 normalizedAim = dir.normalized;
        _an.SetFloat("facingX", normalizedAim.x);
        _an.SetFloat("facingY", normalizedAim.y);
        _an.SetBool("walking", false);
    }

    [YarnCommand("p_walk_to")]
    public IEnumerator WalkTo(GameObject gameObject, float time) {
        FaceTowards(gameObject);
        float finalTime = Time.time + time;
        float totalTime = 0;
        Vector2 startPosition = transform.position;
        Vector2 finalPosition = gameObject.transform.position;
        _an.SetBool("walking", true);
        while (Time.time < finalTime) {
            transform.position = Vector2.Lerp(startPosition, finalPosition, totalTime / time);
            totalTime += Time.deltaTime;
            yield return null;
        }
        _an.SetBool("walking", false);
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
        if (!dodging && canMove) {
            Vector2 dodgeDir = moveDir;
            if (moveDir == Vector2.zero) dodgeDir = aimDir;
            StartCoroutine(Dodge(dodgeDir.normalized));
        }
    }

    void OnFirework() {
        if (canAttack) {
            Instantiate(fireworkWavePrefab, transform.position, Quaternion.identity);
        }
    }

    public void SetPrimary(PlayerWeaponBase weapon) {
        weapons[0] = weapon;
    }

    public void SetSecondary(PlayerWeaponBase weapon) {
        weapons[1] = weapon;
    }

    public void SetSpell(Weapon spell) {
        this.spell = spell;
    }

    public void StartDialogue() {
        initAttack = canAttack;
        initMove = canMove;
        if (initAttack) {
            canAttack = false;
        }
        if (initMove) {
            canMove = false;
            aimDir = Vector2.zero;
        }
    }

    public void StopDialogue() {
        if (initAttack) {
            canAttack = true;
        }
        if (initMove) {
            canMove = true;
        }
    }
}
