using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MementoTracker : MonoBehaviour
{
    [SerializeField] private Image mementoIcon;
    public void Update()
    {
        if (PlayerSingleton.PlayerSing.Play.Spell) {
            mementoIcon.enabled = false;
            mementoIcon.sprite = PlayerSingleton.PlayerSing.Play.Spell.DisplaySprite;
        } 
        else mementoIcon.enabled = false;
    }
}
