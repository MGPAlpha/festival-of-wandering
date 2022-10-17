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
        instance.start();
	}

    // Update is called once per frame
    void Update()
    {
        FindTrackManager();
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
        Debug.Log("WHAT");
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
}
