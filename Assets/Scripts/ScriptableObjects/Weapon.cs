using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Festival/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private List<WeaponBurst> bursts;
    public ReadOnlyCollection<WeaponBurst> Bursts {
        get => bursts.AsReadOnly();
    }
}

[Serializable]
public class WeaponBurst {
    [SerializeField] private float startTime = 0;
    public float StartTime { get => startTime; }
    
    [SerializeField] private BulletData bullet;
    public BulletData Bullet { get => bullet; }

    [SerializeField] private float firingSpread = 0;
    public float FiringSpread { get => firingSpread; }
    [SerializeField] private int bulletsInSpread = 1;
    public int BulletsInSpread { get => bulletsInSpread; }
    [SerializeField] private float spreadTimeInterval = 0;
    public float SpreadTimeInterval { get => spreadTimeInterval; }
    [SerializeField] private float spreadRandomization = 0;
    public float SpreadRandomization { get => spreadRandomization; }

    [SerializeField] private float aimOffset = 0;
    public float AimOffset { get => aimOffset; }
}
