using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _nma;
    private GameObject target;

    public EnemyData enemyType;
    
    // Start is called before the first frame update
    void Start()
    {
        _nma = GetComponent<NavMeshAgent>();
        _nma.updateRotation = false;
        _nma.updateUpAxis = false;
        if (enemyType) Initialize(enemyType);
    }

    void AcquireTarget() {
        target = GameObject.Find("Player");
    }

    void Initialize(EnemyData enemy) {
        enemyType = enemy;
        _nma.speed = enemyType.MoveSpeed;
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        _nma.SetDestination(target.transform.position);
    }
}
