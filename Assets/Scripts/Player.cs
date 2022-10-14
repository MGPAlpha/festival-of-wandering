using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;


public class Player : MonoBehaviour, IDamageable
{

    private PlayerInput _input;
    private Rigidbody2D _rb;
    private Animator _an;
    private SpriteRenderer _sp;
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

    [SerializeField] private int maxHealth = 6;
    private int health;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float maxAimOffset = 2;

    [SerializeField] private InteractManager interactManager;

    private bool dodging = false;
    [SerializeField] private float dodgeSpeedFactor = 1.75f;
    [SerializeField] private float dodgeTime = 1;

    [SerializeField] private float tempInvincibilityTime = 1;
    private float tempInvincibilityRemaining = 0;
    public bool IsDodgeInvincible { get => dodging && dodgeTimer < dodgeTime * 7/8; }
    public bool IsTempInvincible { get => tempInvincibilityRemaining > 0; }
    public bool IsInvincible { get => IsTempInvincible || IsDodgeInvincible; } // Change once invincibility cheat available

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
        _sp = GetComponent<SpriteRenderer>();
        _attackComponent = GetComponentInChildren<PlayerAttackComponent>();
        _weaponEmitter = GetComponentInChildren<WeaponEmitter>();
        inDialog = false;
        canAttack = true;
        health = maxHealth;
    }

    public bool Damage(int amount) {
        if (!IsInvincible) {
            health -= amount;
            tempInvincibilityRemaining = tempInvincibilityTime;
            Debug.Log("New player health: " + health);
            return true;
        } return false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (tempInvincibilityRemaining > 0) {
            tempInvincibilityRemaining -= Time.deltaTime;
        }

        if (IsTempInvincible) {
            _sp.enabled = Time.time % .3 > .15f;
        } else {
            _sp.enabled = true;
        }

        moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
        if (_cco) {
            _cco.m_Offset = Vector2.ClampMagnitude(aimDir, 1) * maxAimOffset;
        }
        if (aimDir.magnitude == 0) aimDir = moveDir;
        if (dodging) return;
        Vector2 normalizedAim = aimDir.normalized;
        _an.SetFloat("facingX", normalizedAim.x);
        _an.SetFloat("facingY", normalizedAim.y);
        _an.SetBool("walking", moveDir.magnitude > 0);
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

    float dodgeTimer;

    IEnumerator Dodge(Vector2 dir) {
        dodging = true;
        _an.SetFloat("facingX", dir.x);
        _an.SetFloat("facingY", dir.y);
        float dodgeSpeed = dodgeSpeedFactor * baseSpeed;
        _rb.velocity = dodgeSpeed * dir;
        dodgeTimer = 0;
        while (dodgeTimer < dodgeTime)
        {
            dodgeTimer += Time.deltaTime;
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

    public void SetSpell(Weapon spell) {
        this.spell = spell;
    }
}
