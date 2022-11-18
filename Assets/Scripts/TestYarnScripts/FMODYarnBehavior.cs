using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class FMODYarnBehavior : MonoBehaviour
{
    [SerializeField]
	private GameObject fManager;
	private FMODUnity.StudioEventEmitter emitter;
    private FMOD.Studio.EventInstance instance;
    private FMOD.Studio.EventInstance instance_2;
    private float combat;
    // Start is called before the first frame update
    void Start()
    {
        fManager = GameObject.Find("TrackManager");
		emitter = fManager.GetComponent<FMODUnity.StudioEventEmitter>();
		if (fManager)
		{
			Debug.Log(fManager.name);
		}
		else
		{
			Debug.Log("No TrackManager found!");
		}

		if (emitter == null)
		{
			Debug.LogWarning("No fmod emitter found, audio will not play.");
		}
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Stages/Stage Theme 2");
        instance_2 = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Misc/WoodlandAmbience");
        //Playtrack("Misc/WoodlandAmbience");
        instance.start();
        instance_2.start();
	}

    // Update is called once per frame
    void Update()
    {
        FindTrackManager();
        //will need to update this for final build
        if (RoomArea.InCombat) {
            combat += .1f;
        } else if (!RoomArea.InCombat && combat > 0) {
            combat -=.1f;
        }
        SetParameter(instance, "Combat", combat);
    }

    
	void FindTrackManager()
    {
		if (!fManager)
        {
			fManager = GameObject.Find("TrackManager");
			emitter = fManager.GetComponent<FMODUnity.StudioEventEmitter>();
		}
    }

    [YarnCommand("play_track")]
    void Playtrack(string name) {
		instance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/" + name);
        instance.start();
    }

    void SetParameter(FMOD.Studio.EventInstance e, string name, float value) {
	    e.setParameterByName(name, value);
	}

    [YarnCommand("stop_track")]
    void StopTrack() {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
        instance.clearHandle();
    }

    [YarnCommand("stop_all_tracks")]
    void StopAllTracks() {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
        instance.clearHandle();
        instance_2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance_2.release();
        instance_2.clearHandle();
    }
}
