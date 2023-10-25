#if UNITY_EDITOR
using UnityEngine;
using WGRF.Core;

public class DummyDbTester : MonoBehaviour
{
    public int rank;
    public string plName;
    public int score;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(ManagerHub.S.Database.AddPlayerRecord(new WGRF.Bus.PlayerRecord(rank, plName, score)))
            { Debug.Log("record successfully inserted"); }
        }
    }
}
#endif