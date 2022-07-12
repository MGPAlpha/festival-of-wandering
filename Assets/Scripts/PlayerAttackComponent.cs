using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerAttackComponent : MonoBehaviour
{
    
    private Animator _an;
    private BoxCollider2D _col;
    private SpriteRenderer _sp;
    private AnimatorOverrideController _ao;

    private bool attacking = false;

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
        
    }

    public void TriggerWeapon(PlayerWeaponBase weapon, Vector2 direction) {
        ReadOnlyCollection<PlayerAttack> attacks = weapon.GetAttacks();
        PlayerAttack currAttack = attacks[0];
        _col.offset = currAttack.ColliderCenter;
        _col.size = currAttack.ColliderSize;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90);
        transform.localPosition = transform.rotation * currAttack.PivotOffset;
        _ao["Attack"] = currAttack.AnimationClip;
        _an.Play("Attacking");
        _sp.enabled = true;
        attacking = true;
        Debug.Log("Playing anim");
    }
}
