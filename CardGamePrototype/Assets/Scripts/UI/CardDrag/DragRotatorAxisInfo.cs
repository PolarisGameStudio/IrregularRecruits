using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct DragRotatorAxisInfo
{
    //todo: specify these somewhere
    public float m_ForceMultiplier;
    public float m_MinDegrees;
    public float m_MaxDegrees;
    public float m_RestSeconds;

    public DragRotatorAxisInfo(float forceMultiplier, float minDegrees, float maxDegrees, float restSeconds)
    {
        m_ForceMultiplier = forceMultiplier;
        m_MinDegrees = minDegrees;
        m_MaxDegrees = maxDegrees;
        m_RestSeconds = restSeconds;
    }
}