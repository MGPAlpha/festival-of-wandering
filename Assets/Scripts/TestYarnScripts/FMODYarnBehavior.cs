using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODYarnBehavior : MonoBehaviour
{
    [SerializeField]
	private GameObject fManager;
	private FMODUnity.StudioEventEmitter emitter;
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
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    
	void findTrackManager()
    {
		if (!fManager)
        {
			fManager = GameObject.Find("TrackManager");
			emitter = fManager.GetComponent<FMODUnity.StudioEventEmitter>();
		}
    }
}
