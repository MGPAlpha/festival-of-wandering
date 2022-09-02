using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;



[CreateAssetMenu(fileName = "New Enemy", menuName = "Festival/Enemy")]
public class EnemyData : ScriptableObject {
    [SerializeField] private string enemyName = "New Enemy";
    public string EnemyName { get => enemyName; }
    [SerializeField] private float moveSpeed = 1f;
    public float MoveSpeed { get => moveSpeed; }
    [SerializeField] private List<Weapon> weapons;
    public ReadOnlyCollection<Weapon> Weapons { get => weapons.AsReadOnly(); }
}
