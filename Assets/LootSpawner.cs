using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class LootSpawner : MonoBehaviour
{
    
    [SerializeField] private LootTable lootTable;
    [SerializeField] private GameObject pickupPrefab;


    private bool spawned = false;

    [YarnCommand("spawn_loot")]
    public GameObject Spawn() {
        if (!lootTable) return null;
        LootProbability loot = lootTable.Choose();
        if (loot == null || loot.type == GenericPickup.PickupType.None) return null;
        GenericPickup newPickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity).GetComponent<GenericPickup>();
        bool shouldAutoPickup = new List<GenericPickup.PickupType>{GenericPickup.PickupType.Heal, GenericPickup.PickupType.HalfHeal, GenericPickup.PickupType.FireworkCharm}.Contains(loot.type);
        newPickup.InitializeBeforeStart(loot.type, shouldAutoPickup);
        return newPickup.gameObject;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
