namespace WGRF.AI
{
    /// <summary>
    /// A base class for ai entity behaviour tree handling
    /// </summary>
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