namespace WGR.Core.Managers
{
    /*
     * The purpose of this file is to cache the public field which are externally set,
     * before and after game saving.
     */
    [System.Serializable]
    public class SaveDataInfo
    {
        public int RemainingSkillPoints;
        public int[] AbilityTiersInOrder; // [Slow, Rewind, Stop]
        public int[] GunKillsInOrder; // [Order of guns in weapon manager loader]
        public int PlayerDeaths;
    }
}