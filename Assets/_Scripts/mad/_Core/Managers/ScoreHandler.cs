namespace WGRF.Core
{
    public class ScoreHandler
    {
        ///<summary>The current score</summary>
        int currentScore;

        ///<summary>The current score</summary>
        public int CurrentScore => currentScore;

        /// <summary>
        /// Increases the current score by the passed value
        /// </summary>
        /// <param name="value">The value to increase the score by</param>
        public void IncreaseScoreBy(int value)
        { currentScore += value; }

        /// <summary>
        /// Decreases the current score by the passed value
        /// </summary>
        /// <param name="value">The value to decrease the score by</param>
        public void DecreaseScoreBy(int value)
        { currentScore += value; }

        ///<summary>Sets the score to 0</summary>
        public void ResetScore()
        { currentScore = 0; }

        ///<summary>Displays the score through the UI handler</summary>
        public void DisplayRoomScore()
        {
            //@TODO: Display the score through the UI handler
        }
    }
}