using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudSingleton : MonoBehaviour
{
    public static HudSingleton Hud {get; private set;}

    private void Awake()
    {
        Hud = this;
    }

    public void HideHud()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ShowHud()
    {
        GetComponent<CanvasGroup>().alpha = 1;
    }
}
