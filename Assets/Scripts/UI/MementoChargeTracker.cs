using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MementoChargeTracker : MonoBehaviour
{
    [SerializeField] private Image chargeUI;

    private void Update()
    {
        chargeUI.fillAmount = PlayerSingleton.PlayerSing.Play.MementoChargePercentage;
    }
}
