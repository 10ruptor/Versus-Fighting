using UnityEngine;

/// <summary>
/// Domaine "orientation visuelle" : decide dans quel sens le personnage regarde,
/// a partir de sa velocite horizontale, et transmet la decision au
/// CharacterAnimatorController.
///
/// Le verrouillage est expose comme un simple booleen : ce controleur ignore
/// volontairement QUI le verrouille et POURQUOI. Aucune reference a la HFSM ni a
/// un etat particulier, le couplage reste a sens unique (l'etat commande, le
/// controleur execute).
/// </summary>
[RequireComponent(typeof(PlayerGameplay))]
public class VisualOrientationController : MonoBehaviour
{
    PlayerGameplay playerGameplay;

    PlayerGameplay.Orientation currentOrientation;
    public PlayerGameplay.Orientation CurrentOrientation => currentOrientation;

    bool orientationLocked;
    public bool OrientationLocked => orientationLocked;

    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    /// <summary>
    /// Fige (ou libere) l'orientation courante. Tant que le verrou est actif,
    /// UpdateOrientation ne touche plus au visuel, quelle que soit la velocite.
    /// </summary>
    public void SetOrientationLocked(bool locked)
    {
        orientationLocked = locked;
    }

    public void UpdateOrientation()
    {
        if (orientationLocked)
            return;

        float horizontalVelocity = playerGameplay.Rigidbody.linearVelocity.x;

        if (horizontalVelocity == 0f)
            return;

        currentOrientation = horizontalVelocity > 0f
            ? PlayerGameplay.Orientation.Right
            : PlayerGameplay.Orientation.Left;

        playerGameplay.Character.CharacterAnimatorController.VisualOrientationUpdate(currentOrientation);
    }
}
