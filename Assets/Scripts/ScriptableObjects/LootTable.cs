using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Loot Table", menuName="Festival/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField]
    public List<LootProbability> lootOptions;

    public LootProbability Choose() {
        
        if (lootOptions.Count == 0) {
            return null;
        }
        float weightSum = 0f;
        foreach (LootProbability loot in lootOptions)
        {
            weightSum += loot.weight;
        }
    
        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = lootOptions.Count - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < lootOptions[index].weight)
            {
                break;
            }
    
            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= lootOptions[index++].weight;
        }
    
        // No other item was selected, so return very last index.
        LootProbability choice = lootOptions[index];

        return choice;
    }
}

[System.Serializable]
public class LootProbability {
    [SerializeField] public GenericPickup.PickupType type;
    [SerializeField] public float weight; 
}

