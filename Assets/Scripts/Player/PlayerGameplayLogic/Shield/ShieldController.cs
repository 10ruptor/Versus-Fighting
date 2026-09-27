using System;
using UnityEngine;

/// <summary>
/// Domaine "bouclier" : durabilite, taille, et verdict sur un coup entrant
/// (rien / blocage / contre).
///
/// Il ne connait ni la machine a etats ni les inputs au dela d'un horodatage : c'est
/// PlayerShieldState qui decide QUAND lever et baisser, et le HitReceptionController qui
/// decide quoi faire du verdict. Ce controleur ne repond qu'a "ce coup est-il arrete, et a
/// quel prix ?".
///
/// La fenetre de contre appartient au bouclier (ShieldDataSO), pas au buffer d'input : le
/// buffer se contente de dater l'appui, la fenetre est confrontee a cette date au moment
/// exact du HIT.
/// </summary>
[RequireComponent(typeof(PlayerGameplay))]
public class ShieldController : MonoBehaviour
{
    PlayerGameplay playerGameplay;

    /// <summary>
    /// Publie le verdict d'un coup arrive sur le bouclier. Point de sortie generique pour
    /// les observateurs (etincelle de blocage, flash de contre, son, secousse de
    /// camera...) sans que ce controleur ait a les connaitre.
    /// </summary>
    public event Action<HitData, ShieldHitOutcome> ShieldHitResolved;

    /// <summary>Emis quand la durabilite tombe a zero : le bouclier casse et devient inutilisable pour un temps.</summary>
    public event Action ShieldBroken;

    ShieldDataSO Data => playerGameplay.Character != null ? playerGameplay.Character.ShieldData : null;
    CharacterShield Shield => playerGameplay.Character != null ? playerGameplay.Character.Shield : null;

    float durability;
    bool isRaised;

    // Instant limite pour contrer, fige au lever du bouclier a partir de la date de l'appui
    // qui l'a leve : un appui supplementaire bouclier deja leve ne le repousse donc pas,
    // la fenetre se merite a chaque nouvelle activation.
    float counterWindowEnd = float.NegativeInfinity;

    // Fin du blockstun. Tant qu'il court, le bouclier absorbe sans nouvel effet : une meme
    // attaque touchant a la fois le volume du bouclier et une hurtbox, ou portant plusieurs
    // hitbox, n'est ainsi comptee qu'une fois.
    float absorbCooldownEnd = float.NegativeInfinity;

    float brokenUntil = float.NegativeInfinity;
    float lastDurabilityLossTime = float.NegativeInfinity;

    public bool IsRaised => isRaised;
    public float Durability => durability;
    public bool IsBroken => Time.time < brokenUntil;
    public float DurabilityRatio => Data != null ? Data.DurabilityRatioOf(durability) : 0f;

    /// <summary>True tant que le contre est encore possible sur cette activation.</summary>
    public bool IsWithinCounterWindow => Time.time <= counterWindowEnd;

    public bool CanRaiseShield => Data != null && !IsBroken && DurabilityRatio >= Data.minDurabilityRatioToRaise;

    void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    // Start et non Awake : le Character est instancie dans le Awake du PlayerGameplay, dont
    // l'ordre d'execution vis a vis de ce composant n'est pas garanti.
    void Start()
    {
        if (Data == null)
        {
            Debug.LogWarning($"ShieldController : aucun ShieldDataSO sur le Character de {name}, le bouclier restera inutilisable.", this);
            return;
        }

        durability = Data.maxDurability;
        SyncShieldSize();
    }

    void Update()
    {
        if (Data == null)
            return;

        if (isRaised)
            DrainOverTime();
        else
            RegenerateOverTime();

        SyncShieldSize();
    }

    #region Lever / baisser

    /// <summary>
    /// Leve le bouclier et arme la fenetre de contre a partir de la date du dernier appui
    /// bouclier. Utiliser la date de l'appui plutot que celle du lever est volontaire : un
    /// appui bufferise, honore quelques frames plus tard, ne doit pas recuperer une fenetre
    /// de contre pleine.
    /// </summary>
    public void RaiseShield()
    {
        if (Data == null)
        {
            Debug.LogError($"ShieldController : RaiseShield sans ShieldDataSO sur {name}.", this);
            return;
        }

        isRaised = true;
        counterWindowEnd = playerGameplay.PlayerInputController.LastShieldPressTime + Data.counterParryWindow;
        absorbCooldownEnd = float.NegativeInfinity;

        SyncShieldSize();

        if (Shield != null)
            Shield.SetActive(true);
    }

    public void LowerShield()
    {
        isRaised = false;
        counterWindowEnd = float.NegativeInfinity;
        absorbCooldownEnd = float.NegativeInfinity;

        // La durabilite ne repart pas immediatement : le delai de regeneration court a
        // partir du moment ou le bouclier cesse d'etre sollicite.
        lastDurabilityLossTime = Time.time;

        if (Shield != null)
            Shield.SetActive(false);
    }

    #endregion

    #region Coup entrant

