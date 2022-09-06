using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

public class PixelPerfectCameraFix : MonoBehaviour
{
    Camera m_Camera;
    PixelPerfectCamera m_PixelPerfectCamera;
    PixelPerfectPreFix m_PixelPerfectPreFix;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_PixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        TryGetComponent<PixelPerfectPreFix>(out m_PixelPerfectPreFix);
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
        ReversePixelSnap();
    }

    void ReversePixelSnap()
    {
        // Vector3 cameraPosition = m_Camera.transform.position;
        // Vector3 roundedCameraPosition = m_PixelPerfectCamera.RoundToPixel(cameraPosition);
        // Vector3 offset = roundedCameraPosition - cameraPosition;

        // Debug.Log(5*offset);

        Vector3 offset = m_PixelPerfectPreFix.PixelOffset;
        Debug.Log(offset.x);
        Matrix4x4 offsetMatrix = Matrix4x4.TRS(-1*offset, Quaternion.identity, new Vector3(1.0f, 1.0f, -1.0f));

        m_Camera.worldToCameraMatrix = offsetMatrix * m_Camera.transform.worldToLocalMatrix;

        // m_Camera.ResetWorldToCameraMatrix();
    }
}
