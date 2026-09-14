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
    protected override string StateAnimationName => "Knocked";

    HitData hitData;
    Vector3 launchVelocity;
    float knockedDuration;
    float elapsedTime;
    bool hasLeftGround;

    public HitData HitData => hitData;

    public void InitializeHit(HitData hitData, Vector3 launchVelocity, float knockedDuration)
    {
        this.hitData = hitData;
        this.launchVelocity = launchVelocity;
        this.knockedDuration = knockedDuration;
    }

    // Le check sol ne s'arme qu'une fois la victime reellement decollee, sinon un coup
    bool HasLanded => hasLeftGround && stateMachine.PlayerGameplay.IsGrounded;
    bool IsKnockedOver => elapsedTime >= knockedDuration;

    public override void RegisterTransition()
    {

        // TODO: remplacer PlayerIdleState par PlayerKnockedGroundedState des que cet etat existe.
        AddTransition(() => HasLanded, stateMachine.stateLibrary[StateType.Idle]);
        AddTransition(() => IsKnockedOver, stateMachine.stateLibrary[StateType.Landing]);
    }

    public override void OnEnable()
    {
        
        base.OnEnable(); // joue l'animation "Knocked"
        
        // this is to ensure only one hit is taken into account ( to update in the futur if issues for combo )
        stateMachine.PlayerGameplay.Character.HurtBoxManager.DisableAllHurtboxesCollider();
        // we keep the orientation at the moment of the hit
        stateMachine.PlayerGameplay.VisualOrientationController.SetOrientationLocked(true);
        // buffer is ignored when knocked
        stateMachine.PlayerGameplay.PlayerInputController.ClearBuffer();
        
        elapsedTime = 0f;
        hasLeftGround = false;

        stateMachine.PlayerGameplay.Rigidbody.linearVelocity = launchVelocity;

        // Le JumpController porte le domaine "physique verticale" : on l'arme en descente pour que la retombee suive le poids du personnage comme tout autre etat aerien.
        stateMachine.PlayerGameplay.JumpController.BeginFall();
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        if (!stateMachine.PlayerGameplay.IsGrounded)
            hasLeftGround = true;

        // Pas de base.FixedUpdate() : pas de controle horizontal aerien pendant le knocked. Seule la physique verticale continue de tourner.
        stateMachine.PlayerGameplay.JumpController.ApplyVerticalPhysics(false);
    }

    public override void OnDisable()
    {
        // base.Exit() (PlayerAirState) rend la main au JumpController : useGravity = true.
        base.OnDisable();
        stateMachine.PlayerGameplay.Character.HurtBoxManager.EnableAllHurtboxesCollider();
        stateMachine.PlayerGameplay.VisualOrientationController.SetOrientationLocked(false);
    }
}
