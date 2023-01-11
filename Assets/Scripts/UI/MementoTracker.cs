using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MementoTracker : MonoBehaviour
{
    [SerializeField] private Image mementoIcon;
    private CanvasGroup cg;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        cg = mementoIcon.GetComponent<CanvasGroup>();
    }

    [SerializeField] Color activeColor = new Color(1,1,1,1);
    [SerializeField] Color disabledColor = new Color(.7f,.7f,.7f,.85f);

    public void Update()
    {
        if (PlayerSingleton.PlayerSing.Play.Spell) {
            cg.alpha = 1f;
            mementoIcon.sprite = PlayerSingleton.PlayerSing.Play.Spell.DisplaySprite;
            mementoIcon.color = PlayerSingleton.PlayerSing.Play.MementoChargePercentage >= 1 ? activeColor : disabledColor;
        } 
        else cg.alpha = 0f;
    }
}
