using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenericPickup : MonoBehaviour
{
    public enum PickupType {
        None,
        Heal,
        HalfHeal,
        MaxHealth,
        FireworkCharm,
        FireworkCharmReplenish,
        Memento,
        Weapon
    }

    private SpriteRenderer _sp;
    private Interactible _in;

    [SerializeField] private PickupType type = PickupType.Heal;
    [SerializeField] private bool autoPickUp;
    [SerializeField] private bool singleUse = true;

    [SerializeField] private int fireworkReplenishAmount = 3;
    
    private bool usedUp = false;

    [SerializeField] private PickupAssetSettings assetSettings;
    
    // Start is called before the first frame update
    void Start()
    {

        _sp = GetComponent<SpriteRenderer>();
        _in = GetComponent<Interactible>();

        

        if (autoPickUp) {
            _in.SetInteractible(false);
        } else {
            _in.SetInteractible(true);
        }

        switch(type) {
            case PickupType.Heal:
                _sp.sprite = assetSettings.healSprite;
                break;
            case PickupType.HalfHeal:
                _sp.sprite = assetSettings.halfHealSprite;
                break;
            case PickupType.MaxHealth:
                _sp.sprite = assetSettings.maxHealthSprite;
                break;
            case PickupType.FireworkCharm:
                _sp.sprite = assetSettings.fireworkCharmSprite;
                break;
            case PickupType.FireworkCharmReplenish:
                _sp.sprite = assetSettings.fireworkCharmReplenishSprite;
                break;
            
        }
    }

    public void InitializeBeforeStart(PickupType type, bool autoPickUp) {
        this.type = type;
        this.autoPickUp = autoPickUp;
        this.singleUse = true;
    }

    public void InteractionTrigger() {
        if (usedUp) return;
        bool successfulInteraction = false;
        switch (type) {
            case PickupType.Heal:
                successfulInteraction = HealPlayer(2);
                break;
            case PickupType.HalfHeal:
                successfulInteraction = HealPlayer(1);
                break;
            case PickupType.MaxHealth:
                MaxHealthUp(2);
                successfulInteraction = true;
                break;
            case PickupType.FireworkCharm:
                successfulInteraction = true;
                PlayerSingleton.PlayerSing.Play.GainFireworks(1);
                break;
            case PickupType.FireworkCharmReplenish:
                if (PlayerSingleton.PlayerSing.Play.FireworkSupply < fireworkReplenishAmount) {
                    PlayerSingleton.PlayerSing.Play.ResupplyFireworks(fireworkReplenishAmount);
                    successfulInteraction = true;
                }
                break;
        }

        if (successfulInteraction) {
            if (singleUse) {
                _in.SetInteractible(false);
                usedUp = true;
            }

            switch (type) {
                case PickupType.Heal:
                case PickupType.HalfHeal:
                case PickupType.MaxHealth:
                case PickupType.FireworkCharm:
                    Destroy(this.gameObject);
                    break;
            }
        }
    }

    private bool HealPlayer(int amount) {
        if (PlayerSingleton.PlayerSing.Play.CanHeal) {
            PlayerSingleton.PlayerSing.Play.Heal(amount);
            return true;
        }
        return false;
    }

    private void MaxHealthUp(int amount = 2) {
        PlayerSingleton.PlayerSing.Play.MaxHealthUp(amount);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!usedUp && other.TryGetComponent<Player>(out Player player) && autoPickUp) {
            InteractionTrigger();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class PickupAssetSettings {
        [SerializeField] public Sprite healSprite;
        [SerializeField] public Sprite halfHealSprite;
        [SerializeField] public Sprite maxHealthSprite;
        [SerializeField] public Sprite fireworkCharmSprite;
        [SerializeField] public Sprite fireworkCharmReplenishSprite;
    }
}
