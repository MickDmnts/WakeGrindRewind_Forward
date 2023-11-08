using System;
using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// A class housing all global game events.
    /// </summary>
    [Obsolete("This class will slowly get partially or fully obsolete while the project moves away from this much event driven programming.")]
    public class GameEventsHandler
    {
        public event Action onSaveOverwrite;
        public void OnSaveOvewrite()
        {
            if (onSaveOverwrite != null)
            {
                onSaveOverwrite();
            }
        }

        /// <summary>
        /// Called whenever the game pauses
        /// </summary>
        public event Action onGamePause;
        public void OnGamePaused()
        {
            if (onGamePause != null)
            {
                onGamePause();
            }
        }

        /// <summary>
        /// Called whenever the game un-pauses
        /// </summary>
        public event Action onGameResumed;
        public void OnGameResumed()
        {
            if (onGameResumed != null)
            {
                onGameResumed();
            }
        }

        /// <summary>
        /// Called whenever the player dies.
        /// </summary>
        public event Action onPlayerDeath;
        public void OnPlayerDeath()
        {
            if (onPlayerDeath != null)
            {
                onPlayerDeath();
            }
        }

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
        /// Called from EnemyEntity script whenever a LEVEL 5 enemy dies.
        /// </summary>
        public event Action onBossLevelEnemyDeath;
        public void OnBossLevelEnemyDeath()
        {
            if (onBossLevelEnemyDeath != null)
            {
                onBossLevelEnemyDeath();
            }
        }

        /// <summary>
        /// Called from BossEntity script when he arrives at his stunned position.
        /// </summary>
        public event Action onBossStunnedPhase;
        public void OnBossStunnedPhase()
        {
            if (onBossStunnedPhase != null)
            {
                onBossStunnedPhase();
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
        /// Called whenever a new scene is loaded to move the player to a specific spot.
        /// Called from MoveToStart script.
        /// </summary>
        public event Action<Vector3> onPlayerSpawn;
        public void OnPlayerSpawn(Vector3 levelPoint)
        {
            if (onPlayerSpawn != null)
            {
                onPlayerSpawn(levelPoint);
            }
        }

        /// <summary>
        /// Called whenever the CurrentSpeed of the BulletStatics script is changed to update every FIRED bullet speed.
        /// </summary>
        public event Action<float> onBulletSpeedChange;
        public void OnBulletSpeedChange(float newSpeed)
        {
            if (onBulletSpeedChange != null)
            {
                onBulletSpeedChange(newSpeed);
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
