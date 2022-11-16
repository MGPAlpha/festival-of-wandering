using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    [SerializeField] private CinemachineCameraOffset _cco;
    [SerializeField] private float offsetSpeed = 10.0f;
    [SerializeField] private float offsetThreshold = 0.05f;
    private Vector3 _cursorOffset = Vector3.zero;

    public void OffsetTo(Vector3 offset)
    {
        _cursorOffset = offset;
    }

    private void Update()
    {
        if (Vector3.Magnitude(_cursorOffset - _cco.m_Offset) < offsetThreshold)
        {
            _cco.m_Offset = _cursorOffset;
            return;
        }
        float movePercentage = Mathf.Clamp01(offsetSpeed * Time.deltaTime);
        _cco.m_Offset = Vector3.Lerp(_cco.m_Offset, _cursorOffset, movePercentage);
    }
}
