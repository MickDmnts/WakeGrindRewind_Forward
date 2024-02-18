using TMPro;
using UnityEngine;

using WGRF.Core;
using WGRF.Internal;

namespace WGRF.UI
{
    public class EndMessageController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] TextMeshProUGUI totalScoreText;
        [SerializeField] TMP_InputField nameField;

        void OnEnable()
        {
            totalScoreText.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString());
        }

        public void SetMessageText(string txt)
        { messageText.SetText(txt); }

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