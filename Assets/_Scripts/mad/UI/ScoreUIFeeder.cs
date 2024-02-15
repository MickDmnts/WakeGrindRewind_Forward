using TMPro;
using UnityEngine;
using WGRF.Core;

namespace WGRF.UI
{
    public class ScoreUIFeeder : MonoBehaviour
    {
        ///<summary>The room score TMP ui element</summary>
        [SerializeField, Tooltip("The room score TMP ui element")] TextMeshProUGUI roomScoreTxt;
        ///<summary>The total score TMP ui element</summary>
        [SerializeField, Tooltip("The total score TMP ui element")] TextMeshProUGUI totalScoreTxt;

        void Start()
        {
            ManagerHub.S.ScoreHandler.onRoomScoreUpdated += UpdateRoomScoreTxt;
            ManagerHub.S.ScoreHandler.onTotalScoreUpdated += UpdateTotalScoreTxt;

            UpdateRoomScoreTxt(0);
            UpdateTotalScoreTxt(0);
        }

        /// <summary>
        /// Updates the TMP score ui element with the passed value
        /// </summary>
        /// <param name="value">The new score</param>
        void UpdateRoomScoreTxt(int value)
        { roomScoreTxt.SetText($"Score: {value}"); }

        /// <summary>
        /// Updates the TMP total score ui element with the passed value
        /// </summary>
        /// <param name="value">The new score</param>
        void UpdateTotalScoreTxt(int value)
        { totalScoreTxt.SetText($"Total Score: {value}"); }

        void OnDestroy()
        {
            ManagerHub.S.ScoreHandler.onRoomScoreUpdated -= UpdateRoomScoreTxt;
            ManagerHub.S.ScoreHandler.onTotalScoreUpdated -= UpdateTotalScoreTxt;
        }
    }
}