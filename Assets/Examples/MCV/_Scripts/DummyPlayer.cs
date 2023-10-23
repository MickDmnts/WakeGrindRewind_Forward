#if UNITY_EDITOR
using UnityEngine;
using WGRF.Core;

public class DummyPlayer : CoreBehaviour
{
    ///<summary>This dummy player's health value</summary>
    [SerializeField, Tooltip("This dummy player's health value")] float dummyHealth = 100f;

    /// <summary>
    /// Templation of a core behaviour script setup
    /// </summary>
    protected override void PreAwake()
    {
        //This line caches the controller from the ROOT of the gameObject, which we know that the Controller sits at.
        //Then, passes it to the SetController method of CoreBehaviour.
        SetController(transform.root.GetComponent<Controller>());

        //This line sets the controller-wide UNIQUE ID of THIS CoreBehaviour component.
        //The ID can also be set from the inspector, this is on the developer who implements the feature.
        SetID("playerEntity");

        //In case you pass the ID from the inspector don't forget to pass the appropriate variable like the line below.
        //SetID(ID);
    }

    /// <summary>
    /// Decreases the entity's health value by the value passed.
    /// </summary>
    /// <param name="attackValue">The attacker's attack value.</param>
    public void DummyAttack(float attackValue)
    { 
        dummyHealth -= attackValue;
        Debug.Log($"New player health: {dummyHealth}");
    }
}
#endif