namespace WGRF.AI.Nodes
{
    /* [Node documentation]
     * 
     * [Custom Selector]
     *  If the AI is marked as stunned run the mainBTEntry INode, else run the stunedAction INode.
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     */
    public class CheckIfStunned : INode
    {
        EnemyNodeData nodeData;

        INode mainBTEntry;
        INode stunedAction;

        public CheckIfStunned(EnemyNodeData nodeData, INode stunedAction, INode mainBTEntry)
        {
            this.nodeData = nodeData;

            this.stunedAction = stunedAction;
            this.mainBTEntry = mainBTEntry;
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to check if the agent is marked as stunned.
        /// <para>If true, run the mainBTEntry INode and return its return value.</para>
        /// <para>If false, run the stunnedAction INode and return its return value.</para>
        /// </summary>
        public bool Run()
        {
            if (!nodeData.GetIsStunned())
            {
                return mainBTEntry.Run();
            }
            else
            {
                return stunedAction.Run();
            }
        }
    }
}