using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MementoTracker : MonoBehaviour
{
    [SerializeField] private Image mementoIcon;
    public void Update()
    {
        mementoIcon.sprite = PlayerSingleton.PlayerSing.Play.Spell.DisplaySprite;
    }
}
