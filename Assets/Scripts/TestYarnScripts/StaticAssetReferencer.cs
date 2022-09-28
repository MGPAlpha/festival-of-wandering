using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAssetReferencer : MonoBehaviour
{
    private bool canAttackInit;
    public void StartDialogue(string dialogue) {
        //CameraSingleton.CamSingle
        DialogueSingleton.Dialogue.Runner.StartDialogue(dialogue);
        PlayerSingleton.PlayerSing.Play.StartDialogue();
    }

    public void EndDialogue() {
        PlayerSingleton.PlayerSing.Play.StopDialogue();
        CameraSingleton.ClearSwitchedCameras();
        //PlayerSingleton.PlayerSing.PInput.ActivateInput();
    }
}
