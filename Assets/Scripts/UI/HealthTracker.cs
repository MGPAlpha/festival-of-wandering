using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Figure out how to make this run while not in playing mode!
public class HealthTracker : MonoBehaviour
{
    private int trackedMaxHealth;
    private int trackedCurrHealth;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private Sprite healthFill;
    [SerializeField] private Sprite healthEmpty;

    private void Start()
    {
        trackedMaxHealth = transform.childCount;
        trackedCurrHealth = 0;
    }

    private void Update()
    {
        // Checks if maxHealth changes
        if (trackedMaxHealth != PlayerSingleton.PlayerSing.Play.MaxHealth) UpdateHealthUI();     
   
        // Checks if the current Health changes
        if (trackedCurrHealth != PlayerSingleton.PlayerSing.Play.Health) UpdateHealthValue();        
    }

    private void UpdateHealthUI()
    {
        if (PlayerSingleton.PlayerSing.Play.MaxHealth < 0)
        {
            return;
        }
        // If the current Player Max Health is greater than the what the UI is tracking
        // then display the missing health Icons
        if (PlayerSingleton.PlayerSing.Play.MaxHealth > trackedMaxHealth)
        {
            // If there aren't enough Health Prefabs already, add more!
            if (transform.childCount < PlayerSingleton.PlayerSing.Play.MaxHealth)
            {
                int priorCount = transform.childCount;
                for (int n = 0; n < PlayerSingleton.PlayerSing.Play.MaxHealth - priorCount; n++)
                    Instantiate(healthPrefab).transform.SetParent(transform, false);
            }

            for (int n = trackedMaxHealth; n < PlayerSingleton.PlayerSing.Play.MaxHealth; n++)
                transform.GetChild(n).gameObject.SetActive(true);
        }
        // Else the current Player Max health is less than what the UI is tracking
        // then hide the latter health icons till the number is dropped 
        else
        {
            for (int n = trackedMaxHealth - 1; n >= PlayerSingleton.PlayerSing.Play.MaxHealth; n--)
                transform.GetChild(n).gameObject.SetActive(false);
        }

        trackedMaxHealth = PlayerSingleton.PlayerSing.Play.MaxHealth;
    }

    private void UpdateHealthValue()
    {
        // If the Player Healed then reflect that healing
        if (PlayerSingleton.PlayerSing.Play.Health > trackedCurrHealth)
            for (int n = trackedCurrHealth; n < PlayerSingleton.PlayerSing.Play.Health; n++)
                transform.GetChild(n).GetComponent<Image>().sprite = healthFill;
        // Else the only other change is that the player lost health, reflect that as well
        else
            for (int n = trackedCurrHealth - 1; n >= PlayerSingleton.PlayerSing.Play.Health; n--)
                transform.GetChild(n).GetComponent<Image>().sprite = healthEmpty;

        trackedCurrHealth = PlayerSingleton.PlayerSing.Play.Health;
    }
}
