using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerAttackComponent : MonoBehaviour
{
    
    private Animator _an;
    private BoxCollider2D _col;
    private SpriteRenderer _sp;
    private AnimatorOverrideController _ao;
    private int animationSpeedParameter;

    HashSet<IDamageable> hits = new HashSet<IDamageable>();

    public PlayerAttack CurrAttack { get; private set; }
    PlayerAttack currAttackIndex;
    PlayerWeaponBase currWeapon;
    private Vector2 attackDirection;
    private bool hitboxActive = false;
    private float attackTime = 0;
    public bool Attacking {
        get; private set;
    }
    private bool canAttack = true;
    private bool canCombo = false;
    private int comboIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _an = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _sp = GetComponent<SpriteRenderer>();
        _sp.enabled = false;
        _ao = new AnimatorOverrideController(_an.runtimeAnimatorController);
        _an.runtimeAnimatorController = _ao;
        animationSpeedParameter = Animator.StringToHash("Attack Speed");
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrAttack == null) return;
        
        attackTime += Time.deltaTime;
        if (!hitboxActive && attackTime > CurrAttack.AttackStartTime && attackTime < CurrAttack.AttackStartTime + CurrAttack.AttackDuration) {
            hitboxActive = true;
        }
        if (hitboxActive && attackTime > CurrAttack.AttackStartTime + CurrAttack.AttackDuration) {
            hitboxActive = false;
        }
        if (!canAttack && attackTime > CurrAttack.AttackStartTime + CurrAttack.AttackDuration + CurrAttack.AttackCooldown) {
            canAttack = true;
            Attacking = false;
        }
        if (canAttack && canCombo && attackTime > CurrAttack.AttackStartTime + CurrAttack.AttackDuration + CurrAttack.AttackCooldown + CurrAttack.ComboTimeLimit) {
            canCombo = false;
        }
    }

    public void TriggerWeapon(PlayerWeaponBase weapon, Vector2 direction) {
        if (!canAttack) return;
        ReadOnlyCollection<PlayerAttack> attacks = weapon.GetAttacks();
        if (canCombo && weapon == currWeapon && comboIndex < attacks.Count) {
            // comboIndex++;
        } else {
            comboIndex = 0;
        }
        currWeapon = weapon;
        CurrAttack = attacks[comboIndex];
        _col.offset = CurrAttack.ColliderCenter;
        _col.size = CurrAttack.ColliderSize;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90);
        transform.localPosition = transform.rotation * CurrAttack.PivotOffset;
        _ao["Attack"] = CurrAttack.AnimationClip;
        _an.SetFloat(animationSpeedParameter, CurrAttack.AnimationSpeed);
        _an.Play("Attacking", -1, 0f);
        _sp.enabled = true;
        Attacking = true;
        canAttack = false;
        hitboxActive = false;
        canCombo = true;
        attackTime = 0;
        hits.Clear();
        attackDirection = direction;
        comboIndex++;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!hitboxActive) return;
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable)) {
            if (hits.Contains(damageable)) return;
            hits.Add(damageable);
            damageable.Damage(1, gameObject.transform.parent.gameObject);
            if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbOther)) {
                rbOther.AddForce(attackDirection * CurrAttack.PushbackForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (hitboxActive) {
            Gizmos.DrawCube(transform.position, Vector3.one/6);
        }
    }
}
