using System;
using UnityEngine;

[Serializable]
public class PlayerAttack {
    [SerializeField] private AnimationClip animationClip;
    public AnimationClip AnimationClip {
        get { return animationClip; }
    }
    [SerializeField] private float animationSpeed = 1f;
    public float AnimationSpeed { get => animationSpeed; }
    [SerializeField] private float movementSpeed = 1f;
    public float MovementSpeed { get => movementSpeed; }
    [SerializeField] private Vector2 pivotOffset;
    public Vector2 PivotOffset {
        get { return pivotOffset; }
    }
    [SerializeField] private Vector2 colliderCenter;
    public Vector2 ColliderCenter {
        get { return colliderCenter; }
    }
    [SerializeField] private Vector2 colliderSize;
    public Vector2 ColliderSize {
        get { return colliderSize; }
    }
    [SerializeField] private float attackStartTime;
    public float AttackStartTime {
        get { return attackStartTime; }
    }
    [SerializeField] private float attackDuration;
    public float AttackDuration {
        get { return attackDuration; }
    }
    [SerializeField] private float attackCooldown;
    public float AttackCooldown {
        get { return attackCooldown; }
    }
    [SerializeField] private float comboTimeLimit;
    public float ComboTimeLimit {
        get { return comboTimeLimit; }
    }
    [SerializeField] private float pushbackForce = 5;
    public float PushbackForce { get => pushbackForce; }

}