    /// <summary>
    /// Rend le verdict du bouclier sur un coup, en applique les consequences cote defenseur
    /// (et cote attaquant sur un contre), et renvoie ce qu'il advient du coup.
    /// </summary>
    public ShieldHitOutcome ResolveHit(HitData hitData, bool contactOnShield)
    {
        if (!isRaised || Data == null || hitData.AttackData == null)
            return ShieldHitOutcome.NotShielded;

        // Un bouclier entame est un bouclier retreci : les zones qu'il ne couvre plus
        // laissent passer le coup normalement.
        if (!contactOnShield && !CoversHit(hitData.HitPosition))
            return ShieldHitOutcome.NotShielded;

        // Deja en train d'encaisser : le coup est absorbe, mais ne compte pas deux fois.
        if (Time.time < absorbCooldownEnd)
            return ShieldHitOutcome.Blocked;

        ShieldHitOutcome outcome = IsWithinCounterWindow && TryCounter(hitData)
            ? ShieldHitOutcome.Parried
            : ShieldHitOutcome.Blocked;

        if (outcome == ShieldHitOutcome.Blocked)
            ApplyBlock(hitData);

        absorbCooldownEnd = Time.time + Data.blockPushbackDuration;

        ShieldHitResolved?.Invoke(hitData, outcome);
        return outcome;
    }

    bool CoversHit(Vector3 hitPosition)
    {
        CharacterShield shield = Shield;
        return shield != null && shield.Covers(hitPosition);
    }

    /// <summary>
    /// Coup absorbe : les degats sont annules (hors chip damage), la durabilite trinque, et
    /// le defenseur est pousse vers l'arriere. Ce recul n'est PAS une ejection : il est
    /// confie a l'etat bouclier, qui le joue sans passer par PlayerKnockedState.
    /// </summary>
    void ApplyBlock(HitData hitData)
    {
        float damage = hitData.AttackData.TotalDamage;

        if (Data.chipDamageRatio > 0f)
            playerGameplay.DamageController.AddDamage(damage * Data.chipDamageRatio);

        // Le recul est joue avant le debit de durabilite : si ce coup casse le bouclier, le
        // joueur aura quand meme ete repousse par le coup qu'il vient de bloquer.
        float side = HitDirection.ResolveHorizontalSide(transform.position, hitData);
        Vector3 pushback = new Vector3(side * Data.blockPushbackSpeed, 0f, 0f);

        playerGameplay.StateMachine.Shield.ApplyBlockPushback(pushback, Data.blockPushbackDuration);

        ConsumeDurability(damage * Data.durabilityCostPerDamage);
    }

    /// <summary>
    /// Contre : l'attaque configuree dans le ShieldDataSO est renvoyee a l'attaquant par son
    /// propre KnockbackController. Le contre emprunte donc exactement le meme chemin qu'un
    /// coup normal (% encaisse puis ejection), sans pipeline parallele a maintenir. Le point
    /// de contact declare est la position du defenseur : l'attaquant part a l'oppose, comme
    /// repousse par le bouclier.
    /// </summary>
    bool TryCounter(HitData hitData)
    {
        if (hitData.Attacker == null)
            return false;

        if (Data.counterAttackData == null)
        {
            Debug.LogWarning($"ShieldController : counterAttackData manquant dans {Data.name}, le contre retombe sur un simple blocage.", this);
            return false;
        }

        ConsumeDurability(Data.counterDurabilityCost);

        HitData counterHit = new HitData
        {
            Attacker = playerGameplay,
            AttackData = Data.counterAttackData,
            HitPosition = transform.position,
            HurtedHurtbox = null
        };

        // Adresse directement au KnockbackController de l'attaquant, et non a sa reception
        // de coups : un contre est une punition, il ne se rejoue pas contre ses defenses
        // (et deux contres ne peuvent donc pas se renvoyer l'un l'autre).
        hitData.Attacker.KnockbackController.Knockback(counterHit);
        return true;
    }

    #endregion

    #region Durabilite

    void ConsumeDurability(float amount)
    {
        if (amount <= 0f)
            return;

        durability = Mathf.Max(0f, durability - amount);
        lastDurabilityLossTime = Time.time;

        if (durability <= 0f)
            Break();
    }

    void DrainOverTime()
    {
        ConsumeDurability(Data.durabilityDrainPerSecond * Time.deltaTime);
    }

    void RegenerateOverTime()
    {
        if (durability >= Data.maxDurability)
            return;

        if (Time.time < lastDurabilityLossTime + Data.durabilityRegenDelay)
            return;

        durability = Mathf.Min(Data.maxDurability, durability + Data.durabilityRegenPerSecond * Time.deltaTime);
    }

    /// <summary>
    /// Casse du bouclier : il tombe et reste inutilisable le temps configure. L'etat
    /// bouclier s'en apercoit via IsRaised et rend la main de lui meme, sans que ce
    /// controleur ait a piloter la machine a etats.
    /// </summary>
    void Break()
    {
        durability = 0f;
        brokenUntil = Time.time + Data.brokenDuration;
        LowerShield();
        ShieldBroken?.Invoke();
    }

    /// <summary>La taille suit la durabilite : un bouclier entame couvre moins.</summary>
    void SyncShieldSize()
    {
        CharacterShield shield = Shield;

        if (shield == null)
            return;

        shield.SetSizeRatio(Mathf.Lerp(Data.minSizeRatio, 1f, DurabilityRatio));
    }

    #endregion
}
