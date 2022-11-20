using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class StaticAssetReferencer : MonoBehaviour
{
    private bool canAttackInit;
    public void StartDialogue(string dialogue) {
        //CameraSingleton.CamSingle
        DialogueSingleton.Dialogue.Runner.StartDialogue(dialogue);
        PlayerSingleton.PlayerSing.Play.StartDialogue();
        Debug.Log("Dialogue Started");
    }

    public void EndDialogue() {
        PlayerSingleton.PlayerSing.Play.StopDialogue();
        CameraSingleton.ClearSwitchedCameras();
        CameraSingleton.ChangeCameraSpeed(1.5f);
        //PlayerSingleton.PlayerSing.PInput.ActivateInput();
        Debug.Log("Dialogue Ended");
    }

    [YarnCommand("return_to_start")]
    public static void ReturnToStart() {
        FMODYarnBehavior fmodMusic = FindObjectOfType<FMODYarnBehavior>();
        fmodMusic.StopAllTracks();
        SceneManager.LoadScene(0);
    }
}
