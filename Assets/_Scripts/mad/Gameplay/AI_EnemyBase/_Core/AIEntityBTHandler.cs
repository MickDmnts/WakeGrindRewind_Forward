namespace WGRF.AI.Entities
{
    /* [CLASS DOCUMENTATION]
     * 
     * Base abstract class for AI entity behaviour tree creation and handling.
     * 
     * [Must know]
     * 1. Stores the prototype node data of its AI Entity gameObject.
     * 
     */
    public abstract class AIEntityBTHandler
    {
        /// <summary>
        /// Call to start this agents' Behaviour Tree creation.
        /// </summary>
        protected abstract void CreateBehaviourTree();

        /// <summary>
        /// Call to update the agent Behaviour Tree from its root.
        /// </summary>
        public abstract void UpdateBT();
    }
}