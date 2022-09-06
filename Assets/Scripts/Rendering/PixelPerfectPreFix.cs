using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

public class PixelPerfectPreFix : MonoBehaviour
{
    Camera m_Camera;
    PixelPerfectCamera m_PixelPerfectCamera;
    public Vector3 PixelOffset { get; private set; } = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_PixelPerfectCamera = GetComponent<PixelPerfectCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
    }

    void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras) {
        Vector3 cameraPosition = m_Camera.transform.position;
        Vector3 roundedCameraPosition = m_PixelPerfectCamera.RoundToPixel(cameraPosition);
        Vector3 offset = roundedCameraPosition - cameraPosition;
        offset.z = -offset.z;
        PixelOffset = offset;
        Debug.Log(offset);
    }
}
