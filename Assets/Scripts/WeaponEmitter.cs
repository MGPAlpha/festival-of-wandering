using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class WeaponEmitter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] public LayerMask hitLayers;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Coroutine> activeBursts = new List<Coroutine>();
    private int activeBurstCount = 0;
    public bool FiringActive { get => activeBurstCount > 0; }

    public void Fire(Weapon weapon, Vector2 direction) {
        
        ReadOnlyCollection<WeaponBurst> bursts = weapon.Bursts;
        foreach (WeaponBurst burst in bursts) {
            activeBursts.Add(StartCoroutine(FireBurstCoroutine(burst, direction)));
        }
    }

    private BulletController FireNewBullet(BulletData bullet, Vector2 direction) {
        GameObject newBullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        BulletController controller = newBullet.GetComponent<BulletController>();
        controller.Init(bullet, transform.position, direction, hitLayers);
        return controller;
    }

    IEnumerator FireBurstCoroutine(WeaponBurst burst, Vector2 direction) {
        activeBurstCount++;
        if (burst.StartTime > 0) yield return new WaitForSeconds(burst.StartTime);
        float localAimAngle = burst.AimOffset;
        if (burst.BulletsInSpread > 1) localAimAngle -= burst.FiringSpread/2 * (burst.SpreadClockwise ? -1 : 1);
        float angleBetweenBullets = burst.FiringSpread / (burst.BulletsInSpread - (burst.FiringSpread == 360 ? 0 : 1));
        angleBetweenBullets *=  (burst.SpreadClockwise ? -1 : 1);
        for (int i = 0; i < burst.BulletsInSpread; i++) {
            // if (i == 0 && burst.BulletsInSpread > 1 && burst.FiringSpread == 360 && burst.SpreadTimeInterval == 0) continue; 
            if (burst.SpreadTimeInterval > 0 && i != 0) yield return new WaitForSeconds(burst.SpreadTimeInterval);
            float thisBulletAimAngle = localAimAngle;
            if (burst.SpreadRandomization > 0) {
                thisBulletAimAngle += Random.Range(-burst.SpreadRandomization/2, burst.SpreadRandomization/2);
            }
            Vector2 bulletDirection = direction.Rotate(thisBulletAimAngle);
            FireNewBullet(burst.Bullet, bulletDirection);
            if (burst.BulletsInSpread > 1) localAimAngle += angleBetweenBullets;
        }
        activeBurstCount--;
    }
}
