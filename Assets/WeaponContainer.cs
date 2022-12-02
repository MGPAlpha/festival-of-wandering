using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class WeaponContainer : MonoBehaviour
{
    
    [SerializeField] private PlayerWeaponBase weapon;

    [YarnCommand("give_weapon")]
    public void GiveToPlayer(int index) {
        if (index == 0) {
            PlayerSingleton.PlayerSing.Play.SetPrimary(weapon);
        } else {
            PlayerSingleton.PlayerSing.Play.SetSecondary(weapon);
        }
    }
}
