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

        /// <summary>
        /// Increases the current score by the passed value
        /// </summary>
        /// <param name="value">The value to increase the score by</param>
        public void IncreaseRoomScoreBy(int value)
        { currentScore += value; }

        /// <summary>
        /// Decreases the current score by the passed value
        /// </summary>
        /// <param name="value">The value to decrease the score by</param>
        public void DecreaseRoomScoreBy(int value)
        { currentScore -= value; }

        /// <summary>
        /// Increases the total score by the passed value
        /// </summary>
        /// <param name="value">The value to increase the score by</param>
        public void IncreaseTotalScoreBy(int value)
        { totalScore += value; }

        ///<summary>Sets the room score to 0</summary>
        public void ResetRoomScore()
        { currentScore = 0; }

        ///<summary>Sets the total score to 0</summary>
        public void ResetTotalScore()
        { totalScore = 0; }
    }
}