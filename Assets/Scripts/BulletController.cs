using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletKillReason {
    Timeout,
    Hit,
    Wall,
    Firework
}

public class BulletController : MonoBehaviour
{
    private BulletData bulletData;
    private LayerMask hitLayers;
    private float lifetimeRemaining;

    private bool isDead;
    private BulletKillReason deathReason;

    private Animator _an;
    private SpriteRenderer _sp;
    private AnimatorOverrideController _ao;
    private int animationSpeedParameter;

    private CircleCollider2D _cc;
    private Rigidbody2D _rb;
    private WeaponEmitter _we;
    private ParticleSystem _ps;

    private Vector2 movementDir;
    private float movementSpeed;
    private float movementAcceleration;
    private float angularVelocity;
    private float angularAcceletation;
    private float angularSwitchTime;
    private float angularSwitchTimer;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _cc = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
        _we = GetComponentInChildren<WeaponEmitter>();
        _ps = GetComponentInChildren<ParticleSystem>();
        _sp = GetComponent<SpriteRenderer>();
        _ao = new AnimatorOverrideController(_an.runtimeAnimatorController);
        _an.runtimeAnimatorController = _ao;
        animationSpeedParameter = Animator.StringToHash("Animation Speed");
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (isDead && !_we.FiringActive && deathReason != BulletKillReason.Firework) {
            Destroy(this.gameObject);
        }
        if (isDead && deathReason == BulletKillReason.Firework && !_ps.IsAlive()) {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        if (isDead) return;
        if (angularSwitchTime > 0) {
            angularSwitchTimer -= Time.fixedDeltaTime;
            if (angularSwitchTimer < 0) {
                angularSwitchTimer += angularSwitchTime;
                angularVelocity *= -1;
            }
        }
        angularVelocity += angularAcceletation * Time.fixedDeltaTime;
        movementDir = movementDir.Rotate(angularVelocity * Time.fixedDeltaTime);
        movementSpeed += movementAcceleration * Time.fixedDeltaTime;
        Vector2 displacement = movementDir * movementSpeed * Time.fixedDeltaTime;
        transform.position += new Vector3(displacement.x, displacement.y);
        lifetimeRemaining -= Time.fixedDeltaTime;
        if (lifetimeRemaining < 0 && bulletData.Lifetime != 0 || bulletData.Lifetime == 0 && movementSpeed < 0) {
            Kill(BulletKillReason.Timeout);
        }
    }

    public void Init(BulletData bullet, Vector3 pos, Vector2 direction, LayerMask hit) {
        isDead = false;
        bulletData = bullet;
        hitLayers = hit;
        transform.position = pos;
        lifetimeRemaining = bullet.Lifetime;
        _ao["Basic Bullet Animation"] = null;
        _sp.enabled = true;
        _an.SetFloat(animationSpeedParameter, bulletData.AnimationSpeed);
        movementDir = direction.normalized;
        movementDir = movementDir.Rotate(-bullet.AngularVelocity * bullet.AngularSwitchTime / 2);
        movementSpeed = bullet.Velocity;
        movementSpeed += Random.Range(0, bullet.VelocityRandomness);
        movementAcceleration = bullet.Acceleration;
        angularVelocity = bullet.AngularVelocity;
        angularVelocity += Random.Range(-bullet.AngularVelocityRandomness, bullet.AngularVelocityRandomness);
        angularAcceletation = bullet.AngularAcceleration;
        angularSwitchTime = bullet.AngularSwitchTime;
        angularSwitchTimer = angularSwitchTime;
        _cc.radius = bullet.ColliderRadius;
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Firework Wave") {
            Kill(BulletKillReason.Firework);
        } else if (hitLayers == (hitLayers | (1 << other.gameObject.layer))) { // If other is hittable
            if (other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)) {
                bool successfulHit = damageable.Damage(1);
                if (!successfulHit) return;
                if (other.attachedRigidbody) {
                    if (bulletData.KnockbackForce > 0)
                        other.attachedRigidbody.AddForce(bulletData.KnockbackForce * this.movementDir, ForceMode2D.Impulse);
                    
                }
                Kill(BulletKillReason.Hit);
            }
        } else if (other.gameObject.layer == 0) { // Layer is default
            Kill(BulletKillReason.Wall);
        }
    }

    public void Kill(BulletKillReason reason) {
        bool doCluster = reason == BulletKillReason.Timeout && bulletData.BurstOnTimeout || reason == BulletKillReason.Wall && bulletData.BurstOnWall;
        if (bulletData.ClusterWeapon && doCluster) {
            _sp.enabled = false;
            Vector2 clusterAimDirection;
            if (reason == BulletKillReason.Wall) {
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, bulletData.ColliderRadius, movementDir, 1, 1);
                clusterAimDirection = hit.normal;
                RaycastHit2D normalHit = Physics2D.Raycast(transform.position, -hit.normal, bulletData.ColliderRadius*2, 1);
                transform.position += (bulletData.ClusterWallOffset + normalHit.distance) * new Vector3(hit.normal.x, hit.normal.y);
            } else if (bulletData.ClusterAimMode == BulletClusterAimMode.Absolute) {
                clusterAimDirection = Vector2.up;
            } else {
                clusterAimDirection = movementDir.normalized;
            }
            clusterAimDirection = clusterAimDirection.Rotate(bulletData.ClusterAimOffset);
            _we.Fire(bulletData.ClusterWeapon, clusterAimDirection);
        } else if (reason == BulletKillReason.Firework) {
            if (_we.FiringActive) {
                Destroy(this.gameObject);
            } else if (!isDead) {
                ParticleSystem.MainModule psMain = _ps.main;
                psMain.startColor = Random.ColorHSV(0,1f, 1f, 1f, 1f, 1f);
                _ps.Play();
                _sp.enabled = false;
            }
        } else {
            Destroy(this.gameObject);
        }
        isDead = true;
        deathReason = reason;
    }
}
