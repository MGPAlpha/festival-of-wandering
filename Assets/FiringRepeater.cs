using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class FiringRepeater : MonoBehaviour
{
    [SerializeField] private bool isActive;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float interval;
    [SerializeField] private Weapon weapon;

    private WeaponEmitter _we;

    private float intervalTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        intervalTimer = interval;
        _we = GetComponentInChildren<WeaponEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        intervalTimer += Time.deltaTime;
        if (intervalTimer > interval) {
            intervalTimer -= interval;
            if (weapon) _we.Fire(weapon, direction);
        }
    }

    [YarnCommand("firingRepeaterActive")]
    void setActive(bool on) {
        isActive = on;
    }
}
