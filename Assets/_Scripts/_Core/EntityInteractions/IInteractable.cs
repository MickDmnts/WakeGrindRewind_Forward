using UnityEngine;

namespace WGR.Core
{
    /*
     * [Must Know]
     * Used in Entity class
     */
    public interface IInteractable
    {
        //for story development
        //void Speakinteraction();

        void AttackInteraction();

        void StunInteraction();
    }
}