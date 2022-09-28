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
    private GameObject target;

    public EnemyData enemyType;
    EnemyState state;
    private float stateTime = 0;
    private float lineOfSightTime = 0;
    [SerializeField] private LayerMask sightBlockingLayers;

    private int health;
    private int maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        _nma = GetComponent<NavMeshAgent>();
        _nma.updateRotation = false;
        _nma.updateUpAxis = false;
        _we = GetComponentInChildren<WeaponEmitter>();
        if (enemyType) Initialize(enemyType);
    }

    void AcquireTarget() {
        target = GameObject.Find("Player");
    }

    void Initialize(EnemyData enemy) {
        state = EnemyState.INIT;
        stateTime = 0;
        enemyType = enemy;
        health = enemy.BaseHealth;
        maxHealth = enemy.BaseHealth;
        _nma.speed = 0;
        target = GameObject.Find("Player");
    }

    public void Damage(int amount) {
        if (state == EnemyState.DEAD) return;
        health -= amount;
        if (health <= 0) {
            Die();
        }
    }

    // Update is called once per frame
    void Update()
    {
        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        _nma.SetDestination(target.transform.position);
        stateTime += Time.deltaTime;
        switch (state) {
            case EnemyState.INIT:
                if (stateTime > enemyType.InitTime) {
                    TransitionMove();
                }
                break;
            case EnemyState.MOVE:
                Vector3 targetDir = target.transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)targetDir.normalized, targetDir.magnitude, sightBlockingLayers);
                if (hit.collider) {
                    lineOfSightTime = 0;
                } else lineOfSightTime += Time.deltaTime;
                if (stateTime > enemyType.MoveTime && lineOfSightTime > enemyType.LineOfSightTime) {
                    TransitionAim();
                }
                break;
            case EnemyState.AIM:
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
                break;
        }
    }

    private void TransitionMove() {
        stateTime = 0;
        state = EnemyState.MOVE;
        _nma.speed = enemyType.MoveSpeed;
        lineOfSightTime = 0;
    }

    private void TransitionAim() {
        stateTime = 0;
        state = EnemyState.AIM;
        _nma.speed = enemyType.SpeedWhileAiming;
    }

    private void TransitionFire() {
        stateTime = 0;
        state = EnemyState.FIRE;
        _nma.speed = enemyType.SpeedWhileFiring;
        Weapon weapon = EnemyAttackChoice.ChooseFromList(enemyType.Weapons, this);
        _we.Fire(weapon, (target.transform.position - transform.position).normalized);
    }

    private void TransitionCooldown() {
        stateTime = 0;
        state = EnemyState.COOLDOWN;
        _nma.speed = 0;
    }

    private void Die() {
        stateTime = 0;
        state = EnemyState.DEAD;
        _nma.speed = 0;
    }
}
