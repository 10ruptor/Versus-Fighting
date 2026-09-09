public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerGameplay playerGameplay) : base(playerGameplay){}

    public override void RegisterTransition()
    {
        // Les deux conditions s'excluent sur IsGrounded : plus besoin du garde-fou
        // !JumpBuffered, qui ne compensait que le caractere "sticky" de l'ancien input
        // et bloquait le passage en Landing pendant toute la fenetre de buffer.
        AddTransition(() => !playerGameplay.IsGrounded, playerGameplay.PlayerLandingState);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
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
