using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Memorise brievement les intentions du joueur pour qu'un input emis pendant un etat
/// qui ne l'accepte pas encore ne soit pas perdu.
/// Sens du flux : PlayerInputController ecrit, la machine a etats lit.
/// </summary>
public class BufferedActionController : MonoBehaviour
{
    const float FallbackBufferDuration = 0.1f;

    [Header("buffer Settings")]
    [SerializeField] private BufferInputSettingsSO bufferSettings;

    private readonly Dictionary<BufferedAction.BufferedActionType, float> actionsBufferDurations = new Dictionary<BufferedAction.BufferedActionType, float>();
    private readonly List<BufferedAction> bufferedActions = new List<BufferedAction>();

    private void Awake()
    {
        if (bufferSettings == null)
        {
            Debug.LogError($"BufferedActionController : aucun BufferInputSettingsSO assigne sur {name}, repli sur {FallbackBufferDuration}s.", this);
            return;
        }

        foreach (BufferInputSettingsSO.bufferInputSetting setting in bufferSettings.settings)
        {
            if (!actionsBufferDurations.TryAdd(setting.actionType, setting.bufferDuration))
                Debug.LogWarning($"BufferedActionController : duree en double pour {setting.actionType}, la premiere est conservee.", this);
        }

        WarnAboutMissingDurations();
    }

    // Un type absent du ScriptableObject est signale au demarrage plutot que de faire
    // crasher le jeu au premier appui de la touche concernee.
    private void WarnAboutMissingDurations()
    {
        foreach (BufferedAction.BufferedActionType actionType in Enum.GetValues(typeof(BufferedAction.BufferedActionType)))
        {
            if (!actionsBufferDurations.ContainsKey(actionType))
                Debug.LogWarning($"BufferedActionController : aucune duree definie pour {actionType} dans {bufferSettings.name}, repli sur {FallbackBufferDuration}s.", this);
        }
    }

    private float DurationOf(BufferedAction.BufferedActionType actionType)
    {
        return actionsBufferDurations.TryGetValue(actionType, out float duration) ? duration : FallbackBufferDuration;
    }

    private bool IsAlive(BufferedAction action) => !action.IsExpired(DurationOf(action.ActionType));

    /// <summary>
    /// Lecture pure, destinee aux conditions de transition : elle ne modifie jamais le buffer.
    /// La consommation revient au Enter() de l'etat d'arrivee.
    /// </summary>
    public bool HasAlive(BufferedAction.BufferedActionType actionType)
    {
        return bufferedActions.Find(action => action.ActionType == actionType && IsAlive(action)) != null;
    }

    public void AddBufferedAction(BufferedAction.BufferedActionType actionType)
    {
        bufferedActions.Add(new BufferedAction(actionType, Time.time));
        Purge();
    }

    /// <summary>
    /// Retire toutes les entrees du type : marteler une touche ne doit pas mettre
    /// plusieurs actions en file d'attente.
    /// </summary>
    public void Consume(BufferedAction.BufferedActionType actionType)
    {
        bufferedActions.RemoveAll(action => action.ActionType == actionType);
    }

    /// <summary>Annule toutes les intentions en attente (knockback, respawn, fin de round...).</summary>
    public void Clear()
    {
        bufferedActions.Clear();
    }

    // La purge n'est qu'une hygiene memoire : HasAlive filtre deja sur la fraicheur.
    // La declencher a l'ecriture evite un Update par frame et toute dependance
    // a l'ordre d'execution des scripts.
    public void Purge()
    {
        bufferedActions.RemoveAll(action => !IsAlive(action));
    }

    public void PrintActionQueue()
    {
        Debug.Log(string.Join(",", bufferedActions), this);
    }
}
