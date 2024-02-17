using System;

namespace WGRF.Core
{
    public class ScoreHandler
    {
        ///<summary>The current score</summary>
        int currentScore;
        ///<summary>Total run score</summary>
        int totalScore;

        ///<summary>The current score</summary>
        public int CurrentScore => currentScore;
        ///<summary>The total score</summary>
        public int TotalScore => totalScore;

        ///<summary>Subscribe to this event to get notified when the room score gets updated</summary>
        public event Action<int> onRoomScoreUpdated;
        void OnRoomScoreUpdated(int value)
        { onRoomScoreUpdated?.Invoke(value); }

        ///<summary>Subscribe to this event to get notified when the total score gets updated</summary>
        public event Action<int> onTotalScoreUpdated;
        void OnTotalScoreUpdated(int value)
        { onTotalScoreUpdated?.Invoke(value); }

        /// <summary>
        /// Increases the current score by the passed value
        /// </summary>
        /// <param name="value">The value to increase the score by</param>
        public void IncreaseRoomScoreBy(int value)
        {
            currentScore += value;
            OnRoomScoreUpdated(currentScore);
        }

        /// <summary>
        /// Decreases the current score by the passed value
        /// </summary>
        /// <param name="value">The value to decrease the score by</param>
        public void DecreaseRoomScoreBy(int value)
        {
            currentScore -= value;
            OnRoomScoreUpdated(currentScore);
        }

        /// <summary>
        /// Increases the total score by the passed value
        /// </summary>
        /// <param name="value">The value to increase the score by</param>
        public void IncreaseTotalScoreBy(int value)
        {
            totalScore += value;
            OnTotalScoreUpdated(totalScore);
        }

        ///<summary>Sets the room score to 0</summary>
        public void ResetRoomScore()
        { currentScore = 0; }

        ///<summary>Sets the total score to 0</summary>
        public void ResetTotalScore()
        { totalScore = 0; }
    }
}