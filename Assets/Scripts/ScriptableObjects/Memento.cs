using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Memento", menuName = "Festival/Memento")]
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
