using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using WGRF.Core;
using WGRF.Internal;

namespace WGRF.UI
{
    public class ScoreboardLoader : MonoBehaviour
    {
        [SerializeField] GameObject dataPrefab;
        [SerializeField] ScrollRect scrollRect;

        GameObject[] cachedContents;

        void OnEnable()
        {
            StartCoroutine(PopulateContent());
        }

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
                sdc.SetRank(records[i].Rank);
                sdc.SetScore(records[i].Score);

                cachedContents[i] = temp;

                yield return new WaitForSecondsRealtime(0.05f);
            }
        }
    }
}