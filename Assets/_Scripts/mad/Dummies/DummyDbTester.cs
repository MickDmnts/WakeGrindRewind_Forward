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
            if (ManagerHub.S.Database.AddPlayerRecord(new WGRF.Bus.PlayerRecord(rank, plName, score)).Result)
            { Debug.Log("record successfully inserted"); }
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ManagerHub.S.Database.UpdatePlayerRecord(plName, new WGRF.Bus.PlayerRecord(rank, plName, score)).Result)
            { Debug.Log("record successfully updated"); }
        }*/
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ManagerHub.S.Database.UpdatePlayerRecord(rank, new WGRF.Bus.PlayerRecord(rank, plName, score)).Result)
            { Debug.Log("record successfully updated"); }
        }

        /*if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ManagerHub.S.Database.DeletePlayerRecord(plName).Result)
            { Debug.Log("record successfully deleted"); }
        }*/
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ManagerHub.S.Database.DeletePlayerRecord(rank).Result)
            { Debug.Log("record successfully deleted"); }
        }
    }
}
#endif