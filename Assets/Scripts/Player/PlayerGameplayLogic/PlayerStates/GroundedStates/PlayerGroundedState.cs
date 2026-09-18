public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine){}
    private bool shield =>  playerGameplay.PlayerInputController.Shield;

    // Bouton tenu OU appui recent encore en buffer : un appui bref emis pendant un etat qui
    // ne l'acceptait pas (fin d'attaque, atterrissage) n'est pas perdu pour autant.
    // Le bouclier doit aussi etre en mesure de se lever : casse ou trop entame, on reste.
    private bool shieldRequested => (shield || playerGameplay.PlayerInputController.ShieldPressedBuffered)
                                    && playerGameplay.ShieldController.CanRaiseShield;

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.IsGrounded, stateMachine.Landing);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump, stateMachine.Jumping);
        AddTransition( () => shieldRequested && playerGameplay.IsGrounded, stateMachine.Shield );
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
    }
}
