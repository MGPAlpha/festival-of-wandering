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

    private bool initAttack;
    private bool initMove;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool canMove;
    [SerializeField] private CinemachineCameraOffset _cco;
    [SerializeField] private CameraOffset cameraOffset;
    [SerializeField] private GameObject fireworkWavePrefab;

    [SerializeField] private PlayerWeaponBase[] weapons = new PlayerWeaponBase[2];
    [SerializeField] private Memento spell;

    private Vector2 moveDir;
    private Vector2 aimDir;
    private Vector2? targetPos;

    [SerializeField] private int maxHealth = 6;
    [SerializeField] private int health;
    
    public bool CanHeal {get => health < maxHealth; }

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float maxAimOffset = 2;

    [SerializeField] private InteractManager interactManager;

    private bool dodging = false;
    [SerializeField] private float dodgeSpeedFactor = 1.75f;
    [SerializeField] private float dodgeTime = 0.75f;

    [SerializeField] private float tempInvincibilityTime = 1;
    private float tempInvincibilityRemaining = 0;
    public bool IsDodgeInvincible { get => dodging && dodgeTimer < dodgeTime * 7/8; }
    public bool IsTempInvincible { get => tempInvincibilityRemaining > 0; }
    public bool IsInvincible { get => IsTempInvincible || IsDodgeInvincible; } // Change once invincibility cheat available

    [SerializeField] private List<Checkpoint> checkpoints;

    private int mementoCharge;

    private ParticleSystem _ps;

    public int MaxHealth => maxHealth;
    public int Health => health;
    public PlayerWeaponBase[] Weapons => weapons;
    public float MementoChargePercentage => (spell && spell.ChargeRequired > 0) ? mementoCharge / (float) spell.ChargeRequired : 1f;
    public Memento Spell => spell;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
        _sp = GetComponent<SpriteRenderer>();
        _ps = GetComponentInChildren<ParticleSystem>();
        _attackComponent = GetComponentInChildren<PlayerAttackComponent>();
        _weaponEmitter = GetComponentInChildren<WeaponEmitter>();
        canMove = true;
        canAttack = true;
        health = maxHealth;
        if (spell) mementoCharge = spell.ChargeRequired;
    }

    public bool Damage(int amount, GameObject src) {
        if (!dead && !IsInvincible) {
            health -= amount;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Combat/combat_hit", transform.position);
            tempInvincibilityRemaining = tempInvincibilityTime;
            if (health <= 0) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Combat/combat_death", transform.position);
                StartCoroutine(Die());
            }
            return true;
        } return false;
    }

    float escHoldTime = 0;

    void OnControlsChanged() {
        if (!_input) _input = GetComponent<PlayerInput>();
        Debug.Log("Controls changed and input is " + _input);
        if (_input && OnControlSchemeChanged != null) OnControlSchemeChanged.Invoke(_input.currentControlScheme);
    }

    public System.Action<string> OnControlSchemeChanged;

    // Update is called once per frame
    void Update()
    {

        if (_input.actions["Exit"].IsPressed()) {
            escHoldTime += Time.deltaTime;
            if (escHoldTime > 3) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        } else {
            escHoldTime = 0;
        }

        if (_cco)
        {
            Vector3 offset = Vector2.ClampMagnitude(aimDir, 1) * maxAimOffset;
            cameraOffset.OffsetTo(offset);
        }
        if (!canMove) {
            moveDir = Vector2.zero;
            _rb.velocity = Vector2.zero;
            return;
        }
        
        if (tempInvincibilityRemaining > 0) {
            tempInvincibilityRemaining -= Time.deltaTime;
        }

        if (IsTempInvincible) {
            _sp.enabled = Time.time % .3 > .15f;
        } else {
            _sp.enabled = true;
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
        // if (canMove) {
        aimDir = _input.currentActionMap["Gamepad Aim"].ReadValue<Vector2>();
        // }
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
        if (weapons[0] && canAttack) {
            _attackComponent.TriggerWeapon(weapons[0], aimDir);
            _an.SetTrigger("attack");
        }
    }

    void OnSecondaryAttack() {
        if (weapons[1] && canAttack) {
            _attackComponent.TriggerWeapon(weapons[1], aimDir);
            _an.SetTrigger("attack");
        }
    }

    void OnSpell() {
        if (!canAttack || !spell || _weaponEmitter.FiringActive || mementoCharge < spell.ChargeRequired) return;
        _weaponEmitter.Fire(spell.Weapon, aimDir);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Combat/pew", transform.position);
        _an.SetTrigger("spell");
        mementoCharge = 0;
    }
    
    void OnInteract() {
        if (canMove) {
            interactManager.Interact();
        }
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

    public void ChargeMemento(int charge) {
        if (!spell) return;
        mementoCharge += charge;
        mementoCharge = Mathf.Min(mementoCharge, spell.ChargeRequired);
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    //private void OnGUI()
    //{
    //    // GUIStyle healthStyle = new GUIStyle();
    //    // healthStyle.fontSize = 30;
    //    // GUI.skin.label.fontSize = 20;
    //    GUILayout.Label("Health: " + health + "/" + maxHealth);
    //    if (weapons[0]) {
    //        GUILayout.Label("Current Weapon: " + (weapons[0] ? (weapons[0].WeaponName + " (Left Click or Right Trigger)") : "None"));
    //    }
    //    if (spell) {
    //        GUILayout.Label("Current Memento: " + spell.MementoName + " (Press E or Right Bumper)");
    //        GUILayout.Label("Memento Charge: " + mementoCharge + "/" + spell.ChargeRequired);
    //    }
    //    if (fireworkSupply > 0) {
    //        GUILayout.Label("Firework Charms: " + fireworkSupply + " remaining (Press Q or Left Bumper)");
    //    }
    //    if (dead) {
    //        GUILayout.Label("You died!");
    //    }
    //    if (interactManager.CanInteract) {
    //        GUILayout.Label("Press F on keyboard or X on controller to Interact!");
    //    }

    //}

    float dodgeTimer;

    IEnumerator Dodge(Vector2 dir) {
        dodging = true;
        _an.SetFloat("facingX", dir.x);
        _an.SetFloat("facingY", dir.y);
        _ps.Play();
        float dodgeSpeed = dodgeSpeedFactor * baseSpeed;
        _rb.velocity = dodgeSpeed * dir;
        dodgeTimer = 0;
        while (dodgeTimer < dodgeTime)
        {
            dodgeTimer += Time.deltaTime;
            yield return null;
        }
        dodging = false;
        _ps.Stop();
    }

    void OnDodge() {
        if (!dodging && canMove) {
            Vector2 dodgeDir = moveDir;
            if (moveDir == Vector2.zero) dodgeDir = aimDir;
            StartCoroutine(Dodge(dodgeDir.normalized));
        }
    }

    [SerializeField] private int fireworkSupply = 2;
    public int FireworkSupply => fireworkSupply;
    public void GainFireworks(int amount) {
        fireworkSupply += amount;
    }
    public void ResupplyFireworks(int amount) {
        fireworkSupply = Mathf.Max(fireworkSupply, amount);
    }

    void OnFirework() {
        if (canAttack && fireworkSupply > 0) {
            Instantiate(fireworkWavePrefab, transform.position, Quaternion.identity);
            fireworkSupply--;
        }
    }

    public void SetPrimary(PlayerWeaponBase weapon) {
        weapons[0] = weapon;
    }

    public void SetSecondary(PlayerWeaponBase weapon) {
        weapons[1] = weapon;
    }

    public void SetSpell(Memento spell) {
        this.spell = spell;
        if (spell) mementoCharge = spell.ChargeRequired;
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

    public void Heal(int amount) {
        health += amount;
        health = Mathf.Min(health, maxHealth);
    }

    public void MaxHealthUp(int amount) {
        maxHealth += amount;
        health += amount;
    }

    [SerializeField] private float deathTime = 3;
    private bool dead = false;
    private IEnumerator Die() {
        initAttack = canAttack;
        initMove = canMove;
        
        canAttack = false;
        canMove = false;
        dead = true;

        float deathTimer = 0;
        while (deathTimer < deathTime) {
            deathTimer += Time.deltaTime;
            yield return null;
        }
        ResupplyFireworks(2);
        RespawnAtLastCheckpoint();
    }

    public void AddCheckpoint(Checkpoint checkpoint) {
        if (!checkpoints.Contains(checkpoint)) {
            checkpoints.Add(checkpoint);
        }
    }

    private void RespawnAtLastCheckpoint() {
        Checkpoint lastCheckpoint = checkpoints[checkpoints.Count-1];
        transform.position = lastCheckpoint.transform.position;
        health = maxHealth;
        lastCheckpoint.ResetCheckpoint();
        StopDialogue();
        dead = false;
    }
}
