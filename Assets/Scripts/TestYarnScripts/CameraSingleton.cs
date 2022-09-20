using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Yarn.Unity;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton CamSingle {get; private set;}
    private CinemachineVirtualCamera initCam;
    private CinemachineFramingTransposer transposer;

    [YarnCommand("switch_camera")]
    public void SwitchCamera(GameObject gameObject, float damping) {
        
    }

    void Awake() {
        CamSingle = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //cinemachine = gameObject.GetComponent<CinemachineVirtualCamera>();
        //transposer = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();
        //PanCameraTo(GameObject.Find("Letter"));
    }
}