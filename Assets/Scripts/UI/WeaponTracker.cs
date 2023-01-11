using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTracker : MonoBehaviour
{
    [SerializeField] private Image weaponIcon1;
    [SerializeField] private Image weaponIcon2;
    private CanvasGroup cg1;
    private CanvasGroup cg2;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        cg1 = weaponIcon1.GetComponent<CanvasGroup>();
        cg2 = weaponIcon2.GetComponent<CanvasGroup>();
    }

    public void Update()
    {
        if (PlayerSingleton.PlayerSing.Play.Weapons[0]) {
            cg1.alpha = 1f;
            weaponIcon1.sprite = PlayerSingleton.PlayerSing.Play.Weapons[0].WeaponIcon;
        } 
        else cg1.alpha = 0f;

        if (PlayerSingleton.PlayerSing.Play.Weapons[1]) {
            cg2.alpha = 1f;
            weaponIcon2.sprite = PlayerSingleton.PlayerSing.Play.Weapons[1].WeaponIcon;
        } 
        else cg2.alpha = 0f;
    }
}
