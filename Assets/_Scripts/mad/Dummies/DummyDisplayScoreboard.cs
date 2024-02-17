#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.Core;

public class DummyDisplayScoreboard : MonoBehaviour
{
    void Update()
    {
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            ManagerHub.S.HUDHandler.OpenMessageUI("Dummy msg");
            ManagerHub.S.HUDHandler.OpenScoreboardUI();
        }
    }
}
#endif