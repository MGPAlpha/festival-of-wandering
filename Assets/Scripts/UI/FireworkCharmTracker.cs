using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkCharmTracker : MonoBehaviour
{
    private int trackedCharmAmt;
    [SerializeField] private GameObject charmIconPrefab;

    private void Start()
    {
        trackedCharmAmt = transform.childCount;
    }

    private void Update()
    {
        if (trackedCharmAmt != PlayerSingleton.PlayerSing.Play.FireworkSupply) UpdateFirecCharmUI();
    }

    private void UpdateFirecCharmUI()
    {
        if (PlayerSingleton.PlayerSing.Play.FireworkSupply < 0)
        {
            return;
        }
        // If the current Player Max Health is greater than the what the UI is tracking
        // then display the missing health Icons
        if (PlayerSingleton.PlayerSing.Play.FireworkSupply > trackedCharmAmt)
        {
            // If there aren't enough Health Prefabs already, add more!
            if (transform.childCount < PlayerSingleton.PlayerSing.Play.FireworkSupply)
            {
                int priorCount = transform.childCount;
                for (int n = 0; n < PlayerSingleton.PlayerSing.Play.FireworkSupply - priorCount; n++)
                    Instantiate(charmIconPrefab).transform.SetParent(transform, false);
            }

            for (int n = trackedCharmAmt; n < PlayerSingleton.PlayerSing.Play.FireworkSupply; n++)
                transform.GetChild(n).gameObject.SetActive(true);
        }
        // Else the current Player Max health is less than what the UI is tracking
        // then hide the latter health icons till the number is dropped 
        else
        {
            for (int n = trackedCharmAmt - 1; n >= PlayerSingleton.PlayerSing.Play.FireworkSupply; n--)
                transform.GetChild(n).gameObject.SetActive(false);
        }

        trackedCharmAmt = PlayerSingleton.PlayerSing.Play.FireworkSupply;
    }

    //Could implement and animation of removing a firework supply -> probably adding a crossfade animation effect ;p
    private void PopFireworkCharmIcon()
    {

    }
}
