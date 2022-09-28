using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Linq;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Festival/Enemy")]
public class EnemyData : ScriptableObject {
    [SerializeField] private string enemyName = "New Enemy";
    public string EnemyName { get => enemyName; }
    [SerializeField] private float moveSpeed = 1f;
    public float MoveSpeed { get => moveSpeed; }
    [SerializeField] private float baseHealth = 3;
    public float BaseHealth { get => baseHealth; }
    [SerializeField] private float initTime = 0;
    public float InitTime { get => initTime; }
    [SerializeField] private float moveTime = 4;
    public float MoveTime { get => moveTime; }
    [SerializeField] private float aimTime = 2;
    public float AimTime { get => aimTime; }
    [SerializeField] private float cooldownTime = .5f;
    public float CooldownTime { get => cooldownTime; }
    [SerializeField] private float speedWhileAiming = 0;
    public float SpeedWhileAiming { get => speedWhileAiming; }
    [SerializeField] private float speedWhileFiring = 0;
    public float SpeedWhileFiring { get => speedWhileFiring; }
    [Tooltip("Enemy must have line of sight for this long before aiming")][SerializeField] private float lineOfSightTime = .5f;
    public float LineOfSightTime { get => lineOfSightTime; }
    [SerializeReference] private List<EnemyAttackChoice> weapons;
    public ReadOnlyCollection<EnemyAttackChoice> Weapons { get => weapons.AsReadOnly(); }
}

[System.Serializable]
public abstract class EnemyAttackChoice {

    [SerializeField] private Weapon weapon;
    public Weapon Weapon { get; private set; }
    public abstract bool ShouldChoose(Enemy enemy);

    public static Weapon ChooseFromList(IEnumerable<EnemyAttackChoice> list, Enemy enemy) {
        foreach (EnemyAttackChoice choice in list) {
            if (choice.ShouldChoose(enemy)) {
                return choice.weapon;
            }
        }
        return null;
    }
}

[System.Serializable]
public class AttackChoiceAlways : EnemyAttackChoice {
    public override bool ShouldChoose(Enemy enemy) => true;
    public static string GetSubclassName() => "Select Always";
}

[System.Serializable]
public class AttackChoiceChance : EnemyAttackChoice {
    
    [SerializeField] private float probability = .5f;

    public override bool ShouldChoose(Enemy enemy)
    {
        float v = Random.value;
        Debug.Log(v);
        return v < probability;
    }

    public static string GetSubclassName() => "Select by Chance";
}

