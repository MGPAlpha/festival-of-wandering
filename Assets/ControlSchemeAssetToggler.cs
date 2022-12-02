using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSchemeAssetToggler : MonoBehaviour
{

    [SerializeField] private CanvasGroup keyboardOption;
    [SerializeField] private CanvasGroup gamepadOption;



    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // Debug.Log(PlayerSingleton.PlayerSing.PInput);
        if (PlayerSingleton.PlayerSing) PlayerSingleton.PlayerSing.Play.OnControlSchemeChanged += ControlsChanged;
        if (PlayerSingleton.PlayerSing) ControlsChanged(PlayerSingleton.PlayerSing.PInput.currentControlScheme);
    }
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        PlayerSingleton.PlayerSing.Play.OnControlSchemeChanged -= ControlsChanged;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        OnEnable();
        Debug.Log("IS anything working!!!");
    }
    


    private void ControlsChanged(string newScheme) {
        Debug.Log("Checking current control scheme");
        if (newScheme == "Gamepad") {
            if (keyboardOption) keyboardOption.alpha = 0;
            if (gamepadOption) gamepadOption.alpha = 1;
        } else {
            if (keyboardOption) keyboardOption.alpha = 1;
            if (gamepadOption) gamepadOption.alpha = 0;
        }
    }

}
