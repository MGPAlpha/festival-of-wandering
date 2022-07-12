using System;
using UnityEngine;

[Serializable]
public class PlayerAttack {
    [SerializeField] private AnimationClip animationClip;
    public AnimationClip AnimationClip {
        get { return animationClip; }
    }
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
    [SerializeField] private float comboTimeLimit;
    public float ComboTimeLimit {
        get { return comboTimeLimit; }
    }
}