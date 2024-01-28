namespace WGRF.Interactions
{
    /// <summary>
    /// An interface used for entity interactions.  
    /// </summary>
    public interface IInteractable
    {
        ///<summary>Calls the attack interaction of the entity</summary>
        void AttackInteraction(int damage);

        /* ///<summary>The stun interaction of the entity</summary>
        void StunInteraction(); */
    }
}