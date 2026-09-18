/// <summary>
/// Verdict du bouclier sur un coup entrant. C'est la seule chose que le reste du jeu a
/// besoin de connaitre : le HitReceptionController s'en sert pour decider si le coup
/// poursuit sa route vers le KnockbackController.
/// </summary>
public enum ShieldHitOutcome
{
    /// <summary>Bouclier baisse, casse, ou trop petit pour couvrir le point d'impact : le coup passe normalement.</summary>
    NotShielded,

    /// <summary>Coup absorbe : degats annules (hors chip), durabilite entamee, court recul.</summary>
    Blocked,

    /// <summary>Appui bouclier dans la fenetre de contre : l'attaquant encaisse et se fait ejecter.</summary>
    Parried
}
