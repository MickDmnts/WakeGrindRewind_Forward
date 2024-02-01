using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WGRF.Core;

namespace WGRF.UI
{
    public class ScorePanelController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI killsText;
        [SerializeField] TextMeshProUGUI timeText;
        [SerializeField] TextMeshProUGUI currentScoreTxt;
        [SerializeField] TextMeshProUGUI totalScoreTxt;
        [SerializeField] Slider closePanelSlider;
        [SerializeField] float maxCloseTime;

        void OnEnable()
        {
            ManagerHub.S.AbilityManager.AbilitiesCanActivate = false;

            closePanelSlider.gameObject.SetActive(false);

            killsText.SetText($"Kills x {ManagerHub.S.AIHandler.GetRoomAgentCount(ManagerHub.S.ActiveRoom)}");
            timeText.SetText($"Time x {ManagerHub.S.InternalTime.RoomTime}");
            currentScoreTxt.SetText("Room Score: " + ManagerHub.S.ScoreHandler.CurrentScore.ToString());
            totalScoreTxt.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString());

            StartCoroutine(AddScore());
        }

        IEnumerator AddScore()
        {
            int roomScore = ManagerHub.S.ScoreHandler.CurrentScore;

            while (roomScore >= 1)
            {
                roomScore -= 1;

                ManagerHub.S.ScoreHandler.DecreaseRoomScoreBy(1);
                ManagerHub.S.ScoreHandler.IncreaseTotalScoreBy(1);

                currentScoreTxt.SetText("Room Score: " + ManagerHub.S.ScoreHandler.CurrentScore.ToString());
                totalScoreTxt.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString());

                yield return new WaitForSecondsRealtime(0.01f);
            }

            float timer = 0f;
            closePanelSlider.gameObject.SetActive(true);
            closePanelSlider.maxValue = maxCloseTime;
            closePanelSlider.value = 0f;
            while (timer < maxCloseTime)
            {
                timer += 0.1f;
                closePanelSlider.value = timer;
                yield return new WaitForSecondsRealtime(0.1f);
            }

            ManagerHub.S.HUDHandler.CloseScoreUI();
        }

        void OnDisable()
        {
            StopAllCoroutines();
            ManagerHub.S.ScoreHandler.ResetRoomScore();
            ManagerHub.S.InternalTime.ChangeTimeScale(1.0f);

            ManagerHub.S.AbilityManager.AbilitiesCanActivate = true;
        }
    }
}