using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WGRF.AI;
using WGRF.Core;

namespace WGRF.UI
{
    /// <summary>
    /// The score panel responsible for the score panel ui elements
    /// </summary>
    public class ScorePanelController : MonoBehaviour
    {
        ///<summary>The room kills text</summary>
        [SerializeField, Tooltip("The room kills text")] TextMeshProUGUI killsText;
        ///<summary>The room time text</summary>
        [SerializeField, Tooltip("The room time text")] TextMeshProUGUI timeText;
        ///<summary>The room score text</summary>
        [SerializeField, Tooltip("The room score text")] TextMeshProUGUI currentScoreTxt;
        ///<summary>The total score text</summary>
        [SerializeField, Tooltip("The total score text")] TextMeshProUGUI totalScoreTxt;
        ///<summary>The slider for countdown until the window closes</summary>
        [SerializeField, Tooltip("The slider for countdown until the window closes")] Slider closePanelSlider;
        ///<summary>The time until the window closes when the texts are filled in</summary>
        [SerializeField, Tooltip("The time until the window closes when the texts are filled in")] float maxCloseTime;

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

        ///<summary>Starts the score diplay sequence</summary>
        IEnumerator ShowTexts()
        {
            //Show kills
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            killsText.gameObject.SetActive(true);
            killsText.SetText($"Kills x {ManagerHub.S.AIHandler.GetRoomAgentCount(ManagerHub.S.ActiveRoom)}");

            //Show time
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            timeText.gameObject.SetActive(true);
            timeText.SetText($"Time - {ManagerHub.S.InternalTime.RoomTime}");

            //Show current score
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            currentScoreTxt.gameObject.SetActive(true);
            currentScoreTxt.SetText("Room Score: " + ManagerHub.S.ScoreHandler.CurrentScore.ToString());

            //Show total score
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);
            totalScoreTxt.gameObject.SetActive(true);
            totalScoreTxt.SetText("Total Score: " + ManagerHub.S.ScoreHandler.TotalScore.ToString());

            //Show close slider
            yield return new WaitForSecondsRealtime(0.5f);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pistol);

            //Fill in the time score
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

            //Fill in the room score to total score
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

            //Show the close slider
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

            //Game end message
            if (ManagerHub.S.ActiveRoom == (int)EnemyRoom.Room7)
            { ManagerHub.S.HUDHandler.OpenMessageUI("Congratulations!\nYou won!"); }
        }

        void OnDisable()
        {
            StopAllCoroutines();
            ManagerHub.S.ScoreHandler.ResetRoomScore();
            ManagerHub.S.InternalTime.ChangeTimeScale(1.0f);

            ManagerHub.S.AbilityManager.AbilitiesCanActivate = true;
            ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Crosshair);
        }
    }
}