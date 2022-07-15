using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerAttackComponent : MonoBehaviour
{
    
    private Animator _an;
    private BoxCollider2D _col;
    private SpriteRenderer _sp;
    private AnimatorOverrideController _ao;

    HashSet<GameObject> hits = new HashSet<GameObject>();

    PlayerAttack currAttack;
    PlayerAttack currAttackIndex;
    PlayerWeaponBase currWeapon;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (currAttack == null) return;
        
        attackTime += Time.deltaTime;
        if (!hitboxActive && attackTime > currAttack.AttackStartTime && attackTime < currAttack.AttackStartTime + currAttack.AttackDuration) {
            hitboxActive = true;
        }
        if (hitboxActive && attackTime > currAttack.AttackStartTime + currAttack.AttackDuration) {
            hitboxActive = false;
        }
        if (!canAttack && attackTime > currAttack.AttackStartTime + currAttack.AttackDuration + currAttack.AttackCooldown) {
            canAttack = true;
        }
        if (canAttack && canCombo && attackTime > currAttack.AttackStartTime + currAttack.AttackDuration + currAttack.AttackCooldown + currAttack.ComboTimeLimit) {
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
        currAttack = attacks[comboIndex];
        _col.offset = currAttack.ColliderCenter;
        _col.size = currAttack.ColliderSize;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90);
        transform.localPosition = transform.rotation * currAttack.PivotOffset;
        _ao["Attack"] = currAttack.AnimationClip;
        _an.Play("Attacking");
        _sp.enabled = true;
        Attacking = true;
        canAttack = false;
        hitboxActive = false;
        canCombo = true;
        attackTime = 0;
        hits.Clear();
        comboIndex++;
    }
}
