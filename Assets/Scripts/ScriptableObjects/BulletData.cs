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

    [SerializeField] private float velocity = 5;
    public float Velocity { get => velocity; }
    [SerializeField] private float velocityRandomness = 0;
    public float VelocityRandomness { get => velocityRandomness; }
    [SerializeField] private float acceleration = 0;
    public float Acceleration { get => acceleration; }
    [SerializeField] private float angularVelocity = 0;
    public float AngularVelocity { get => angularVelocity; }
    [SerializeField] private float angularVelocityRandomness = 0;
    public float AngularVelocityRandomness { get => angularVelocityRandomness; }
    [SerializeField] private float angularAcceletation = 0;
    public float AngularAcceleration { get => angularAcceletation; }

    [SerializeField] private float colliderRadius = .2f;
    public float ColliderRadius { get => colliderRadius; }
}
