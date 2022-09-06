using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Yarn.Unity;

public class CustomCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    private CinemachineFramingTransposer transposer;

    [YarnCommand("pan_camera")]
    public void PanCameraTo(GameObject gameObject, float damping) {
        if (gameObject == null) {
            Debug.Log("godDAMMIT");
        }
        Debug.Log(gameObject);
        transposer.m_XDamping = damping;
        transposer.m_YDamping = damping;
        cinemachine.m_Follow = gameObject.transform;
    }

    public void PanCameraTo(GameObject gameObject) {
        PanCameraTo(gameObject, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        cinemachine = gameObject.GetComponent<CinemachineVirtualCamera>();
        transposer = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();
        //PanCameraTo(GameObject.Find("Letter"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
