using UnityEngine;

/// <summary>
/// Etat de projection. Enfant de PlayerAirState : il herite du cycle aerien
/// (gravite pilotee par le JumpController, restauration de la gravite en Exit)
/// mais neutralise volontairement input et controle horizontal pendant toute sa duree.
/// Il n'effectue aucun calcul de knockback : il applique le vecteur deja resolu par
/// le KnockbackController et gere timer + transitions.
/// </summary>
public class PlayerKnockedState : PlayerAirState
{
    public PlayerKnockedState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Knocked";

    HitData hitData;
    Vector3 launchVelocity;
    float knockedDuration;
    float elapsedTime;
    bool hasLeftGround;

    public HitData HitData => hitData;

    public void Initialize(HitData hitData, Vector3 launchVelocity, float knockedDuration)
    {
        this.hitData = hitData;
        this.launchVelocity = launchVelocity;
        this.knockedDuration = knockedDuration;
    }

    // Le check sol ne s'arme qu'une fois la victime reellement decollee, sinon un coup
    // encaisse au sol sortirait de l'etat des la premiere frame, avant meme que
    // l'ejection ait ete integree par la physique.
    bool HasLanded => hasLeftGround && playerGameplay.IsGrounded;
    bool IsKnockedOver => elapsedTime >= knockedDuration;

    public override void RegisterTransition()
    {
        // L'ordre d'ajout fait la priorite : CheckTransitions sort au premier match,
        // donc le sol l'emporte sur le timer et le cas simultane est resolu de fait.

        // TODO: remplacer PlayerIdleState par PlayerKnockedGroundedState des que cet etat existe.
        AddTransition(() => HasLanded, playerGameplay.PlayerIdleState);
        AddTransition(() => IsKnockedOver, playerGameplay.PlayerLandingState);
    }

    public override void Enter()
    {
        base.Enter(); // joue l'animation "Knocked"

        elapsedTime = 0f;
        hasLeftGround = false;

        // Set direct plutot qu'AddForce : l'ejection remplace le momentum courant au lieu
        // de s'y ajouter, et ne depend ni de la masse ni du fixedDeltaTime.
        // HitPosition n'est pas utilise ici : les rotations du Rigidbody sont gelees,
        // il ne sert que de cote d'ejection (deja resolu) et d'origine pour les VFX.
        playerGameplay.Rigidbody.linearVelocity = launchVelocity;

        // Le JumpController porte le domaine "physique verticale" : on l'arme en descente
        // pour que la retombee suive le poids du personnage comme tout autre etat aerien.
        playerGameplay.JumpController.BeginFall();
    }

    public override void Update()
    {
        // Pas de base.Update() : aucun input n'est lu pendant le knocked, saut compris.
        elapsedTime += Time.deltaTime;
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        if (!playerGameplay.IsGrounded)
            hasLeftGround = true;

        // Pas de base.FixedUpdate() : pas de controle horizontal aerien pendant le knocked.
        // Seule la physique verticale continue de tourner.
        playerGameplay.JumpController.ApplyVerticalPhysics(false);
    }

    // Exit() n'est pas surcharge : PlayerAirState.Exit() rend deja la main au
    // JumpController (useGravity = true), ce qui est exactement le comportement voulu.
}
