#if UNITY_EDITOR
using UnityEngine;
using WGRF.Core;

public class DummySettingsTester : MonoBehaviour
{
    public bool gore;
    public int master;
    public int ost;
    public int sfx;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ManagerHub.S.SettingsHandler.UpdateUserSettings(new UserSettings()
            {
                goreVFX = gore,
                masterVolume = master,
                ostVolume = ost,
                sfxVolume = sfx
            });
        }
    }
}
#endif