namespace WGR.AI.Nodes
{
    /* [Node Documentation]
     * Universal node that just runs and returns the phaseBehaviour INode value.
     * 
     * [Must know]
     * 1. BossNodeData compatible.
     */
    public class BossPhaseUpdater : INode
    {
        BossNodeData bossNodeData;
        INode phaseBehaviour;

        public BossPhaseUpdater(BossNodeData bossNodeData, INode phaseBehaviour)
        {
            this.bossNodeData = bossNodeData;
            this.phaseBehaviour = phaseBehaviour;
        }

        public INodeData GetNodeData()
        {
            return bossNodeData;
        }

        public bool Run()
        {
            return phaseBehaviour.Run();
        }
    }
}