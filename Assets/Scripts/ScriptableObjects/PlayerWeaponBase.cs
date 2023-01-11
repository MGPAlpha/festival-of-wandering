using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Weapon", menuName = "Festival/Player Weapon")]
public class PlayerWeaponBase : ScriptableObject
{
    [SerializeField] private string weaponName;
    public string WeaponName {
        get { return weaponName; }
    }
    [SerializeField] private Sprite weaponIcon;
    public Sprite WeaponIcon { get => weaponIcon; }
    [SerializeField] private bool isHeavy;
    public bool IsHeavy {
        get { return isHeavy; }
    }
    [SerializeField] private List<PlayerAttack> attacks;
    public ReadOnlyCollection<PlayerAttack> GetAttacks() {
        return attacks.AsReadOnly();
    }
}
