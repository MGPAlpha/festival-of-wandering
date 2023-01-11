using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundLoader : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;
    private FMOD.Studio.Bus masterBus;


    // Start is called before the first frame update
    void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Misc/MainTheme_M2");
        instance.start();
        masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
    }

    public void PlayOnHover() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/menu_hover", transform.position);
    }

    public void PlayOnSelect() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/menu_selection", transform.position);
    }

    public void EndTrack() {
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE .ALLOWFADEOUT);
    }
}
