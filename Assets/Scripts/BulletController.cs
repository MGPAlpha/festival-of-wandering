using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private BulletData bulletData;
    private LayerMask hitLayers;
    private float lifetimeRemaining;

    private Animator _an;
    private AnimatorOverrideController _ao;
    private int animationSpeedParameter;

    private CircleCollider2D _cc;
    private Rigidbody2D _rb;

    private Vector2 movementDir;
    private float movementSpeed;
    private float movementAcceleration;
    private float angularVelocity;
    private float angularAcceletation;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _cc = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
        _ao = new AnimatorOverrideController(_an.runtimeAnimatorController);
        _an.runtimeAnimatorController = _ao;
        animationSpeedParameter = Animator.StringToHash("Animation Speed");
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        angularVelocity += angularAcceletation * Time.fixedDeltaTime;
        movementDir = movementDir.Rotate(angularVelocity * Time.fixedDeltaTime);
        movementSpeed += movementAcceleration * Time.fixedDeltaTime;
        Vector2 displacement = movementDir * movementSpeed * Time.fixedDeltaTime;
        transform.position += new Vector3(displacement.x, displacement.y);
        lifetimeRemaining -= Time.fixedDeltaTime;
        if (lifetimeRemaining < 0) {
            Destroy(this.gameObject);
        }
    }

    public void Init(BulletData bullet, Vector3 pos, Vector2 direction, LayerMask hit) {
        bulletData = bullet;
        hitLayers = hit;
        transform.position = pos;
        lifetimeRemaining = bullet.Lifetime;
        _ao["Basic Bullet Animation"] = null;
        _an.SetFloat(animationSpeedParameter, bulletData.AnimationSpeed);
        movementDir = direction.normalized;
        movementSpeed = bullet.Velocity;
        movementSpeed += Random.Range(0, bullet.VelocityRandomness);
        movementAcceleration = bullet.Acceleration;
        angularVelocity = bullet.AngularVelocity;
        angularVelocity += Random.Range(-bullet.AngularVelocityRandomness, bullet.AngularVelocityRandomness);
        angularAcceletation = bullet.AngularAcceleration;
        _cc.radius = bullet.ColliderRadius;
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitLayers == (hitLayers | (1 << other.gameObject.layer))) { // If other is hittable

        } else if (other.gameObject.layer == 0) { // Layer is default
            Destroy(this.gameObject);
        }
    }
}
