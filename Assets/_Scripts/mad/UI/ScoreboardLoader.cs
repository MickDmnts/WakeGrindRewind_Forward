using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using WGRF.Core;
using WGRF.Internal;

namespace WGRF.UI
{
    /// <summary>
    /// Responsible for loading the scoreboard data
    /// </summary>
    public class ScoreboardLoader : MonoBehaviour
    {
        ///<summary>The score data prefab</summary>
        [SerializeField, Tooltip("The score data prefab")] GameObject dataPrefab;
        ///<summary>The scroll rect to populate</summary>
        [SerializeField, Tooltip("The scroll rect to populate")] ScrollRect scrollRect;

        ///<summary>The created scoreboard elements</summary>
        GameObject[] cachedContents;

        void OnEnable()
        { StartCoroutine(PopulateContent()); }

        ///<summary>Populates the scoreboard and fills the scoreboard data from the database</summary>
        IEnumerator PopulateContent()
        {
            PlayerRecord[] records = ManagerHub.S.Database.GetAllPlayerRecords();
            cachedContents = new GameObject[records.Length];

            for (int i = 0; i < records.Length; i++)
            {
                GameObject temp = Instantiate(dataPrefab);
                temp.transform.SetParent(scrollRect.content.transform, false);
                float yPos = -i * 100f;
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, yPos);

                ScoreDataController sdc = temp.GetComponent<ScoreDataController>();
                sdc.SetName(records[i].Name);
                //sdc.SetRank(records[i].Rank);
                sdc.SetScore(records[i].Score);

                cachedContents[i] = temp;

                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < cachedContents.Length; i++)
            { Destroy(cachedContents[i]); }
        }
    }
}