using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    INIT,
    MOVE,
    AIM,
    FIRE,
    COOLDOWN,
    DEAD
}

public class Enemy : MonoBehaviour, IDamageable
{
    private NavMeshAgent _nma;
    private WeaponEmitter _we;
    private CircleCollider2D _cc;
    private BoxCollider2D _hitbox;
    private Animator _an;
    private SpriteRenderer _sp;
    private Rigidbody2D _rb;
    private EnemyHitEffect _hitEffect;
    private GameObject target;

    public bool IsDead { get => state == EnemyState.DEAD; }

    public EnemyData enemyType;
    EnemyState state;
    private float stateTime = 0;
    private float lineOfSightTime = 0;
    [SerializeField] private LayerMask sightBlockingLayers;

    [SerializeField] private float spriteDissolveTime = 1;
    [SerializeField] private float spriteDissolveDelay = 1;
    [SerializeField] private float killingBlowForce = 6;

    private int health;
    private int maxHealth;

    private bool attackAnimTriggered;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _nma = GetComponent<NavMeshAgent>();
        _we = GetComponentInChildren<WeaponEmitter>();
        _cc = GetComponent<CircleCollider2D>();
        _an = GetComponent<Animator>();
        _hitbox = GetComponent<BoxCollider2D>();
        _sp = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _hitEffect = GetComponent<EnemyHitEffect>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        AimAssistSystem.RegisterTarget(_hitbox);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        AimAssistSystem.DeleteTarget(_hitbox);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        _nma.updateRotation = false;
        _nma.updateUpAxis = false;
        
        
        if (enemyType) Initialize(enemyType);
    }

    void AcquireTarget() {
        target = GameObject.Find("Player");
    }

    public void Initialize(EnemyData enemy) {
        state = EnemyState.INIT;
        stateTime = 0;
        enemyType = enemy;
        _an.runtimeAnimatorController = enemy.AnimatorController;
        health = enemy.BaseHealth;
        maxHealth = enemy.BaseHealth;
        attackAnimTriggered = false;
        _nma.speed = 0;
        _cc.enabled = true;
        _hitbox.enabled = true;
        _an.speed = 1;
        _an.SetFloat("idleSpeed", enemy.IdleAnimSpeed);
        _an.SetFloat("walkSpeed", enemy.WalkAnimSpeed);
        _an.SetFloat("attackSpeed", enemy.AttackAnimSpeed);
        _hitbox.offset = enemy.HitboxCenter;
        _hitbox.size = enemy.HitboxSize;
        _sp.material.SetFloat("_Dissolve", 0);
        target = GameObject.Find("Player");
    }

    public bool Damage(int amount, GameObject src) {
        // Debug.Log($"Enemy Health: {health}");
        if (state == EnemyState.DEAD) return false;
        health -= amount;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Combat/combat_hit", transform.position);
        _hitEffect.PlayEffect();
        if (health <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Combat/combat_death", transform.position);
            _rb.AddForce((transform.position - src.transform.position).normalized * killingBlowForce, ForceMode2D.Impulse);
            Die();
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        Vector2 nmaDirection = (_nma.steeringTarget - transform.position).normalized;
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        _nma.SetDestination(target.transform.position);
        stateTime += Time.deltaTime;
        switch (state) {
            case EnemyState.INIT:
                if (stateTime > enemyType.InitTime) {
                    TransitionMove();
                }
                break;
            case EnemyState.MOVE:
                _an.SetFloat("facingX", nmaDirection.x);
                _an.SetFloat("facingY", nmaDirection.y);
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)targetDirection.normalized, targetDirection.magnitude, sightBlockingLayers);
                if (hit.collider) {
                    lineOfSightTime = 0;
                } else lineOfSightTime += Time.deltaTime;
                if (stateTime > enemyType.MoveTime && lineOfSightTime > enemyType.LineOfSightTime) {
                    TransitionAim();
                }
                break;
            case EnemyState.AIM:
                _an.SetFloat("facingX", targetDirection.x);
                _an.SetFloat("facingY", targetDirection.y);
                if (!attackAnimTriggered && enemyType.AimTime - stateTime < enemyType.AttackAnimLeadTime) {
                    _an.SetTrigger("attack");
                    attackAnimTriggered = true;
                }
                if (stateTime > enemyType.AimTime) {
                    TransitionFire();
                }
                break;
            case EnemyState.FIRE:
                if (!_we.FiringActive) {
                    TransitionCooldown();
                }
                break;
            case EnemyState.COOLDOWN:
                if (stateTime > enemyType.CooldownTime) {
                    TransitionMove();
                }
                break;
            case EnemyState.DEAD:
                float dissolveProgress = (stateTime - spriteDissolveDelay) / spriteDissolveTime;
                _sp.material.SetFloat("_Dissolve", Mathf.Clamp(dissolveProgress, 0, 1));
                if (dissolveProgress >= 1) {
                    Destroy(this.gameObject);
                }
                break;
        }
    }

    private void TransitionMove() {
        stateTime = 0;
        state = EnemyState.MOVE;
        _nma.speed = enemyType.MoveSpeed;
        _an.SetBool("walking", true);
        lineOfSightTime = 0;
    }

    private void TransitionAim() {
        stateTime = 0;
        state = EnemyState.AIM;
        _nma.speed = enemyType.SpeedWhileAiming;
        _an.SetBool("walking", false);
        attackAnimTriggered = false;
    }

    private void TransitionFire() {
        stateTime = 0;
        if (!attackAnimTriggered) {
            _an.SetTrigger("attack");
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Combat/pew", transform.position);
        state = EnemyState.FIRE;
        _nma.speed = enemyType.SpeedWhileFiring;
        Weapon weapon = EnemyAttackChoice.ChooseFromList(enemyType.Weapons, this);
        _we.Fire(weapon, (target.transform.position - transform.position).normalized);
    }

    private void TransitionCooldown() {
        stateTime = 0;
        state = EnemyState.COOLDOWN;
        _nma.speed = 0;
        attackAnimTriggered = false;
    }

    private void Die() {
        stateTime = 0;
        state = EnemyState.DEAD;
        _nma.speed = 0;
        _cc.enabled = false;
        _hitbox.enabled = false;
        _an.speed = .2f;
    }
}
