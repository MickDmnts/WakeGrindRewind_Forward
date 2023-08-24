namespace WGR.Gameplay.AI
{
    /* [Node Documentation]
     *  
     *  Node responsible for running the boss phase-corresponding INode.
     *  
     * 
     */
    public class BossPhaseSelector : INode
    {
        BossNodeData bossNodeData;
        INode[] phasesInOrder;

        public BossPhaseSelector(BossNodeData bossNodeData, INode[] phasesInOrder)
        {
            this.bossNodeData = bossNodeData;
            this.phasesInOrder = phasesInOrder;
        }

        public INodeData GetNodeData()
        {
            return bossNodeData;
        }

        /// <summary>
        /// Call to switch  between different boss phase behaviours based on the Current Boss Phase value.
        /// </summary>
        /// <returns>Updates the phase Run and returns its return value.</returns>
        public bool Run()
        {
            switch (bossNodeData.GetCurrentBossPhase())
            {
                //Entry
                case BossPhase.StartingPhase:
                    return phasesInOrder[0].Run();

                //Budha room
                case BossPhase.BudhaRoomPhase:
                    return phasesInOrder[1].Run();

                //Bedroom
                case BossPhase.BedroomPhase:
                    return phasesInOrder[2].Run();

                //When the boss "dies", before he gets the dialogue prompt.
                case BossPhase.StunnedPhase:
                    return phasesInOrder[3].Run();
            }

            return false;
        }
    }
}