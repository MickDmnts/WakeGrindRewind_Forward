using System;
using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// A class housing all global game events.
    /// </summary>
    public class GameEventsHandler
    {
        /// <summary>
        /// Called whenever an enemy dies.
        /// </summary>
        public event Action onEnemyDeath;
        public void OnEnemyDeath()
        {
            if (onEnemyDeath != null)
            {
                onEnemyDeath();
            }
        }

        /// <summary>
        /// Called whenever an enemy dies to initiate the camera shake behaviour
        /// and passes the length and the power of the shake.
        /// </summary>
        public event Action<float, float> cameraShakeOnEnemyDeath;
        public void CameraShakeOnEnemyDeath(float lenght, float power)
        {
            if (cameraShakeOnEnemyDeath != null)
            {
                cameraShakeOnEnemyDeath(lenght, power);
            }
        }

        /// <summary>
        /// Called from player abilities and used from ThrowableEntities to change their speed and rotation speed in runtime.
        /// </summary>
        public event Action<float, float, bool> onAbilityUse;
        public void OnAbilityUse(float newSpeed, float newRotSpeed, bool isFrozen)
        {
            if (onAbilityUse != null)
            {
                onAbilityUse(newSpeed, newRotSpeed, isFrozen);
            }
        }

        /// <summary>
        /// Called from Player abilities to notify every subbed method that an ability behaviour has finished.
        /// Mainly used in ThrowableEntity.
        /// </summary>
        public event Action onAbilityEnd;
        public void OnAbilityEnd()
        {
            if (onAbilityEnd != null)
            {
                onAbilityEnd();
            }
        }

        /// <summary>
        /// Called whenever the player starts going back in his previous positions, not when the rewind ability gets used.
        /// </summary>
        /// /// <param name="value"></param>
        public event Action<bool> onPlayerRewind;
        public void OnPlayerRewind(bool value)
        {
            if (onPlayerRewind != null)
            {
                onPlayerRewind(value);
            }
        }

        /// <summary>
        /// Called from PlayerAttack to notify every subbed method that the player fired a gun.
        /// </summary>
        public event Action onPlayerShootStart;
        public void OnPlayerShootStart()
        {
            if (onPlayerShootStart != null)
            {
                onPlayerShootStart();
            }
        }

        /// <summary>
        /// Called from PlayerAttack to notify every subbed method that the player stopped firing a gun.
        /// </summary>
        public event Action onPlayerShootEnd;
        public void OnPlayerShootEnd()
        {
            if (onPlayerShootEnd != null)
            {
                onPlayerShootEnd();
            }
        }

        /// <summary>
        /// Called from PlayerAttack to notify every subbed method that the player stopped kicking.
        /// </summary>
        public event Action onPlayerKickStart;
        public void OnPlayerKickStart()
        {
            if (onPlayerKickStart != null)
            {
                onPlayerKickStart();
            }
        }
        /// <summary>
        /// Called from PlayerAttack to notify every subbed method that the player stopped kicking.
        /// </summary>
        public event Action onPlayerKickEnd;
        public void OnPlayerKickEnd()
        {
            if (onPlayerKickEnd != null)
            {
                onPlayerKickEnd();
            }
        }

        /// <summary>
        /// Called when the player picks up the bat prefab in the intro scene.
        /// </summary>
        public event Action onWeaponPickup;
        public void OnWeaponPickup()
        {
            if (onWeaponPickup != null)
            {
                onWeaponPickup();
            }
        }
    }
}
