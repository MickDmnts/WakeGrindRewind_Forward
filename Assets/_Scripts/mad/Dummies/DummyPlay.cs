#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WGRF.Core;

public class DummyPlay : MonoBehaviour
{
    void Start()
    {
        ManagerHub.S.StageHandler.LoadRun();
    }
}
#endif