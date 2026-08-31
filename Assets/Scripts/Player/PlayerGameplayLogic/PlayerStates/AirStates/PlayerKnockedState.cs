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
    bool HasLanded => hasLeftGround && playerGameplay.IsGrounded;
    bool IsKnockedOver => elapsedTime >= knockedDuration;

    public override void RegisterTransition()
    {

        // TODO: remplacer PlayerIdleState par PlayerKnockedGroundedState des que cet etat existe.
        AddTransition(() => HasLanded, playerGameplay.PlayerIdleState);
        AddTransition(() => IsKnockedOver, playerGameplay.PlayerLandingState);
    }

    public override void Enter()
    {
        base.Enter(); // joue l'animation "Knocked"

        elapsedTime = 0f;
        hasLeftGround = false;

        // L'orientation est figee sur celle de l'impact. Sans ce verrou, le
        // VisualOrientationController - qui oriente selon la velocite horizontale -
        // retournerait le personnage vers sa destination d'ejection.
        playerGameplay.VisualOrientationController.SetOrientationLocked(true);

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

    public override void Exit()
    {
        // base.Exit() (PlayerAirState) rend la main au JumpController : useGravity = true.
        base.Exit();

        playerGameplay.VisualOrientationController.SetOrientationLocked(false);
    }
}
