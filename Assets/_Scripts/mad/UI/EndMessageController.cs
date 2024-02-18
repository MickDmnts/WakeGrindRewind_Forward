using TMPro;
using UnityEngine;

using WGRF.Core;
using WGRF.Internal;

namespace WGRF.UI
{
    /// <summary>
    /// A UI controller responsible for controlling the UI message panel
    /// </summary>
    public class EndMessageController : MonoBehaviour
    {
        ///<summary>The message text</summary>
        [SerializeField, Tooltip("The message text")] TextMeshProUGUI messageText;
        ///<summary>The total score text</summary>
        [SerializeField, Tooltip("The total score text")] TextMeshProUGUI totalScoreText;
        ///<summary>The player name input field</summary>
        [SerializeField, Tooltip("The player name input field")] TMP_InputField nameField;

        void OnEnable()
        { totalScoreText.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString()); }

        /// <summary>
        /// Sets the message text to the passed string
        /// </summary>
        /// <param name="txt">The message</param>
        public void SetMessageText(string txt)
        { messageText.SetText(txt); }

        /// <summary>
        /// Writes the score to the database and displays the ScoreboardUI.
        /// </summary>
        public void FinalizeScore()
        {
            if (nameField.text.Length <= 0)
            {
                nameField.image.color = Color.red;
                return;
            }

            nameField.image.color = Color.white;
            int score = ManagerHub.S.ScoreHandler.TotalScore;
            PlayerRecord record = new PlayerRecord() { Rank = ManagerHub.S.Database.GetPlayerRecordCount() + 1, Name = nameField.text, Score = score };
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ManagerHub.S.Database.AddPlayerRecord(record);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            ManagerHub.S.HUDHandler.CloseMessageUI();
            ManagerHub.S.HUDHandler.OpenScoreboardUI();
        }
    }
}