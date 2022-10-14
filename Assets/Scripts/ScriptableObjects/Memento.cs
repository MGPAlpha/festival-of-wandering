using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento : ScriptableObject
{
    
    [SerializeField] private string mementoName = "New Memento";
    public string MementoName { get => mementoName; }

    [SerializeField] private Sprite displaySprite;
    public Sprite DisplaySprite { get => displaySprite; }

    [SerializeField] private Weapon weapon;
    public Weapon Weapon { get => weapon; }

    [SerializeField] private int chargeRequired = 15;
    public int ChargeRequired { get => chargeRequired; }

}
