using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Bullet", menuName = "Festival/Bullet")]
public class BulletData : ScriptableObject
{
    [SerializeField] private float lifetime = 5;
    public float Lifetime { get => lifetime; }
    [SerializeField] private AnimationClip animation;
    public AnimationClip Animation { get => animation; }
    [SerializeField] private float animationSpeed = 1;
    public float AnimationSpeed { get => animationSpeed; }
    [SerializeField] private bool rotateSprite = false;
    public bool RotateSprite { get => rotateSprite; }

    [SerializeField] private float movementSpeed = 5;
    public float MovementSpeed { get => movementSpeed; }

    [SerializeField] private float colliderRadius = .2f;
    public float ColliderRadius { get => colliderRadius; }
}
