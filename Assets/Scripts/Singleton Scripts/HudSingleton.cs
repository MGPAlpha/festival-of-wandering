using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudSingleton : MonoBehaviour
{
    public static HudSingleton Hud {get; private set;}
    [SerializeField] private float fadeSpeed = 3f;
    private CanvasGroup cg;

    private void Awake()
    {
        Hud = this;
    }

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (DialogueSingleton.Dialogue.Runner.IsDialogueRunning)
        {
            HideHud();
        } 
        else
        {
            ShowHud();
        }
        cg.alpha = Mathf.Clamp(cg.alpha, 0, 1);
    }

    public void HideHud()
    {
        cg.alpha -= fadeSpeed * Time.deltaTime;
    }

    public void ShowHud()
    {
        cg.alpha += fadeSpeed * Time.deltaTime;
    }
}
