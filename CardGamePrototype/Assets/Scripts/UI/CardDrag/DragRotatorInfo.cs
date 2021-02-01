using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class DragRotatorInfo
{
    //TODO: these should
    public DragRotatorAxisInfo m_PitchInfo = new DragRotatorAxisInfo(15,-30,30,3);
    public DragRotatorAxisInfo m_RollInfo = new DragRotatorAxisInfo(20, -50, 50, 3);
}