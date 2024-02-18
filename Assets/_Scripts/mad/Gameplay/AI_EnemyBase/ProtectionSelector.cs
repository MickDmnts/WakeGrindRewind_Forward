using WGRF.Core;

namespace WGRF.AI
{
    ///<summary>Enforcer protection action node</summary>
    public class ProtectionSelector : INode
    {
        ///<summary>The enforcer blackboard</summary>
        EnforcerNodeData nodeData;
        ///<summary>The FindMostVulnerable node</summary>
        FindMostVulnerable lowestHp;
        ///<summary>The ProtectVulnerable node</summary>
        ProtectVulnerable protectNode;

        ///<summary>Creates a ProtectionSelector instance</summary>
        public ProtectionSelector(FindMostVulnerable lowestHp, ProtectVulnerable protectNode, EnforcerNodeData nodeData)
        {
            this.nodeData = nodeData;
            this.lowestHp = lowestHp;
            this.protectNode = protectNode;
        }

        public INodeData GetNodeData()
        { return nodeData; }

        public bool Run()
        {
            if (nodeData.CanProtect)
            {
                //Continues to next baheviour when all enemies are dead.
                if (ManagerHub.S.AIHandler.GetAliveAgentCount((int)nodeData.EnemyEntity.EnemyRoom) <= 0)
                { return false; }

                if (!lowestHp.Run())
                { return protectNode.Run(); }

                return true;
            }

            return false;
        }
    }
}
