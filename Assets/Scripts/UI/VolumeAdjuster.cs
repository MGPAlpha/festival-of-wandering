using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjuster : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private FMOD.Studio.Bus masterBus;
    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;

    private void Awake()
    {
        //These guys will be funny
        masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        musicBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        sfxBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
    }

    public void UpdateMasterVolume()
    {
        masterBus.setVolume(masterSlider.value);
    }

    public void UpdateMusicVolume()
    {
        musicBus.setVolume(musicSlider.value);
    }

    public void UpdateSfxVolume()
    {
        sfxBus.setVolume(sfxSlider.value);
    }
}
