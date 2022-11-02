using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    [SerializeField] private CinemachineCameraOffset _cco;
    [SerializeField] private float offsetSpeed = 17.0f;
    [SerializeField] private float offsetThreshold = 0.05f;

    public void OffsetTo(Vector3 offset, float ts)
    {
        if (Vector3.Magnitude(offset - _cco.m_Offset) < offsetThreshold)
        {
            _cco.m_Offset = offset;
            return;
        }
        // _cco.m_Offset = offset;
        _cco.m_Offset = Vector3.Lerp(_cco.m_Offset, offset, offsetSpeed * ts);
    }
}
