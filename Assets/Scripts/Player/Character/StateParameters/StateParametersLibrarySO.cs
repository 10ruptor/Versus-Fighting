using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Table "etat -> parametres" d'un personnage. Donnee pure, donc un ScriptableObject plutot
/// qu'un composant : deux personnages peuvent partager la meme librairie, ou n'en surcharger
/// qu'un etat.
/// Assigner le MEME asset a plusieurs etats est une decision de design, pas un raccourci :
/// PlayerStateMachine ne touche pas au collider quand deux etats consecutifs partagent leur
/// asset, ce qui garantit qu'un enchainement Idle -> Move -> Dash ne le modifie jamais.
/// </summary>
[CreateAssetMenu(fileName = "StateParametersLibrary", menuName = "Versus Fighting/State Parameters Library")]
public class StateParametersLibrarySO : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public PlayerState.StateType state;
        public StateParametersSO parameters;
    }

    [Tooltip("Servi a tout etat absent de la table. Sans lui, un etat non configure garde le collider de l'etat precedent.")]
    [SerializeField] StateParametersSO defaultParameters;

    [SerializeField] Entry[] entries = Array.Empty<Entry>();

    Dictionary<PlayerState.StateType, StateParametersSO> lookup;

    void OnEnable() => lookup = null;
    void OnValidate() => lookup = null;

    public StateParametersSO Resolve(PlayerState.StateType state)
    {
        BuildLookup();

        if (lookup.TryGetValue(state, out StateParametersSO parameters) && parameters != null)
            return parameters;

        if (defaultParameters == null)
            Debug.LogError($"{name} : aucun parametre pour {state}, et aucun asset par defaut n'est assigne.", this);

        return defaultParameters;
    }

    void BuildLookup()
    {
        if (lookup != null)
            return;

        lookup = new Dictionary<PlayerState.StateType, StateParametersSO>();

        foreach (Entry entry in entries)
        {
            if (!lookup.TryAdd(entry.state, entry.parameters))
                Debug.LogError($"{name} : deux entrees pour {entry.state}, seule la premiere est retenue.", this);
        }
    }
}
