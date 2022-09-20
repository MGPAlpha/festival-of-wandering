using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAssetReferencer : MonoBehaviour
{
    private bool canAttackInit;
    public void StartDialogue(string dialogue) {
        //PlayerSingleton.PlayerSing.PInput.DeactivateInput();
        canAttackInit = PlayerSingleton.PlayerSing.Play.CanAttack;
        if (canAttackInit) {
            PlayerSingleton.PlayerSing.Play.CanAttack = false;
        }
        PlayerSingleton.PlayerSing.Play.CanMove = false;
        CameraSingleton.CamSingle
        DialogueSingleton.Dialogue.Runner.StartDialogue(dialogue);
    }

    public void EndDialogue() {
        if (canAttackInit) {
            PlayerSingleton.PlayerSing.Play.CanAttack = true;
        }
        PlayerSingleton.PlayerSing.Play.CanMove = true;
        //PlayerSingleton.PlayerSing.PInput.ActivateInput();
    }
}
