namespace WGRF.Bus
{
    /// <summary>
    /// A struct in a database digestible format for database rows.
    /// </summary>
    public struct PlayerRecord
    {
        ///<summary>Player's rank</summary>
        public int Rank;
        ///<summary>Player's name</summary>
        public string Name;
        ///<summary>Player's score</summary>
        public int Score;

        /// <summary>
        /// Constructs a new player record instance
        /// </summary>
        /// <param name="rank">Player's rank</param>
        /// <param name="name">Player's name</param>
        /// <param name="score">Player's score</param>
        public PlayerRecord(int rank, string name, int score)
        {
            this.Rank = rank;
            this.Name = name;
            this.Score = score;
        }
    }
}