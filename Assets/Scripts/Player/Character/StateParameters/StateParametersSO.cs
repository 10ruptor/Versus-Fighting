using System;
using UnityEngine;

/// <summary>
/// Parametres de gameplay d'un etat, dependants du personnage : deux Yokai n'ont pas la
/// meme capsule accroupie. L'asset est resolu par StateParametersLibrarySO et injecte dans
/// l'etat au demarrage, puis lu par le controleur concerne au moment ou l'etat est entre.
/// </summary>
[CreateAssetMenu(fileName = "StateParameters", menuName = "Versus Fighting/State Parameters")]
public class StateParametersSO : ScriptableObject
{
    /// <summary>
    /// Capsule ABSOLUE, appliquee telle quelle a l'entree de l'etat. Volontairement pas un
    /// multiplicateur ni un calcul depuis le mesh : le collider ne doit bouger qu'aux
    /// instants choisis, jamais frame par frame.
    /// Le bas de la capsule vaut center.y - height / 2 : garder cette valeur constante d'un
    /// etat a l'autre garde les pieds au sol.
    /// </summary>
    [Serializable]
    public struct ColliderSettings
    {
        public float height;
        public float radius;
        public Vector3 center;

        // Une capsule a hauteur ou rayon nul traverse le decor : on refuse de l'appliquer.
        public bool IsValid => height > 0f && radius > 0f;
    }

    [SerializeField]
    ColliderSettings colliderSettings = new ColliderSettings
    {
        // Valeurs de la capsule authoree sur le prefab PlayerGameplay : un asset neuf
        // reproduit le comportement actuel au lieu d'ecraser le collider avec des zeros.
        height = 2f,
        radius = 0.5f,
        center = Vector3.zero,
    };

    public ColliderSettings Collider => colliderSettings;
}
