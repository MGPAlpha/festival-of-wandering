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
    [SerializeField] private Sprite healthFull;
    [SerializeField] private Sprite healthHalf;
    [SerializeField] private Sprite healthEmpty;

    private void Start()
    {
        trackedMaxHealth = transform.childCount * 2;
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
            int priorCount = transform.childCount * 2;
            // If there aren't enough Health Prefabs already, add more!
            if (priorCount < PlayerSingleton.PlayerSing.Play.MaxHealth)
            {
                for (int n = 0; n < (PlayerSingleton.PlayerSing.Play.MaxHealth - priorCount) / 2; n++)
                {
                    Instantiate(healthPrefab).transform.SetParent(transform, false);
                    transform.GetChild(transform.childCount - 1).GetComponent<Image>().sprite = healthEmpty;
                }
            }

            for (int n = trackedMaxHealth / 2; n < (PlayerSingleton.PlayerSing.Play.MaxHealth / 2); n++)
                transform.GetChild(n).gameObject.SetActive(true);
        }
        // Else the current Player Max health is less than what the UI is tracking
        // then hide the latter health icons till the number is dropped 
        else
        {
            for (int n = (trackedMaxHealth / 2) - 1; n >= (PlayerSingleton.PlayerSing.Play.MaxHealth / 2); n--)
                transform.GetChild(n).gameObject.SetActive(false);
        }

        trackedMaxHealth = PlayerSingleton.PlayerSing.Play.MaxHealth;
    }

    private void UpdateHealthValue()
    {

        // If the Player Healed then reflect that healing
        if (PlayerSingleton.PlayerSing.Play.Health > trackedCurrHealth)
        {
            int lengthOfIteration = ((PlayerSingleton.PlayerSing.Play.Health + 1) / 2);
            for (int n = trackedCurrHealth / 2; n < lengthOfIteration; n++)
            {
                if (n == lengthOfIteration - 1 && PlayerSingleton.PlayerSing.Play.Health % 2 == 1)
                {
                    transform.GetChild(n).GetComponent<Image>().sprite = healthHalf;
                }
                else
                {
                    transform.GetChild(n).GetComponent<Image>().sprite = healthFull;
                }
            }
        }             
        // Else the only other change is that the player lost health, reflect that as well
        else
        {
            int lengthOfIteration = PlayerSingleton.PlayerSing.Play.Health / 2;
            for (int n = ((trackedCurrHealth - 1) / 2); n >= lengthOfIteration; n--)
            {
                if (n == lengthOfIteration && PlayerSingleton.PlayerSing.Play.Health % 2 == 1)
                {
                    transform.GetChild(n).GetComponent<Image>().sprite = healthHalf;
                }
                else
                {
                    transform.GetChild(n).GetComponent<Image>().sprite = healthEmpty;
                }
            }
             
        }
            

        trackedCurrHealth = PlayerSingleton.PlayerSing.Play.Health;
    }
}
