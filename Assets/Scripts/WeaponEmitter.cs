using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEmitter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private LayerMask hitLayers;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(Weapon weapon, Vector2 direction) {
        GameObject newBullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        BulletController controller = newBullet.GetComponent<BulletController>();
        controller.Init(weapon.Bullet, transform.position, direction, hitLayers);
    }
}
