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
    [SerializeReference] private List<EnemyAttackChoice> weapons;
    public ReadOnlyCollection<EnemyAttackChoice> Weapons { get => weapons.AsReadOnly(); }
}

[System.Serializable]
public abstract class EnemyAttackChoice {

    [SerializeField] private Weapon weapon;
    public Weapon Weapon { get; private set; }
    public abstract bool ShouldChoose();
}

[System.Serializable]
public class AttackChoiceAlways : EnemyAttackChoice {
    public override bool ShouldChoose() => true;
    public static string GetSubclassName() => "Select Always";
}

[System.Serializable]
public class AttackChoiceChance : EnemyAttackChoice {
    
    [SerializeField] private float probability = .5f;

    public override bool ShouldChoose()
    {
        return Random.Range(0,1) < probability;
    }

    public static string GetSubclassName() => "Select by Chance";
}

