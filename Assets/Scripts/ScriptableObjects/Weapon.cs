using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Festival/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private BulletData bullet;
    public BulletData Bullet { get => bullet; }
}
