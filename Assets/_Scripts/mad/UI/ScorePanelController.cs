using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WGRF.AI;
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
            killsText.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
            currentScoreTxt.gameObject.SetActive(false);
            totalScoreTxt.gameObject.SetActive(false);

            StartCoroutine(ShowTexts());
        }

        IEnumerator ShowTexts()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            killsText.gameObject.SetActive(true);
            killsText.SetText($"Kills x {ManagerHub.S.AIHandler.GetRoomAgentCount(ManagerHub.S.ActiveRoom)}");
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            timeText.gameObject.SetActive(true);
            timeText.SetText($"Time - {ManagerHub.S.InternalTime.RoomTime}");
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            currentScoreTxt.gameObject.SetActive(true);
            currentScoreTxt.SetText("Room Score: " + ManagerHub.S.ScoreHandler.CurrentScore.ToString());
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            totalScoreTxt.gameObject.SetActive(true);
            totalScoreTxt.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString());
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);

            int timeScore = ManagerHub.S.InternalTime.RoomTimeInt;
            timeText.SetText($"Time - 00:00:00");
            while (timeScore >= 1)
            {
                timeScore -= 1;

                ManagerHub.S.ScoreHandler.DecreaseRoomScoreBy(1);

                currentScoreTxt.SetText("Room Score: " + ManagerHub.S.ScoreHandler.CurrentScore.ToString());

                yield return new WaitForSecondsRealtime(0.001f);
            }

            yield return new WaitForSecondsRealtime(1f);

            int roomScore = ManagerHub.S.ScoreHandler.CurrentScore;
            while (roomScore >= 1)
            {
                roomScore -= 1;

                ManagerHub.S.ScoreHandler.DecreaseRoomScoreBy(1);
                ManagerHub.S.ScoreHandler.IncreaseTotalScoreBy(1);

                currentScoreTxt.SetText("Room Score: " + ManagerHub.S.ScoreHandler.CurrentScore.ToString());
                totalScoreTxt.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString());

                yield return new WaitForSecondsRealtime(0.001f);
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

            if (ManagerHub.S.ActiveRoom == (int)EnemyRoom.Room7)
            { ManagerHub.S.HUDHandler.OpenMessageUI("Congratulations!\nYou won!"); }
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